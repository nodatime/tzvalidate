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

import org.apache.commons.cli.ParseException;

public final class Java8Dump {
    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormatter
        .ofPattern("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US)
        .withZone(ZoneOffset.UTC);
    private static final DateTimeFormatter ZONE_NAME_FORMAT = DateTimeFormatter.ofPattern("zzz", Locale.US);

    public static void main(String[] args) throws IOException, ParseException {
        DumpOptions options = DumpOptions.parse("Java8Dump", args, true);
        if (options == null) {
            return;
        }
        
        String zoneId = options.getZoneId();
        if (zoneId != null) {
            ZoneId zone = ZoneId.of(zoneId);
            if (zone == null) {
                System.out.println("Zone " + zoneId + " does not exist");
            } else {
                dumpZone(zone, options);
            }
        } else {
            // Sort IDs in ordinal fashion
            Set<String> ids = new TreeSet<>(ZoneRulesProvider.getAvailableZoneIds());
            for (String id : ids) {
                dumpZone(ZoneId.of(id), options);
                System.out.printf("\r\n");
            }
        }
    }

    private static void dumpZone(ZoneId zone, DumpOptions options) {
        System.out.printf("%s\r\n", zone.getId());
        // Instant.MIN can't be formatted, so let's just do "quite a long time ago"
        Instant early = ZonedDateTime.of(1, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        System.out.printf("Initially: %s\r\n", formatOffsetAndName(zone, early));

        Instant start = ZonedDateTime.of(options.getFromYear(), 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        Instant end = ZonedDateTime.of(options.getToYear(), 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        
        ZoneRules rules = zone.getRules();
        ZoneOffsetTransition transition = rules.nextTransition(start.minusNanos(1));
        while (transition != null && transition.getInstant().isBefore(end)) {
            System.out.printf("%s %s\r\n",
                    INSTANT_FORMAT.format(transition.getInstant()),
                    formatOffsetAndName(zone, transition.getInstant()));
            transition = rules.nextTransition(transition.getInstant());
        }
    }
    
    private static String formatOffsetAndName(ZoneId zone, Instant instant) {
        ZoneRules rules = zone.getRules();
        ZoneOffset offset = rules.getOffset(instant);
        long seconds = offset.getTotalSeconds();
        String sign = seconds < 0 ? "-" : "+";
        if (seconds < 0) {
            seconds = -seconds;
        }
        return String.format("%s%02d:%02d:%02d %s %s",
            sign, seconds / 3600, (seconds / 60) % 60, seconds % 60,
            rules.getDaylightSavings(instant).isZero() ? "standard" : "daylight",
            ZONE_NAME_FORMAT.format(instant.atZone(zone)));
    }
}
