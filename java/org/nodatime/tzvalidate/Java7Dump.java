// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.Locale;
import java.util.TimeZone;
import java.util.concurrent.TimeUnit;

import org.apache.commons.cli.ParseException;

public final class Java7Dump implements ZoneTransitionsProvider {
    
    private static final long ONE_DAY_MILLIS = TimeUnit.DAYS.toMillis(1);
    private static final TimeZone UTC = TimeZone.getTimeZone("UTC");    

    public static void main(String[] args) throws IOException, ParseException {
        DumpCoordinator.dump(new Java7Dump(), false,  args);
    }

    @Override
    public ZoneTransitions getTransitions(String id, int fromYear, int toYear) {
        TimeZone zone = TimeZone.getTimeZone(id);
        DateFormat nameFormat = new SimpleDateFormat("zzz", Locale.US);
        nameFormat.setTimeZone(zone);

        // Given the way we find transitions, we really don't want to go
        // from any earlier than this...
        if (fromYear < 1800) {
            fromYear = 1800;
        }

        Calendar calendar = GregorianCalendar.getInstance(UTC);
        calendar.set(fromYear, 0, 1, 0, 0, 0);
        calendar.set(Calendar.MILLISECOND, 0);
        long start = calendar.getTimeInMillis();
        calendar.set(toYear, 0, 1, 0, 0, 0);
        long end = calendar.getTimeInMillis();
        calendar.set(1, 0, 1, 0, 0, 0);
        long early = calendar.getTimeInMillis();
        Date date = new Date(early);
        
        ZoneTransitions transitions = new ZoneTransitions(id);
        transitions.addTransition(null,
            zone.getOffset(early),
            zone.inDaylightTime(date),
            nameFormat.format(date));
        
        Long transition = getNextTransition(zone, start - 1, end);
        while (transition != null) {
            date.setTime(transition);
            transitions.addTransition(date,
                zone.getOffset(transition),
                zone.inDaylightTime(date),
                nameFormat.format(date));
            transition = getNextTransition(zone, transition, end);
        }
        return transitions;
    }
    
    /**
     * Work out the next transition from the given starting point, in the given time zone.
     * This checks each day until END, and assumes there is no more than one transition per day.
     * Once it's found a day containing a transition, it performs a binary search to find the actual
     * time of the transition.
     * Unfortunately, java.util.TimeZone doesn't provide a cleaner approach than this :(
     */
    private static Long getNextTransition(TimeZone zone, long start, long end) {
        long startOffset = zone.getOffset(start);
        boolean startDaylight = zone.inDaylightTime(new Date(start));
        
        long now = start + ONE_DAY_MILLIS;
        Date nowDate = new Date();
        
        // Inclusive upper bound to enable us to find transitions within the last day.
        while (now <= end) {
            nowDate.setTime(now);
            if (zone.getOffset(now) != startOffset || zone.inDaylightTime(nowDate) != startDaylight) {
                // Right, so there's a transition strictly after now - ONE_DAY_MILLIS, and less than or equal to now. Binary search...
                long upperInclusive = now;
                long lowerExclusive = now - ONE_DAY_MILLIS;
                while (upperInclusive > lowerExclusive + 1) {
                    // The values will never be large enough for this addition to be a problem.
                    long candidate = (upperInclusive + lowerExclusive) / 2;
                    nowDate.setTime(candidate);
                    if (zone.getOffset(candidate) != startOffset || zone.inDaylightTime(nowDate) != startDaylight) {
                        // Still seeing a difference: look earlier
                        upperInclusive = candidate;
                    } else {
                        // Same as at start of day: look later
                        lowerExclusive = candidate;
                    }
                }
                // If we turn out to have hit the end point, we're done without a final transition. 
                return upperInclusive == end ? null : upperInclusive;
            }
            now += ONE_DAY_MILLIS;
        }
        return null;
    }

    @Override
    public void initialize(DumpOptions options) {
        // No-op
    }

    @Override
    public Iterable<String> getZoneIds() {
    	// TODO: Remove the abbreviation IDs which aren't actually in TZDB.
        return Arrays.asList(TimeZone.getAvailableIDs());
    }
}
