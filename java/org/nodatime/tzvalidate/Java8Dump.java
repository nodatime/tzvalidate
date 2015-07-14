// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.time.zone.ZoneOffsetTransition;
import java.time.zone.ZoneRules;
import java.time.zone.ZoneRulesProvider;
import java.util.Locale;
import java.util.Set;
import java.util.TreeSet;

public final class Java8Dump {
    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormatter
        .ofPattern("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US)
        .withZone(ZoneOffset.UTC);
    private static final DateTimeFormatter ZONE_NAME_FORMAT = DateTimeFormatter
        .ofPattern("zzz", Locale.US);
    private static final Instant START = ZonedDateTime.of(1905, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
    private static final Instant END = ZonedDateTime.of(2035, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();

    public static void main(String[] args) throws IOException {
        if (args.length > 1) {
            System.out.println("Usage: Java8Dump [zone]");
            return;
        }
        if (args.length == 1) {
            String id = args[0];
            ZoneId zone = ZoneId.of(id);
            if (zone == null) {
                System.out.println("Zone " + id + " does not exist");
            } else {
                dumpZone(zone);
            }
        } else {
            // Sort IDs in ordinal fashion
            Set<String> ids = new TreeSet<>(ZoneRulesProvider.getAvailableZoneIds());
            for (String id : ids) {
                dumpZone(ZoneId.of(id));
                System.out.printf("\r\n");
            }
        }
    }

    private static void dumpZone(ZoneId zone) {
        System.out.printf("%s\r\n", zone.getId());
        ZoneRules rules = zone.getRules();
        ZoneOffsetTransition firstTransition = rules.nextTransition(START);
        if (firstTransition == null || !firstTransition.getInstant().isBefore(END)) {
            System.out.printf("Fixed: %s %s\r\n",
                printOffset(rules.getOffset(START)),
                ZONE_NAME_FORMAT.format(START.atZone(zone)));
            return;
        }
        Instant now = firstTransition.getInstant();
        ZoneOffset offset = firstTransition.getOffsetAfter();
        while (now.isBefore(END)) {
            // TODO: Name. Can't seem to find this...
            System.out.printf("%s %s %s %s\r\n",
                    INSTANT_FORMAT.format(now),
                    printOffset(offset),
                    rules.getDaylightSavings(now).isZero() ? "standard" : "daylight",
                    ZONE_NAME_FORMAT.format(now.atZone(zone)));
            ZoneOffsetTransition transition = rules.nextTransition(now);
            if (transition == null) {
                return;
            }
            now = transition.getInstant();
            offset = transition.getOffsetAfter();
        }
    }

    private static String printOffset(ZoneOffset offset) {
        long seconds = offset.getTotalSeconds();
        String sign = seconds < 0 ? "-" : "+";
        if (seconds < 0) {
            seconds = -seconds;
        }
        return String.format("%s%02d:%02d:%02d", sign, seconds / 3600,
                (seconds / 60) % 60, seconds % 60);
    }
}
