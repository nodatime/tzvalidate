// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.io.Writer;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import org.apache.commons.cli.ParseException;
import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.Instant;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import org.joda.time.tz.ZoneInfoCompiler;

public final class JodaDump implements ZoneDumper {

    private Map<String, DateTimeZone> zones;
    
    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormat
        .forPattern("yyyy-MM-dd HH:mm:ss'Z'")
        .withZone(DateTimeZone.UTC);
    private static final DateTimeFormatter ZONE_NAME_FORMAT = DateTimeFormat
        .forPattern("zz");

    private static final List<String> KNOWN_FILES = Arrays.asList(
        "africa", "antarctica",
        "asia", "australasia", "backward", "etcetera", "europe",
        "northamerica", "pacificnew", "southamerica");

    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new JodaDump(), true, args);
    }
    
    private static Map<String, DateTimeZone> getZones(String source) throws IOException {
        if (source == null) {
            return DateTimeZone.getAvailableIDs()
                .stream()
                .collect(Collectors.toMap(id -> id, DateTimeZone::forID));
        }

        File directory = new File(source);
        if (!directory.isDirectory()) {
            throw new IOException(directory + " is not a directory");
        }
        File[] files = KNOWN_FILES.stream()
            .map(f -> new File(directory, f))
            .toArray(File[]::new);
        ZoneInfoCompiler compiler = new ZoneInfoCompiler();
        PrintStream out = System.out;
        // Ignore standard output while compiling...
        System.setOut(new PrintStream(new ByteArrayOutputStream()));
        Map<String, DateTimeZone> zones = compiler.compile(null, files);
        System.setOut(out);
        return zones;
    }

    public void dumpZone(String id, int fromYear, int toYear, Writer writer) throws IOException {
        writer.write(id + "\n");
        DateTimeZone zone = zones.get(id);
        // Note that the ID we fetched isn't always the same as DateTimeZone.getID()
        DateTimeFormatter nameFormatter = ZONE_NAME_FORMAT.withZone(zone);
        
        String line = String.format("Initially:           %s %s %s\n",
            printOffset(zone.getOffset(Long.MIN_VALUE)),
            zone.isStandardOffset(Long.MIN_VALUE) ? "standard" : "daylight",
            zone.getShortName(Long.MIN_VALUE));
        writer.write(line);
                
        Instant start = new DateTime(fromYear, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
        Instant end = new DateTime(toYear, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();

        long now = zone.nextTransition(start.getMillis());
        if (now == start.getMillis()) {
            return;
        }
        while (now < end.getMillis()) {
            int standardOffset = zone.getStandardOffset(now);
            int wallOffset = zone.getOffset(now);
            line = String.format("%s %s %s %s\n",
                              INSTANT_FORMAT.print(now),
                              printOffset(wallOffset),
                              standardOffset == wallOffset ? "standard" : "daylight",
                              nameFormatter.print(now));
            writer.write(line);
                              
            long next = zone.nextTransition(now);
            if (next <= now) {
                break;
            }
            now = next;
        }
    }

    private static String printOffset(long millis) {
        long seconds = Math.abs(millis) / 1000;
        if (seconds < 0) {
            seconds = -seconds;
        }
        String sign = millis < 0 ? "-" : "+";
        return String.format("%s%02d:%02d:%02d", sign, seconds / 3600,
                (seconds / 60) % 60, seconds % 60);
    }

    @Override
    public void initialize(DumpOptions options) throws IOException {
        zones = getZones(options.getSource());
    }

    @Override
    public Iterable<String> getZoneIds() {
        return zones.keySet();
    }
}
