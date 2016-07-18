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
import java.util.Date;
import java.util.Locale;

import org.apache.commons.cli.ParseException;

public final class Java8Dump implements ZoneTransitionsProvider {
    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new Java8Dump(), false, args);
    }

    @Override
    public ZoneTransitions getTransitions(String id, int fromYear, int toYear) {
        ZoneId zone = ZoneId.of(id);
        ZoneRules rules = zone.getRules();
        DateTimeFormatter nameFormat = DateTimeFormatter.ofPattern("zzz", Locale.US);
        ZoneTransitions transitions = new ZoneTransitions(id);

        Instant start = ZonedDateTime.of(fromYear, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        transitions.addTransition(null, rules.getOffset(start).getTotalSeconds() * 1000,
            rules.isDaylightSavings(start),
            nameFormat.format(start.atZone(zone)));
        Instant end = ZonedDateTime.of(toYear, 1, 1, 0, 0, 0, 0, ZoneOffset.UTC).toInstant();
        
        ZoneOffsetTransition transition = rules.nextTransition(start.minusNanos(1));
        while (transition != null && transition.getInstant().isBefore(end)) {
            Instant instant = transition.getInstant();
            transitions.addTransition(new Date(instant.toEpochMilli()),
                rules.getOffset(instant).getTotalSeconds() * 1000,
                rules.isDaylightSavings(instant),
                nameFormat.format(instant.atZone(zone)));
            transition = rules.nextTransition(instant);
        }
        return transitions;
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
