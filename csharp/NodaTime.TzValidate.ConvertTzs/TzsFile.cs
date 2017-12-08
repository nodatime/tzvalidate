// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace NodaTime.TzValidate.ConvertTzs
{
    /// <summary>
    /// In-memory representation of a TZS file.
    /// </summary>
    public sealed class TzsFile
    {
        private static Regex TzPattern = new Regex("^TZ=\"([^\"]*)\"$");
        private static Regex InitialTransitionPattern = new Regex(
            @"^-\t-\t(?<offset>[+-]\d{2}(?:\d{2}(?:\d{2})?)?)(?:\t(?<abbr>[^\t]*)(?<isdst>\t1)?)?$");
        private static Regex RegularTransitionPattern = new Regex(
            @"^(?<date>\d{4}-\d{2}-\d{2})\t(?<time>\d{2}(?::\d{2}(?::\d{2})?)?)\t(?<offset>[+-]\d{2}(?:\d{2}(?:\d{2})?)?)(?:\t(?<abbr>[^\t]*)(?<isdst>\t1)?)?$");

        public IReadOnlyDictionary<string, string> Aliases { get; }
        public IEnumerable<TzsZone> Zones { get; }
        public string Version { get; }

        private TzsFile(
            IReadOnlyDictionary<string, string> aliases,
            IEnumerable<TzsZone> zones,
            string version)
        {
            Aliases = aliases;
            Zones = zones;
            Version = version;
        }

        public static TzsFile Parse(TextReader reader, string version)
        {
            Dictionary<string, string> aliases = new Dictionary<string, string>();
            List<TzsZone> zones = new List<TzsZone>();

            string line;
            string currentId = null;
            List<Transition> currentTransitionList = null;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim() == "")
                {
                    continue;
                }
                if (line.StartsWith("Link"))
                {
                    string[] bits = line.Split('\t');
                    aliases.Add(bits[2], bits[1]);
                    continue;
                }
                Match match = TzPattern.Match(line);
                if (match.Success)
                {
                    if (currentId != null)
                    {
                        zones.Add(new TzsZone(currentId, currentTransitionList));
                    }
                    currentId = match.Groups[1].Value;
                    currentTransitionList = new List<Transition>();
                    continue;
                }
                match = InitialTransitionPattern.Match(line);
                if (match.Success)
                {
                    string abbr = match.Groups["abbr"].Value;
                    string offset = match.Groups["offset"].Value;
                    bool isDst = match.Groups["isdst"].Success;
                    abbr = GetAbbreviation(abbr, offset);
                    currentTransitionList.Add(new Transition(null, ParseOffset(offset), isDst, abbr));
                    continue;
                }
                match = RegularTransitionPattern.Match(line);
                if (match.Success)
                {
                    string date = match.Groups["date"].Value;
                    string time = match.Groups["time"].Value;
                    string abbr = match.Groups["abbr"].Value;
                    string offset = match.Groups["offset"].Value;
                    bool isDst = match.Groups["isdst"].Success;
                    abbr = GetAbbreviation(abbr, offset);
                    DateTime local = ParseDateAndTime(date, time);
                    TimeSpan parsedOffset = ParseOffset(offset);
                    // DateTimeOffset can't cope with non-minute offsets :(
                    var utc = DateTime.SpecifyKind(local - parsedOffset, DateTimeKind.Utc);
                    currentTransitionList.Add(new Transition(utc, parsedOffset , isDst, abbr));
                    continue;
                }
                throw new Exception($"Unexpected line in TZS file: \"{line}\"");
            }
            if (currentId != null)
            {
                zones.Add(new TzsZone(currentId, currentTransitionList));
            }
            return new TzsFile(aliases, zones, version);
        }

        private static TimeSpan ParseOffset(string offset)
        {
            bool negative = offset[0] == '-';
            offset = offset.Substring(1);
            if (offset.Length == 2)
            {
                offset += "00";
            }
            if (offset.Length == 4)
            {
                offset += "00";
            }
            TimeSpan parsed = TimeSpan.ParseExact(offset, "hhmmss", CultureInfo.InvariantCulture);
            return negative ? -parsed : parsed;
        }

        private static DateTime ParseDateAndTime(string date, string time)
        {
            if (time.Length == 2)
            {
                time += ":00";
            }
            if (time.Length == 5)
            {
                time += ":00";
            }
            return DateTime.ParseExact($"{date} {time}", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
        }

        private static string GetAbbreviation(string abbreviation, string offset)
        {
            if (!string.IsNullOrEmpty(abbreviation))
            {
                return abbreviation.Trim('"');
            }
            if (!string.IsNullOrEmpty(offset))
            {
                return offset.Trim('"');
            }
            return "-00";
        }        
    }
}
