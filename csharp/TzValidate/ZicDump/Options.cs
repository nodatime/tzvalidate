// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using CommandLine;
using CommandLine.Text;

namespace NodaTime.TzValidate.ZicDump
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

        [Option("s", "source", Required = true, HelpText = "Data source - a single file, or a directory")]
        public string Source { get; set; }

        [Option("z", "zone", Required = false, HelpText = "Zone ID, to dump a single time zone")]
        public string ZoneId { get; set; }

        // The default bool handling in CommandLine is somewhat broken.
        // Fake it with an enum...
        public enum BoolValue
        {
            False, True
        }

        [Option("i", "fake-initial", Required = false, DefaultValue = BoolValue.True,
            HelpText = "Fake initial offset for zones without a big bang transition (true or false; defaults to true)")]
        public BoolValue FakeInitialTransitionText { get; set; }

        public bool FakeInitialTransition => FakeInitialTransitionText == BoolValue.True;

        [HelpOption(HelpText = "Display this help screen.")]
        public string GetUsage()
        {
            var help = new HelpText(new HeadingInfo("ZicDump"))
            {
                AdditionalNewLineAfterOption = true,
                Copyright = new CopyrightInfo("Jon Skeet", 2015)
            };
            help.AddPreOptionsLine("Usage: ZicDump -s data-source [-f from-year] [-t to-year] [-i true/false] [-z zone id]");
            help.AddOptions(this);
            return help;
        }

    }
}
