// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace NodaTime.TzValidate.MungeZdump
{
    /// <summary>
    /// Application to run zdump on each of the zones listed in a NodaTime nzd file,
    /// and convert the output to the tzvalidate format.
    /// </summary>
    class Program
    {
        private static readonly Instant Start = Instant.FromUtc(1905, 1, 1, 0, 0);
        private static readonly Instant End = Instant.FromUtc(2035, 1, 1, 0, 0);
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.GeneralPattern;
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");
        private static readonly IPattern<LocalDateTime> LocalPattern = LocalDateTimePattern.CreateWithInvariantCulture("ddd MMM d HH:mm:ss yyyy");
        // Sample line:
        // Europe/Paris  Sun Oct 27 00:59:59 2019 UT = Sun Oct 27 02:59:59 2019 CEST isdst=1 gmtoff=7200
        private static readonly Regex LineRegex = new Regex(@"^[^ ]*  (?<utc>.*) UT = (?<local>.*\d\d\d\d) (?<name>[^ ]*) isdst=(?<isdst>.) gmtoff=(?<gmtoff>[-\d]*)$");

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: NodaDump <source> <path to zdump or text file>");
                Console.WriteLine("The source may be a local nzd file, or a URL, e.g. http://nodatime.org/tzdb/tzdb2015e.nzd");
                Console.WriteLine("If the path ends with .txt, it is assumed to be a text file with the output of zdump, for testing");
                return;
            }
            var sourceData = LoadSource(args[0]);
            var source = TzdbDateTimeZoneSource.FromStream(new MemoryStream(sourceData));
            var provider = new DateTimeZoneCache(source);

            if (args[1].EndsWith(".txt"))
            {
                DumpZone("Loaded from text file", File.ReadAllLines(args[1]));
            }
            else
            {
                // Ids is already sorted
                foreach (var id in provider.Ids)
                {
                    var lines = RunZdump(id, args[1]);
                    DumpZone(id, lines);
                    Console.Write("\r\n");
                }
            }
        }

        private static void DumpZone(string id, IEnumerable<string> lines)
        {
            Console.Write("{0}\r\n", id);
            // There's a before/on pair for each transition. We only want the "on" line.
            // Additionally, skip the first pair which just gives the earliest known transition.
            var lineList = lines.Where((value, index) => index % 2 == 1).Skip(1).ToList();
            // Oh, and get rid of the final line, too... which just gives the latest known transition (or
            // possibly the latest supported time).
            lineList.RemoveAt(lineList.Count - 1);
            bool writtenAnything = false;

            // Keep the offset and name outside the scope of the loop, so that we can use the last
            // one we saw for a fixed zone.
            var offset = Offset.Zero;
            string name = null;
            foreach (var line in lineList)
            {
                var match = LineRegex.Match(line);
                if (!match.Success)
                {
                    throw new Exception("Invalid line: " + line);
                }
                var utc = LocalPattern.Parse(match.Groups["utc"].Value.Replace("  ", " ")).Value;
                var local = LocalPattern.Parse(match.Groups["local"].Value.Replace("  ", " ")).Value;
                name = match.Groups["name"].Value;
                var isStandard = match.Groups["isdst"].Value == "0";
                var offsetInMillis = Period.Between(utc, local, PeriodUnits.Milliseconds).Milliseconds;
                offset = Offset.FromMilliseconds((int) offsetInMillis);
                var gmtoff = int.Parse(match.Groups["gmtoff"].Value);
                if (gmtoff != offsetInMillis / 1000)
                {
                    throw new Exception("Inconsistent offsets in line: " + line);
                }

                var utcInstant = utc.InUtc().ToInstant();
                if (utcInstant <= Start || utcInstant >= End)
                {
                    // We've still saved the offset/name, which will be written at the end.
                    continue;
                }

                writtenAnything = true;
                Console.Write("{0} {1} {2} {3}\r\n",
                    InstantPattern.Format(utcInstant),
                    OffsetPattern.Format(offset),
                    isStandard ? "standard" : "daylight",
                    name);
            }
            if (!writtenAnything)
            {
                if (name != null)
                {
                    Console.Write("Fixed: {0} {1}\r\n",
                        OffsetPattern.Format(offset),
                        name);
                }
                else
                {
                    // Hmm. Must be a very fixed zone, like "EST". No lines at all...
                    // We could potentially rerun zdump at this point, without -v,
                    // and work out the difference between UTC and local time. But icky...
                    Console.Write("Fixed: Unknown offset");
                }
            }
        }

        private static IEnumerable<string> RunZdump(string id, string zdump)
        {
            var info = new ProcessStartInfo
            {
                FileName = zdump,
                // Deliberately to the start of time, so that for fixed zones we still
                // see a transition in most cases.
                Arguments = "-v -c 2036 " + id,
                RedirectStandardOutput = true,
                UseShellExecute = false
            };
            var process = Process.Start(info);
            return process.StandardOutput.ReadToEnd().Split('\n').ToList();
        }

        private static byte[] LoadSource(string source)
        {
            if (source.StartsWith("http://") || source.StartsWith("https://"))
            {
                using (var client = new WebClient())
                {
                    return client.DownloadData(source);
                }
            }
            else
            {
                return File.ReadAllBytes(source);
            }
        }
    }
}
