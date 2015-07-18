// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using NodaTime;
using NodaTime.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProcessZdumpOutput
{
    /// <summary>
    /// Processes the output of the accompanying shell scripts: a pair of text files,
    /// one containing transitions for each zone and one containing the current time for each zone.
    /// After processing, the result is written to the console in tzvalidate format.
    /// </summary>
    class Program
    {
        private static readonly Regex NowRegex = new Regex(@"^[^ ]*  (?<local>.*\d\d\d\d) (?<name>.*)$");
        private static readonly Regex TransitionRegex = new Regex(@"^[^ ]*  (?<utc>.*) UT = (?<local>.*\d\d\d\d) (?<name>.*) isdst=(?<isdst>.) gmtoff=(?<gmtoff>[-\d]*)$");
        private static readonly Instant Start = Instant.FromUtc(1905, 1, 1, 0, 0);
        private static readonly Instant End = Instant.FromUtc(2035, 1, 1, 0, 0);
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.GeneralPattern;
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");
        private static readonly IPattern<LocalDateTime> LocalPattern = LocalDateTimePattern.CreateWithInvariantCulture("ddd MMM d HH:mm:ss yyyy");

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.Error.WriteLine("Usage: ProcessZdumpOutput <transition file> <now file>");
                return;
            }

            var transitionLines = File.ReadLines(args[0])
                .Where(x => x != "") // Ignore empty lines
                .Where((value, index) => index % 2 == 1) // Skip odd-numbered lines
                .Where(x => !x.Contains("= NULL")) // Ignore start/end lines
                .ToList();

            var nowLines = File.ReadLines(args[1])
                .Where(x => x != "") // Ignore empty lines
                .ToList();

            var allIds = nowLines.Select(ExtractId).ToList();
            allIds.Sort(StringComparer.Ordinal);
            var prefix = FindIdPrefix(allIds);

            var transitionPairs = from line in transitionLines
                                  let transition = ConvertTransition(line)
                                  where transition != null
                                  select new { id = ExtractId(line), transition };
            var transitionOutput = transitionPairs.ToLookup(pair => pair.id, pair => pair.transition);

            // Work out roughly when we ran, but finding the line for the UTC zone.
            var utcNowLine = nowLines.Single(line => line.StartsWith(prefix + "UTC "));
            Instant runTime = ExtractRunTimeForUtc(utcNowLine);

            var nowOutput = nowLines.ToDictionary(ExtractId, line => ConvertNowLine(line, runTime));

            foreach (var id in allIds)
            {
                Console.Write(id.Substring(prefix.Length) + "\r\n");
                if (transitionOutput.Contains(id))
                {
                    foreach (var line in transitionOutput[id])
                    {
                        Console.Write(line);
                    }
                }
                else
                {
                    Console.Write(nowOutput[id]);
                }
                Console.Write("\r\n");
            }
        }

        private static Instant ExtractRunTimeForUtc(string utcNowLine)
        {
            var match = NowRegex.Match(utcNowLine);
            if (!match.Success)
            {
                throw new Exception("Invalid UTC line: " + utcNowLine);
            }
            var local = LocalPattern.Parse(match.Groups["local"].Value.Replace("  ", " ")).Value;
            return local.InUtc().ToInstant();
        }

        private static string ConvertNowLine(string line, Instant runTime)
        {
            var match = NowRegex.Match(line);
            if (!match.Success)
            {
                throw new Exception("Invalid line: " + line);
            }
            var local = LocalPattern.Parse(match.Groups["local"].Value.Replace("  ", " ")).Value;
            var name = match.Groups["name"].Value;
            var offsetAsDuration = local.InUtc().ToInstant() - runTime;
            // Round it to the nearest 5 minutes.
            var roundedSeconds = Math.Round(offsetAsDuration.TotalSeconds / 300) * 300;
            var offset = Offset.FromSeconds((int) roundedSeconds);

            return string.Format("Fixed: {0} {1}\r\n", OffsetPattern.Format(offset), name);
        }

        private static string ExtractId(string line)
        {
            return line.Split(' ')[0];
        }

        private static string ConvertTransition(string line)
        {
            var match = TransitionRegex.Match(line);
            if (!match.Success)
            {
                throw new Exception("Invalid transition line: " + line);
            }
            var utc = LocalPattern.Parse(match.Groups["utc"].Value.Replace("  ", " ")).Value;
            var local = LocalPattern.Parse(match.Groups["local"].Value.Replace("  ", " ")).Value;
            var name = match.Groups["name"].Value;
            var isStandard = match.Groups["isdst"].Value == "0";
            var offsetInMillis = Period.Between(utc, local, PeriodUnits.Milliseconds).Milliseconds;
            var offset = Offset.FromMilliseconds((int) offsetInMillis);
            var gmtoff = int.Parse(match.Groups["gmtoff"].Value);
            if (gmtoff != offsetInMillis / 1000)
            {
                throw new Exception("Inconsistent offsets in transition line: " + line);
            }

            var utcInstant = utc.InUtc().ToInstant();
            if (utcInstant <= Start || utcInstant >= End)
            {
                return null;
            }
            return string.Format("{0} {1} {2} {3}\r\n",
                InstantPattern.Format(utcInstant),
                OffsetPattern.Format(offset),
                isStandard ? "standard" : "daylight",
                name);
        }

        private static string FindIdPrefix(List<string> allIds)
        {
            var firstId = allIds[0];
            int slashIndex = -1;
            var previousPrefix = "";
            while (true)
            {
                slashIndex = firstId.IndexOf('/', slashIndex + 1);
                if (slashIndex == -1)
                {
                    throw new Exception("Unable to find a common prefix");
                }
                string prefix = firstId.Substring(0, slashIndex + 1);
                if (allIds.Any(id => !id.StartsWith(prefix)))
                {
                    return previousPrefix;
                }
                // Okay, that prefix worked - lets try moving forward another slash...
                previousPrefix = prefix;
            }
        }
    }
}
