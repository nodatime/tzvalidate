// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import net.time4j.Moment;
import net.time4j.PlainTimestamp;
import net.time4j.format.expert.ChronoFormatter;
import net.time4j.format.expert.PatternType;
import net.time4j.scale.TimeScale;
import net.time4j.tz.TZID;
import net.time4j.tz.Timezone;
import net.time4j.tz.TransitionHistory;
import net.time4j.tz.ZonalOffset;
import net.time4j.tz.ZonalTransition;
import org.apache.commons.cli.ParseException;

import java.io.IOException;
import java.io.Writer;
import java.util.Locale;
import java.util.stream.Collectors;

public final class Time4JDump implements ZoneDumper {
    private static final ChronoFormatter<Moment> MOMENT_FORMATTER =
        ChronoFormatter.ofMomentPattern("uuuu-MM-dd HH:mm:ssXXX", PatternType.CLDR, Locale.ROOT, ZonalOffset.UTC);

    private static final ChronoFormatter<Moment> ZONE_NAME_FORMATTER =
        ChronoFormatter.setUp(Moment.axis(), Locale.ROOT).addLiteral(' ').addShortTimezoneName().build();

    // actually unused because it relies on non-historic JDK-data (originating from CLDR)
    // and not on historic abbreviations contained in TZDB (which are partially invented however)
    private final boolean printingNames = false;

    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new Time4JDump(), false, args);
    }

    public void dumpZone(String id, int fromYear, int toYear, Writer writer) throws IOException {
        writer.write(id + "\n");

        Timezone tz = Timezone.of(id);
        TransitionHistory history = tz.getHistory();

        String name = "";
        if (this.printingNames) {
            name = " -"; // we consider the era before first recorded transition as having no timezone concept at all
        }
        writer.write("Initially:          " + format(history.getInitialOffset()) + " standard" + name + "\n");

        Moment start = PlainTimestamp.of(fromYear, 1, 1, 0, 0, 0).atUTC();
        Moment end = PlainTimestamp.of(toYear, 1, 1, 0, 0, 0).atUTC();

        for (ZonalTransition transition : history.getTransitions(start, end)) {
            Moment transitionTime = Moment.of(transition.getPosixTime(), TimeScale.POSIX);
            ZonalOffset offsetAfter = ZonalOffset.ofTotalSeconds(transition.getTotalOffset());
            String dstInfo = transition.isDaylightSaving() ? " daylight" : " standard";
            if (this.printingNames) {
                name = ZONE_NAME_FORMATTER.with(tz).format(transitionTime);
            }
            writer.write(MOMENT_FORMATTER.format(transitionTime) + format(offsetAfter) + dstInfo + name + "\n");
        }
    }

    @Override
    public void initialize(DumpOptions options) {
        // configuring member "printingNames" based on dump options ?
    }

    @Override
    public Iterable<String> getZoneIds() {
        return Timezone.getAvailableIDs().stream().map(TZID::canonical).collect(Collectors.toList());
    }

    private static String format(ZonalOffset offset) {
        String s = " " + offset.toString();
        if (offset.getAbsoluteSeconds() == 0) {
            s += ":00";
        }
        return s;
    }

}
