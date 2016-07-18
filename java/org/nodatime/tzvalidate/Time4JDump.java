// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.util.Date;
import java.util.Locale;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

import org.apache.commons.cli.ParseException;

import net.time4j.Moment;
import net.time4j.PlainTimestamp;
import net.time4j.format.expert.ChronoFormatter;
import net.time4j.scale.TimeScale;
import net.time4j.tz.TZID;
import net.time4j.tz.Timezone;
import net.time4j.tz.TransitionHistory;
import net.time4j.tz.ZonalTransition;

public final class Time4JDump implements ZoneTransitionsProvider {
    private static final ChronoFormatter<Moment> ZONE_NAME_FORMATTER =
        ChronoFormatter.setUp(Moment.axis(), Locale.ROOT).addLiteral(' ').addShortTimezoneName().build();

    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new Time4JDump(), false, args);
    }
    
    @Override
    public ZoneTransitions getTransitions(String id, int fromYear, int toYear) {
        ZoneTransitions transitions = new ZoneTransitions(id);
        Timezone tz = Timezone.of(id);
        ChronoFormatter<Moment> formatter = ZONE_NAME_FORMATTER.with(tz);
        TransitionHistory history = tz.getHistory();
        Moment start = PlainTimestamp.of(fromYear, 1, 1, 0, 0, 0).atUTC();
        Moment end = PlainTimestamp.of(toYear, 1, 1, 0, 0, 0).atUTC();
        Moment initialMoment = start.minus(1, TimeUnit.NANOSECONDS);
        transitions.addTransition(null,
            tz.getOffset(initialMoment).getIntegralAmount() * 1000L,
            tz.isDaylightSaving(initialMoment),
            formatter.format(initialMoment));

        for (ZonalTransition transition : history.getTransitions(start, end)) {
            Moment transitionTime = Moment.of(transition.getPosixTime(), TimeScale.POSIX);
            transitions.addTransition(
                new Date(transitionTime.getPosixTime() * 1000L),
                transition.getTotalOffset() * 1000L,
                transition.isDaylightSaving(),
                formatter.format(transitionTime));
        }
        return transitions;
    }

    @Override
    public void initialize(DumpOptions options) {
        // configuring member "printingNames" based on dump options ?
    }

    @Override
    public Iterable<String> getZoneIds() {
        return Timezone.getAvailableIDs().stream().map(TZID::canonical).collect(Collectors.toList());
    }
}
