// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.TzValidate.TimeZoneInfoDump
{
    /// <summary>
    /// Options for TimeZoneInfoDump, typically specified on the command line.
    /// </summary>
    internal sealed class Options
    {
        [Option("f", "from-year", Required = false, HelpText = "Lower bound (inclusive) to print transitions from.")]
        public int? FromYear { get; set; }

        [Option("t", "to-year", Required = false,
            HelpText = "Upper bound (exclusive) to print transitions until (defaults to 2035)", DefaultValue = 2035)]
        public int ToYear { get; set; } = 2050;

        [Option("z", "zone", Required = false, HelpText = "Zone ID, to dump a single time zone")]
        public string ZoneId { get; set; }
        
        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("ZicDump"))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("Jon Skeet", 2015)
            };
            help.AddPreOptionsLine("Usage: dnx run [-f from-year] [-t to-year] [-z zone id]");
            help.AddOptions(this);
            return help;
        }

    }
}
