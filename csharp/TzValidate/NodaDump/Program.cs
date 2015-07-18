// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.IO;
using System.Net;
using NodaTime.Text;
using NodaTime.TimeZones;

namespace NodaTime.TzValidate.NodaDump
{
    /// <summary>
    /// Dumps all the time zones from a particular NZD file in tzvalidate format.
    /// </summary>
    internal static class Program
    {
        private static readonly Instant Start = Instant.FromUtc(1905, 1, 1, 0, 0);
        private static readonly Instant End = Instant.FromUtc(2035, 1, 1, 0, 0);
        private static readonly IPattern<Instant> InstantPattern = NodaTime.Text.InstantPattern.GeneralPattern;
        private static readonly IPattern<Offset> OffsetPattern = NodaTime.Text.OffsetPattern.CreateWithInvariantCulture("l");

        static void Main(string[] args)
        {
            if (args.Length < 1 || args.Length > 2)
            {
                Console.WriteLine("Usage: NodaDump <source> [zone]");
                Console.WriteLine("The source may be a local nzd file, or a URL, e.g. http://nodatime.org/tzdb/tzdb2015e.nzd");
                return;
            }
            var sourceData = LoadSource(args[0]);
            var source = TzdbDateTimeZoneSource.FromStream(new MemoryStream(sourceData));
            var provider = new DateTimeZoneCache(source);

            if (args.Length == 2)
            {
                DumpZone(provider[args[1]]);
            }
            else
            {
                // Ids is already sorted
                foreach (var id in provider.Ids)
                {
                    DumpZone(provider[id]);
                    Console.Write("\r\n");
                }
            }
        }

        private static void DumpZone(DateTimeZone zone)
        {
            Console.Write("{0}\r\n", zone.Id);
            var zoneInterval = zone.GetZoneInterval(Start);
            if (!zoneInterval.HasEnd || zoneInterval.End >= End)
            {
                Console.Write("Fixed: {0} {1}\r\n",
                    OffsetPattern.Format(zoneInterval.WallOffset),
                    zoneInterval.Name);
                return;
            }
            while (zoneInterval.HasEnd && zoneInterval.End < End)
            {
                zoneInterval = zone.GetZoneInterval(zoneInterval.End);
                Console.Write("{0} {1} {2} {3}\r\n",
                    InstantPattern.Format(zoneInterval.Start),
                    OffsetPattern.Format(zoneInterval.WallOffset),
                    zoneInterval.Savings == Offset.Zero ? "standard" : "daylight",
                    zoneInterval.Name);
            }
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
