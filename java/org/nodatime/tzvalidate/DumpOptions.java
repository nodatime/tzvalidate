// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.
package org.nodatime.tzvalidate;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.CommandLineParser;
import org.apache.commons.cli.DefaultParser;
import org.apache.commons.cli.HelpFormatter;
import org.apache.commons.cli.Options;
import org.apache.commons.cli.ParseException;

/**
 * The options for a tzvalidate dump program.
 */
public final class DumpOptions {

    private final String zoneId;
    private final int fromYear;
    private final int toYear;
    private final String source;
    private final String version;
    private final boolean includeAbbreviations;

    private DumpOptions(String zoneId, int fromYear, int toYear, String source,
        String version, boolean includeAbbreviations) {
        this.zoneId = zoneId;
        this.fromYear = fromYear;
        this.toYear = toYear;
        this.source = source;
        this.version = version;
        this.includeAbbreviations = includeAbbreviations;
    }

    public String getZoneId() {
        return zoneId;
    }

    public int getFromYear() {
        return fromYear;
    }

    public int getToYear() {
        return toYear;
    }

    public String getSource() {
        return source;
    }

    public String getVersion() {
        return version;
    }
    
    public boolean includeAbbreviations() {
    	return includeAbbreviations;
    }

    public static DumpOptions parse(String name, String[] args,
        boolean includeSourceOption) throws ParseException {
        Options options = new Options();
        options.addOption("f", "from-year", true,
            "Lower bound (inclusive) to print transitions from.");
        options.addOption("t", "to-year", true,
            "Upper bound (inclusive) to print transitions from.");
        options.addOption("v", "version", true,
            "Version to report in output headers.");
        options.addOption("z", "zone", true, "Single zone ID to dump.");
        options.addOption("noabbr", "Suppress abbreviations in output");
        options.addOption("?", "h", false, "Print help.");
        if (includeSourceOption) {
            options.addOption("s", "source", true, "Source of time zone data");
        }
        CommandLineParser parser = new DefaultParser();
        CommandLine cmd = parser.parse(options, args);
        if (cmd.hasOption("?")) {
            HelpFormatter formatter = new HelpFormatter();
            formatter.printHelp(name, options);
            return null;
        }
        return new DumpOptions(cmd.getOptionValue("z"),
            cmd.hasOption("f") ? Integer.parseInt(cmd.getOptionValue("f")) : 1,
            cmd.hasOption("t") ? Integer.parseInt(cmd.getOptionValue("t")) : 2035,
            cmd.getOptionValue("s"),
            cmd.getOptionValue("v"),
            // TODO: Parse true/false values
            !cmd.hasOption("noabbr"));
    }
}
