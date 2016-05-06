// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace NodaTime.TzValidate.TimeZoneInfoDump
{
    class Program
    {
        public int Main(string[] args)
        {
            Options options = new Options();
            ICommandLineParser parser = new CommandLineParser(new CommandLineParserSettings(Console.Error) { MutuallyExclusive = true });
            if (!parser.ParseArguments(args, options))
            {
                return 1;
            }

            if (options.ZoneId != null)
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(options.ZoneId);
                Dump(zone, options, Console.Out);
            }
            else
            {
                var writer = new StringWriter();
                var zones = TimeZoneInfo.GetSystemTimeZones().OrderBy(zone => zone.Id, StringComparer.Ordinal);
                foreach (var zone in zones)
                {
                    Dump(zone, options, writer);
                }
                var text = writer.ToString();
                WriteHeaders(text, options, Console.Out);
                Console.Write(text);
            }

            return 0;
        }

        private static void Dump(TimeZoneInfo zone, Options options, TextWriter writer)
        {
            Console.Write($"{zone.Id}\n");

            // This will be a bit odd using Windows time zones, as most have permanent
            // daylight saving rules... but for tz data, it should be okay.
            var initial = new DateTimeOffset(2, 1, 1, 0, 0, 0, 0, TimeSpan.Zero);
            var initialOffset = zone.GetUtcOffset(initial);
            var initialDaylight = zone.IsDaylightSavingTime(initial);
            Console.WriteLine("Initially:           {0} {1} {2}",
                (initialOffset.Ticks >= 0 ? "+" : "-") + initialOffset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture),
                initialDaylight ? "daylight" : "standard",
                initialDaylight ? zone.DaylightName : zone.StandardName);

            int fromYear = options.FromYear ?? 1800;
            DateTimeOffset start = new DateTimeOffset(fromYear, 1, 1, 0, 0, 0, TimeSpan.Zero);
            DateTimeOffset end = new DateTimeOffset(options.ToYear, 1, 1, 0, 0, 0, TimeSpan.Zero);

            DateTimeOffset? transition = GetNextTransition(zone, start.AddTicks(-1), end);
            while (transition != null)
            {
                var offset = zone.GetUtcOffset(transition.Value);
                var isDaylight = zone.IsDaylightSavingTime(transition.Value);
                // It's unfortunate that TimeZoneInfo doesn't support the idea of different names
                // for different periods in history. Never mind - this is better than nothing,
                // for diagnostic purposes.
                Console.Write("{0} {1} {2} {3}\n",
                    transition.Value.ToString("yyyy-MM-dd HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                    (offset.Ticks >= 0 ? "+" : "-") + offset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture),
                    isDaylight ? "daylight" : "standard",
                    isDaylight ? zone.DaylightName : zone.StandardName);
                transition = GetNextTransition(zone, transition.Value, end);
            }
            writer.Write("\n");
        }

        private static DateTimeOffset? GetNextTransition(TimeZoneInfo zone, DateTimeOffset start, DateTimeOffset end)
        {
            TimeSpan startOffset = zone.GetUtcOffset(start);
            bool startDaylight = zone.IsDaylightSavingTime(start);
            DateTimeOffset now = start.AddDays(1);
            while (now <= end)
            {
                if (zone.GetUtcOffset(now) != startOffset || zone.IsDaylightSavingTime(now) != startDaylight)
                {
                    // Right, so there's a transition strictly after now - (one day), and less than or equal to now. Binary search...
                    long upperInclusiveTicks = now.Ticks;
                    long lowerExclusiveTicks = now.AddDays(-1).Ticks;
                    while (upperInclusiveTicks > lowerExclusiveTicks + 1)
                    {
                        long candidateTicks = (upperInclusiveTicks + lowerExclusiveTicks) / 2;
                        var candidateDto = new DateTimeOffset(candidateTicks, TimeSpan.Zero);
                        if (zone.GetUtcOffset(candidateDto) != startOffset || zone.IsDaylightSavingTime(candidateDto) != startDaylight)
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
                    return upperInclusiveTicks == end.Ticks
                        ? (DateTimeOffset?)null
                        : new DateTimeOffset(upperInclusiveTicks, TimeSpan.Zero);
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
