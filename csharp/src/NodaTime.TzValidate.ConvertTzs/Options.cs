// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.TzValidate.ConvertTzs
{
    /// <summary>
    /// Options for ZicDump, typically specified on the command line.
    /// </summary>
    internal sealed class Options
    {
        [Option("f", "from-year", Required = false, HelpText = "Lower bound (inclusive) to print transitions from.")]
        public int? FromYear { get; set; }

        [Option("t", "to-year", Required = false,
            HelpText = "Upper bound (exclusive) to print transitions until (defaults to 2035)", DefaultValue = 2035)]
        public int ToYear { get; set; } = 2050;

        [Option("s", "source", Required = true, HelpText = "Data source - a TZS file, or an IANA distribution")]
        public string Source { get; set; }

        [Option("o", "output", Required = false, HelpText = "Output file (defaults to writing to the console")]
        public string OutputFile { get; set; }

        [Option("z", "zone", Required = false, HelpText = "Zone ID, to dump a single time zone")]
        public string ZoneId { get; set; }

        [Option("v", "version", Required = false, HelpText = "Data version, if known (for standalone file)")]
        public string Version { get; set; }

        [Option(null, "hash", Required = false, HelpText = "Only output the SHA-256 hash")]
        public bool HashOnly { get; set; }

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("NodaTime.TzValidate.ConvertTzs"))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("Jon Skeet", 2016)
            };
            help.AddPreOptionsLine("Usage: dotnet run -- -s data-source [-f from-year] [-t to-year] [-i true/false] [-z zone id] [-v data version]");
            help.AddOptions(this);
            return help;
        }
    }
}
