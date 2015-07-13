// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace MungeZdump
{

    /// <summary>
    /// Application to run zdump on each of the zones listed in a NodaTime nzd file,
    /// </summary>
    class Program
    {
        private static readonly Instant Start = Instant.FromUtc(1905, 1, 1, 0, 0);
        private static readonly Instant End = Instant.FromUtc(2035, 1, 1, 0, 0);
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.GeneralPattern;
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");
        private static readonly IPattern<LocalDateTime> LocalPattern = LocalDateTimePattern.CreateWithInvariantCulture("ddd MMM d HH:mm:ss yyyy");
        // Sample line:
        // Europe/Paris  Sun Oct 27 00:59:59 2019 UTC = Sun Oct 27 02:59:59 2019 CEST isdst=1
        private static readonly Regex LineRegex = new Regex(@"^[^ ]*  (?<utc>.*) UTC = (?<local>.*\d\d\d\d) (?<name>[^ ]*) isdst=(?<isdst>.)$");

        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: NodaDump <source> <path to zdump>");
                Console.WriteLine("The source may be a local nzd file, or a URL, e.g. http://nodatime.org/tzdb/tzdb2015e.nzd");
                return;
            }
            var sourceData = LoadSource(args[0]);
            var source = TzdbDateTimeZoneSource.FromStream(new MemoryStream(sourceData));
            var provider = new DateTimeZoneCache(source);

            // Ids is already sorted
            foreach (var id in provider.Ids)
            {
                DumpZone(id, args[1]);
                Console.Write("\r\n");
            }
        }

        private static void DumpZone(string id, string zdump)
        {
            Console.Write("{0}\r\n", id);
            // There's a before/on pair for each transition. We only want the "on" line.
            // Additionally, skip the first pair which just gives the earliest known transition.
            var lines = RunZdump(id, zdump).Where((value, index) => index % 2 == 1).Skip(1).ToList();
            bool writtenAnything = false;
            foreach (var line in lines)
            {
                var match = LineRegex.Match(line);
                if (!match.Success)
                {
                    throw new Exception("Invalid line: " + line);
                }
                var utc = LocalPattern.Parse(match.Groups["utc"].Value.Replace("  ", " ")).Value;
                var local = LocalPattern.Parse(match.Groups["local"].Value.Replace("  ", " ")).Value;
                var name = match.Groups["name"].Value;
                var isStandard = match.Groups["isdst"].Value == "0";
                var offsetInMillis = Period.Between(utc, local, PeriodUnits.Milliseconds).Milliseconds;
                var offset = Offset.FromMilliseconds((int) offsetInMillis);

                var utcInstant = utc.InUtc().ToInstant();
                if (utcInstant <= Start)
                {
                    continue;
                }
                if (utcInstant >= End)
                {
                    if (!writtenAnything)
                    {
                        Console.Write("Fixed: {0} {1}\r\n",
                            OffsetPattern.Format(offset),
                            name);
                        return;
                    }
                    return;
                }

                writtenAnything = true;
                Console.Write("{0} {1} {2} {3}\r\n",
                    InstantPattern.Format(utcInstant),
                    OffsetPattern.Format(offset),
                    isStandard ? "standard" : "daylight",
                    name);
            }
        }

        private static List<string> RunZdump(string id, string zdump)
        {
            var info = new ProcessStartInfo
            {
                FileName = zdump,
                Arguments = "-v " + id,
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
