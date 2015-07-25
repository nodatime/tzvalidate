// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.Locale;
import java.util.TimeZone;
import java.util.concurrent.TimeUnit;

import org.apache.commons.cli.ParseException;

public final class Java7Dump {
    
    private static final long ONE_DAY_MILLIS = TimeUnit.DAYS.toMillis(1);
    private static final TimeZone UTC = TimeZone.getTimeZone("UTC");
    private static final SimpleDateFormat INSTANT_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
    private static final SimpleDateFormat ZONE_NAME_FORMAT = new SimpleDateFormat("zz", Locale.US);
    
    static {
        INSTANT_FORMAT.setTimeZone(UTC);
    }

    public static void main(String[] args) throws IOException, ParseException {
        DumpOptions options = DumpOptions.parse("Java7Dump", args, false);
        if (options == null) {
            return;
        }
        
        if (options.getZoneId() != null) {
            dumpZone(options.getZoneId(), options);
        } else {
            String[] ids = TimeZone.getAvailableIDs();
            Arrays.sort(ids);
            for (String id : ids) {
                dumpZone(id, options);
                System.out.printf("\r\n");
            }
        }
    }

    private static void dumpZone(String id, DumpOptions options) {
        System.out.printf("%s\r\n", id);
        
        int fromYear = options.getFromYear();
        // Given the way we find transitions, we really don't want to go
        // from any earlier than this...
        if (fromYear < 1800) {
            fromYear = 1800;
        }

        Calendar calendar = GregorianCalendar.getInstance(UTC);
        calendar.set(fromYear, 0, 1, 0, 0, 0);
        calendar.set(Calendar.MILLISECOND, 0);
        long start = calendar.getTimeInMillis();
        calendar.set(options.getToYear(), 0, 1, 0, 0, 0);
        long end = calendar.getTimeInMillis();
        calendar.set(1, 0, 1, 0, 0, 0);
        long early = calendar.getTimeInMillis();
        
        TimeZone zone = TimeZone.getTimeZone(id);
        ZONE_NAME_FORMAT.setTimeZone(zone);
        System.out.printf("Initially:           %s\r\n", formatOffsetAndName(zone, early));  
        
        Long transition = getNextTransition(zone, start - 1, end);
        Date date = new Date(); // Reused in the loop
        while (transition != null) {
            date.setTime(transition);
            System.out.printf("%s %s\r\n",
                INSTANT_FORMAT.format(date),
                formatOffsetAndName(zone, transition));
            transition = getNextTransition(zone, transition, end);
        }
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
        
        while (now < end) {
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
                return upperInclusive;
            }
            now += ONE_DAY_MILLIS;
        }
        return null;
    }

    private static String formatOffsetAndName(TimeZone zone, long instant) {
        Date date = new Date(instant);
        int offsetMilliseconds = zone.getOffset(instant);
        long seconds = offsetMilliseconds / 1000;
        String sign = seconds < 0 ? "-" : "+";
        if (seconds < 0) {
            seconds = -seconds;
        }
        return String.format("%s%02d:%02d:%02d %s %s",
            sign, seconds / 3600, (seconds / 60) % 60, seconds % 60,
            zone.inDaylightTime(date) ? "daylight" : "standard",
            ZONE_NAME_FORMAT.format(date));
    }
}
