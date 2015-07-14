// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.IOException;
import java.io.PrintStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;
import java.util.Map;

import org.joda.time.*;
import org.joda.time.format.*;
import org.joda.time.tz.*;

public final class JodaDump {

    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormat
        .forPattern("yyyy-MM-dd'T'HH:mm:ss'Z'")
        .withZone(DateTimeZone.UTC);
    private static final DateTimeFormatter ZONE_NAME_FORMAT = DateTimeFormat
        .forPattern("zz");
    private static final Instant START = new DateTime(1905, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();
    private static final Instant END = new DateTime(2035, 1, 1, 0, 0, DateTimeZone.UTC).toInstant();

    private static final String[] KNOWN_FILES = { "africa", "antarctica",
        "asia", "australasia", "backward", "etcetera", "europe",
        "northamerica", "pacificnew", "southamerica" };

    public static void main(String[] args) throws IOException {

        if (args.length < 1 || args.length > 2) {
            System.out.println("Usage: JodaDump <directory> [zone]");
            return;
        }
        File directory = new File(args[0]);
        if (!directory.isDirectory()) {
            System.out.println(directory + " is not a directory");
            return;
        }

        File[] files = new File[KNOWN_FILES.length];
        for (int i = 0; i < files.length; i++) {
            files[i] = new File(directory, KNOWN_FILES[i]);
        }

        ZoneInfoCompiler compiler = new ZoneInfoCompiler();
        PrintStream out = System.out;
        // Ignore standard output while compiling...
        System.setOut(new PrintStream(new ByteArrayOutputStream()));
        Map<String, DateTimeZone> zones = compiler.compile(null, files);
        System.setOut(out);

        if (args.length == 2) {
            String id = args[1];
            DateTimeZone zone = zones.get(id);
            if (zone == null) {
                System.out.println("Zone " + id + " does not exist");
            } else {
                dumpZone(id, zone);
            }
        } else {
            // Sort IDs in ordinal fashion
            List<String> ids = new ArrayList<String>(zones.keySet());
            Collections.sort(ids); // compareTo does ordinal comparisons
            for (String id : ids) {
                dumpZone(id, zones.get(id));
                System.out.printf("\r\n");
            }
        }
    }

    private static void dumpZone(String id, DateTimeZone zone) {
        System.out.printf("%s\r\n", id);
        DateTimeFormatter nameFormatter = ZONE_NAME_FORMAT.withZone(zone);
        long now = zone.nextTransition(START.getMillis());
        if (now == START.getMillis() || now >= END.getMillis()) {
            System.out.printf("Fixed: %s %s\r\n",
                printOffset(zone.getOffset(START)),
                zone.getShortName(START.getMillis()));
            return;
        }
        while (now < END.getMillis()) {
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
