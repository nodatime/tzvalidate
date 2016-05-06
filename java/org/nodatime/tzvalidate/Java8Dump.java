// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.io.Writer;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.time.ZonedDateTime;
import java.time.format.DateTimeFormatter;
import java.time.zone.ZoneOffsetTransition;
import java.time.zone.ZoneRules;
import java.util.Locale;

import org.apache.commons.cli.ParseException;

public final class Java8Dump implements ZoneDumper {
    private static final DateTimeFormatter INSTANT_FORMAT = DateTimeFormatter
        .ofPattern("yyyy-MM-dd HH:mm:ss'Z'", Locale.US)
        .withZone(ZoneOffset.UTC);
    private static final DateTimeFormatter ZONE_NAME_FORMAT = DateTimeFormatter.ofPattern("zzz", Locale.US);

    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new Java8Dump(), false, args);
    }

    public void dumpZone(String id, int fromYear, int toYear, Writer writer) throws IOException {
        writer.write(id + "\n");
        ZoneId zone = ZoneId.of(id);
        // Instant.MIN can't be formatted, so let's just do "quite a long time ago"
        Instant early = ZonedDateTime.of(1, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        writer.write("Initially:           " + formatOffsetAndName(zone, early) + "\n");  

        Instant start = ZonedDateTime.of(fromYear, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        Instant end = ZonedDateTime.of(toYear, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        
        ZoneRules rules = zone.getRules();
        ZoneOffsetTransition transition = rules.nextTransition(start.minusNanos(1));
        while (transition != null && transition.getInstant().isBefore(end)) {
            writer.write(INSTANT_FORMAT.format(transition.getInstant()) + " "
                + formatOffsetAndName(zone, transition.getInstant()) + "\n");
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

    @Override
    public void initialize(DumpOptions options) {
        // No-op
    }

    @Override
    public Iterable<String> getZoneIds() {
        return ZoneId.getAvailableZoneIds();
    }
}
