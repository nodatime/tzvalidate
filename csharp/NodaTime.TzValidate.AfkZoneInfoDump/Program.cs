// Copyright 2018 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Afk.ZoneInfo;
using CommandLine;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NodaTime.TzValidate.AfkZoneInfoDump
{
    class Program
    {
        static int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }

            if (!string.IsNullOrEmpty(options.DataDirectory))
            {
                Environment.SetEnvironmentVariable("TZDIR", options.DataDirectory);
            }

            using (var output = options.OutputFile == null ? Console.Out : File.CreateText(options.OutputFile))
            {
                if (options.ZoneId != null)
                {
                    var zone = TzTimeInfo.GetZones().Single(z => z.Name == options.ZoneId);
                    Dump(zone, options, output);
                }
                else
                {
                    var writer = new StringWriter();
                    var zones = TzTimeInfo.GetZones().OrderBy(zone => zone.Name, StringComparer.Ordinal);
                    foreach (var zone in zones)
                    {
                        Dump(zone, options, writer);
                    }
                    var text = writer.ToString();
                    WriteHeaders(text, options, output);
                    output.Write(text);
                }
            }
            return 0;
        }

        private static void Dump(TzTimeZone zone, Options options, TextWriter writer)
        {
            writer.Write($"{zone.Name}\n");

            // Note: we don't get daylight/standard or appropriate names from Afk.TimeZone.
            var initial = new DateTimeOffset(2, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            var initialOffset = zone.GetUtcOffset(initial);
            writer.Write("Initially:           {0}\n",
                (initialOffset.Ticks >= 0 ? "+" : "-") + initialOffset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture));

            int fromYear = options.FromYear ?? 1800;
            DateTimeOffset start = new DateTimeOffset(fromYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset end = new DateTimeOffset(options.ToYear, 1, 1, 0, 0, 0, TimeSpan.Zero);

            DateTimeOffset? transition = GetNextTransition(zone, start.AddTicks(-1), end);
            while (transition != null)
            {
                var offset = zone.GetUtcOffset(transition.Value);
                writer.Write("{0} {1}\n",
                    transition.Value.ToString("yyyy-MM-dd HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                    (offset.Ticks >= 0 ? "+" : "-") + offset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture));
                transition = GetNextTransition(zone, transition.Value, end);
            }
            writer.Write("\n");
        }

        private static DateTimeOffset? GetNextTransition(TzTimeZone zone, DateTimeOffset start, DateTimeOffset end)
        {
            TimeSpan startOffset = zone.GetUtcOffset(start);
            DateTimeOffset now = start.AddDays(1);
            while (now <= end)
            {
                if (zone.GetUtcOffset(now) != startOffset)
                {
                    // Right, so there's a transition strictly after now - (one day), and less than or equal to now. Binary search...
                    long upperInclusiveTicks = now.Ticks;
                    long lowerExclusiveTicks = now.AddDays(-1).Ticks;
                    while (upperInclusiveTicks > lowerExclusiveTicks + 1)
                    {
                        long candidateTicks = (upperInclusiveTicks + lowerExclusiveTicks) / 2;
                        var candidateDto = new DateTimeOffset(candidateTicks, TimeSpan.Zero);
                        if (zone.GetUtcOffset(candidateDto) != startOffset)
                        {
                            // Still seeing a difference: look earlier
                            upperInclusiveTicks = candidateTicks;
                        }
                        else
                        {
                            // Same as at start of day: look later
                            lowerExclusiveTicks = candidateTicks;
                        }
                    }
                    // If we turn out to have hit the end point, we're done without a final transition. 
                    if (upperInclusiveTicks == end.Ticks)
                    {
                        return null;
                    }
                    else if (upperInclusiveTicks != start.UtcDateTime.Ticks + 1)
                    {
                        return new DateTimeOffset(upperInclusiveTicks, TimeSpan.Zero);
                    }
                    // Otherwise, we've hit https://github.com/LZorglub/TimeZone/issues/1
                    // Skip a day and continue
                }
                now = now.AddDays(1);
            }
            return null;
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
