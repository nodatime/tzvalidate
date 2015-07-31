// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using System;
using System.Globalization;
using System.Linq;

namespace NodaTime.TzValidate.TimeZoneInfoDump
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

            if (options.ZoneId != null)
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(options.ZoneId);
                Dump(zone, options);
            }
            else
            {
                var zones = TimeZoneInfo.GetSystemTimeZones().OrderBy(zone => zone.Id, StringComparer.Ordinal);
                foreach (var zone in zones)
                {
                    Dump(zone, options);
                    Console.Write("\r\n");
                }
            }

            return 0;
        }

        private static void Dump(TimeZoneInfo zone, Options options)
        {
            Console.Write($"{zone.Id}\r\n");

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
                Console.WriteLine("{0} {1} {2} {3}",
                    transition.Value.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                    (offset.Ticks >= 0 ? "+" : "-") + offset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture),
                    isDaylight ? "daylight" : "standard",
                    isDaylight ? zone.DaylightName : zone.StandardName);
                transition = GetNextTransition(zone, transition.Value, end);
            }
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
                        var candidateDto = new DateTimeOffset(new DateTime(candidateTicks, DateTimeKind.Utc), TimeSpan.Zero);
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
                        ? (DateTimeOffset?) null
                        : new DateTimeOffset(new DateTime(upperInclusiveTicks, DateTimeKind.Utc), TimeSpan.Zero);
                }
                now = now.AddDays(1);
            }
            return null;
        }
   }
}
