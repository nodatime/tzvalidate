// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using CommandLine;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NodaTime.TzValidate.ZicDump
{
    /// <summary>
    /// Dumps all the time zones from a particular zic output directory or file in tzvalidate format.
    /// </summary>
    public class Program
    {
        public int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }

            var file = options.Source;
            if (File.Exists(file))
            {
                ProcessFile(file, file, options, Console.Out);
            }
            else if (Directory.Exists(file))
            {
                var writer = new StringWriter();
                ProcessDirectory(file, options, writer);
                var text = writer.ToString();
                WriteHeaders(text, options, Console.Out);
                Console.Write(text);
            }
            else
            {
                Console.Error.WriteLine($"File not found: {file}");
                return 1;
            }
            return 0;
        }

        private static void ProcessDirectory(string directory, Options options, TextWriter writer)
        {
            var pairs = Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories)
                .Select(file => new
                {
                    file,
                    id = file.StartsWith(directory) ?
                        file.Substring(directory.Length).Replace("\\", "/").TrimStart('/') : file
                })
                .Where(pair => pair.id != "zone.tab")
                .OrderBy(pair => pair.id, StringComparer.Ordinal)
                .ToList();
            if (options.ZoneId != null)
            {
                var pair = pairs.FirstOrDefault(p => p.id == options.ZoneId);
                if (pair == null)
                {
                    throw new Exception($"Unknown zone ID: {options.ZoneId}");
                }
                ProcessFile(pair.id, pair.file, options, writer);
            }
            else
            {
                foreach (var pair in pairs)
                {
                    ProcessFile(pair.id, pair.file, options, writer);
                }
            }
        }

        private static void ProcessFile(string id, string file, Options options, TextWriter writer)
        {
            writer.Write($"{id}\n");
            using (var stream = File.OpenRead(file))
            {
                var zone = ZoneFile.FromStream(stream);
                var transitions = zone.GetTransitions(options);
                foreach (var transition in transitions)
                {
                    writer.Write($"{transition}\n");
                }
            }
            writer.Write("\n");
        }

        private static void WriteHeaders(string text, Options options, TextWriter writer)
        {
            if (options.Version != null)
            {
                writer.Write($"Version: {options.Version}\n");
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
    }
}
