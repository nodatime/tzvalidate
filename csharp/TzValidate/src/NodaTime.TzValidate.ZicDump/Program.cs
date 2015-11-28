// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using CommandLine;
using System;
using System.IO;
using System.Linq;

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
                ProcessFile(file, file, options);
            }
            else if (Directory.Exists(file))
            {
                ProcessDirectory(file, options);
            }
            else
            {
                Console.Error.WriteLine($"File not found: {file}");
                return 1;
            }
            return 0;
        }

        private static void ProcessDirectory(string directory, Options options)
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
                ProcessFile(pair.id, pair.file, options);
            }
            else
            {
                foreach (var pair in pairs)
                {
                    ProcessFile(pair.id, pair.file, options);
                }
            }
        }

        private static void ProcessFile(string id, string file, Options options)
        {
            Console.Write($"{id}\r\n");
            using (var stream = File.OpenRead(file))
            {
                var zone = ZoneFile.FromStream(stream);
                var transitions = zone.GetTransitions(options);
                foreach (var transition in transitions)
                {
                    Console.Write($"{transition}\r\n");
                }
            }
            Console.Write("\r\n");
        }
    }
}
