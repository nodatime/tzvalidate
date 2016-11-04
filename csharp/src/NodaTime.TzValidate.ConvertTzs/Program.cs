// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using SharpCompress.Readers.Tar;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NodaTime.TzValidate.ConvertTzs
{
    public class Program
    {
        static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }

            using (var output = options.OutputFile == null ? Console.Out : File.CreateText(options.OutputFile))
            {
                var source = LoadSource(options);
                Dump(output, source, options);
            }
            return 0;
        }

        private static TzsFile LoadSource(Options options)
        {
            byte[] data = LoadFileOrUrl(options.Source);
            // Realistically, there's always going to be a link to start with...
            if (data.Length > 4 && Encoding.UTF8.GetString(data, 0, 4) == "Link")
            {
                return TzsFile.Parse(new StringReader(Encoding.UTF8.GetString(data)), options.Version);
            }
            return FromArchive(new MemoryStream(data));
        }

        private static byte[] LoadFileOrUrl(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://") || source.StartsWith("ftp://"))
            {
                using (var client = new HttpClient())
                {
                    // I know using .Result is nasty, but we're in a console app, and nothing is
                    // going to deadlock...
                    return client.GetAsync(source).Result.EnsureSuccessStatusCode().Content.ReadAsByteArrayAsync().Result;
                }
            }
            return File.ReadAllBytes(source);
        }

        internal static TzsFile FromArchive(Stream archiveData)
        {
            string version = null;
            MemoryStream data = null;
            using (var reader = TarReader.Open(archiveData))
            {
                while (reader.MoveToNextEntry())
                {
                    if (reader.Entry.IsDirectory)
                    {
                        continue;
                    }
                    if (reader.Entry.Key.EndsWith("/version"))
                    {
                        var entryStream = new MemoryStream();
                        reader.WriteEntryTo(entryStream);
                        version = Encoding.UTF8.GetString(entryStream.ToArray()).Trim();
                    }
                    if (reader.Entry.Key.EndsWith("/to2050.tzs"))
                    {
                        data = new MemoryStream();
                        reader.WriteEntryTo(data);
                        data.Position = 0;
                    }
                }
            }
            if (data == null)
            {
                throw new Exception("Unable to find to2050.tzs in archive");
            }
            return TzsFile.Parse(new StreamReader(data), version);
        }


        private static void Dump(TextWriter writer, TzsFile source, Options options)
        {
            var zoneMap = source.Zones.ToDictionary(z => z.Id);
            var zones = source.Zones
                .Concat(source.Aliases.Select(pair => new TzsZone(pair.Key, zoneMap[pair.Value].Transitions)))
                .OrderBy(z => z.Id, StringComparer.Ordinal)
                .Where(z => z.Id != "Factory")
                .ToList();
            if (options.ZoneId != null)
            {
                if (options.HashOnly)
                {
                    // TODO: Maybe allow this after all, and include the zone ID in the headers?
                    throw new Exception("Cannot use hash option with a single zone ID");
                }
                var zone = zones.FirstOrDefault(z => z.Id == options.ZoneId);
                if (zone == null)
                {
                    throw new Exception($"Unknown zone ID: {options.ZoneId}");
                }
                DumpZone(zone, writer, options);
            }
            else
            {
                var bodyWriter = new StringWriter();
                foreach (var zone in zones)
                {
                    DumpZone(zone, bodyWriter, options);
                }
                var text = bodyWriter.ToString();
                if (options.HashOnly)
                {
                    writer.Write(ComputeHash(text) + "\n");
                }
                else
                {
                    WriteHeaders(text, source.Version, options, writer);
                    writer.Write(text);
                }
            }

        }

        private static void DumpZone(TzsZone zone, TextWriter writer, Options options)
        {
            writer.Write($"{zone.Id}\n");
            var transitions = zone.GetTransitions(options);
            foreach (var transition in transitions)
            {
                writer.Write($"{transition}\n");
            }
            writer.Write("\n");
        }

        private static void WriteHeaders(string text, string version, Options options, TextWriter writer)
        {
            if (version != null)
            {
                writer.Write($"Version: {version}\n");
            }
            using (var hashAlgorithm = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = hashAlgorithm.ComputeHash(bytes);
                var hashText = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                writer.Write($"Body-SHA-256: {hashText}\n");
            }
            writer.Write("Format: tzvalidate-0.1\n");
            writer.Write($"Range: {options.FromYear ?? 1}-{options.ToYear}\n");
            writer.Write($"Generator: {typeof(Program).GetTypeInfo().Assembly.GetName().Name}\n");
            writer.Write($"GeneratorUrl: https://github.com/nodatime/tzvalidate\n");
            writer.Write("\n");
        }

        /// <summary>
        /// Computes the SHA-256 hash of the given text after encoding it as UTF-8,
        /// and returns the hash in lower-case hex.
        /// </summary>
        private static string ComputeHash(string text)
        {
            using (var hashAlgorithm = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(text);
                var hash = hashAlgorithm.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
