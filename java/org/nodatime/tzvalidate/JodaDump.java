// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.Collections;
import java.util.List;
import java.util.Map;
import java.util.stream.Collectors;

import org.apache.commons.cli.ParseException;
import org.joda.time.*;
import org.joda.time.format.*;
import org.joda.time.tz.*;

public final class JodaDump {

    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormat
        .forPattern("yyyy-MM-dd'T'HH:mm:ss'Z'")
        .withZone(DateTimeZone.UTC);
    private static final DateTimeFormatter ZONE_NAME_FORMAT = DateTimeFormat
        .forPattern("zz");

    private static final List<String> KNOWN_FILES = Arrays.asList(
        "africa", "antarctica",
        "asia", "australasia", "backward", "etcetera", "europe",
        "northamerica", "pacificnew", "southamerica");

    public static void main(String[] args) throws IOException, ParseException {
        DumpOptions options = DumpOptions.parse("JodaDump", args, true);
        if (options == null) {
            return;
        }
        
        Map<String, DateTimeZone> zones = getZones(options.getSource());

        String zoneId = options.getZoneId();
        if (zoneId != null) {
            DateTimeZone zone = zones.get(zoneId);
            if (zone == null) {
                System.out.println("Zone " + zoneId + " does not exist");
            } else {
                dumpZone(zoneId, zone, options);
            }
        } else {
            // Sort IDs in ordinal fashion
            List<String> ids = new ArrayList<String>(zones.keySet());
            Collections.sort(ids); // compareTo does ordinal comparisons
            for (String id : ids) {
                dumpZone(id, zones.get(id), options);
                System.out.printf("\r\n");
            }
        }
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

    private static void dumpZone(String id, DateTimeZone zone, DumpOptions options) {
        // Note that the ID we fetched isn't always the same as DateTimeZone.getID()
        System.out.printf("%s\r\n", id);
        DateTimeFormatter nameFormatter = ZONE_NAME_FORMAT.withZone(zone);
        System.out.printf("Initially: %s %s %s\r\n",
            printOffset(zone.getOffset(Long.MIN_VALUE)),
            zone.isStandardOffset(Long.MIN_VALUE) ? "standard" : "daylight",
            zone.getShortName(Long.MIN_VALUE));
                
        Instant start = new DateTime(options.getFromYear(), 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
        Instant end = new DateTime(options.getToYear(), 1, 1, 0, 0, DateTimeZone.UTC).toInstant();

        long now = zone.nextTransition(start.getMillis());
        if (now == start.getMillis()) {
            return;
        }
        while (now < end.getMillis()) {
            int standardOffset = zone.getStandardOffset(now);
            int wallOffset = zone.getOffset(now);
            System.out.printf("%s %s %s %s\r\n",
                              INSTANT_FORMAT.print(now),
                              printOffset(wallOffset),
                              standardOffset == wallOffset ? "standard" : "daylight",
                              nameFormatter.print(now));
                              
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
}
