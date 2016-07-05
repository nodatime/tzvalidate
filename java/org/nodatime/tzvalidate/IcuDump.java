// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.util.Arrays;
import java.util.Date;
import java.util.Locale;

import org.apache.commons.cli.ParseException;

import com.ibm.icu.text.DateFormat;
import com.ibm.icu.text.SimpleDateFormat;
import com.ibm.icu.util.BasicTimeZone;
import com.ibm.icu.util.Calendar;
import com.ibm.icu.util.GregorianCalendar;
import com.ibm.icu.util.TimeZone;
import com.ibm.icu.util.TimeZoneTransition;

public final class IcuDump implements ZoneTransitionsProvider {
    private static final TimeZone UTC = TimeZone.getTimeZone("UTC");

    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new IcuDump(), false, args);
    }

    @Override
    public ZoneTransitions getTransitions(String id, int fromYear, int toYear) {
        BasicTimeZone zone = (BasicTimeZone) TimeZone.getTimeZone(id);
        // A poor attempt to get the time zone abbreviation. It's
        // locale-sensitive, and then only
        // applies some of the time.
        // See http://stackoverflow.com/questions/31626356
        DateFormat nameFormat = new SimpleDateFormat("zzz", Locale.US);
        nameFormat.setTimeZone(zone);

        Calendar calendar = GregorianCalendar.getInstance(UTC);
        calendar.set(fromYear, 0, 1, 0, 0, 0);
        calendar.set(Calendar.MILLISECOND, 0);
        long start = calendar.getTimeInMillis();
        calendar.set(toYear, 0, 1, 0, 0, 0);
        long end = calendar.getTimeInMillis();
        Date date = new Date(start);

        ZoneTransitions transitions = new ZoneTransitions(id);
        transitions.addTransition(null,
            zone.getOffset(start),
            zone.inDaylightTime(date),
            nameFormat.format(date));

        TimeZoneTransition transition = zone.getNextTransition(start, true /* inclusive */);

        while (transition != null && transition.getTime() < end) {
            long now = transition.getTime();
            date.setTime(now);
            transitions.addTransition(date,
                zone.getOffset(now),
                zone.inDaylightTime(date),
                nameFormat.format(date));
            transition = zone.getNextTransition(now, false /* exclusive */);
        }
        return transitions;
    }

    @Override
    public void initialize(DumpOptions options) {
        // No-op
    }

    @Override
    public Iterable<String> getZoneIds() {
        return Arrays.asList(TimeZone.getAvailableIDs());
    }
}
