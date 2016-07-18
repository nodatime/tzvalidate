// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.util.Arrays;
import java.util.Date;
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

public final class JodaDump implements ZoneTransitionsProvider {

    private Map<String, DateTimeZone> zones;

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

    @Override
    public ZoneTransitions getTransitions(String id, int fromYear, int toYear) {
        DateTimeZone zone = zones.get(id);
        // Note that the ID we fetched isn't always the same as DateTimeZone.getID()
        DateTimeFormatter nameFormatter = DateTimeFormat
            .forPattern("zz").withZone(zone);
                        
        Instant start = new DateTime(fromYear, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
        Instant end = new DateTime(toYear, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
        ZoneTransitions transitions = new ZoneTransitions(id);
        long now = start.getMillis();
        transitions.addTransition(null,
            zone.getOffset(now),
            !zone.isStandardOffset(now),
            nameFormatter.print(now));

        now = zone.nextTransition(now);
        if (now == start.getMillis()) {
            return transitions;
        }
        while (now < end.getMillis()) {
            transitions.addTransition(new Date(now),
                zone.getOffset(now),
                !zone.isStandardOffset(now),
                nameFormatter.print(now));                              
            long next = zone.nextTransition(now);
            if (next <= now) {
                break;
            }
            now = next;
        }
        return transitions;
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
