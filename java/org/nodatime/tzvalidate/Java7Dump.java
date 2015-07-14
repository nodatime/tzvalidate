// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.text.SimpleDateFormat;
import java.time.Instant;
import java.time.ZoneId;
import java.time.ZoneOffset;
import java.time.zone.ZoneOffsetTransition;
import java.util.Arrays;
import java.util.Calendar;
import java.util.Date;
import java.util.GregorianCalendar;
import java.util.Locale;
import java.util.TimeZone;
import java.util.concurrent.TimeUnit;

public final class Java7Dump {
    
    private static final long ONE_DAY_MILLIS = TimeUnit.DAYS.toMillis(1);
    private static final TimeZone UTC = TimeZone.getTimeZone("UTC");
    private static final SimpleDateFormat INSTANT_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
    private static final SimpleDateFormat ZONE_NAME_FORMAT = new SimpleDateFormat("zz", Locale.US);
    private static final long START;
    private static final long END;

    
    static {
        INSTANT_FORMAT.setTimeZone(TimeZone.getTimeZone("UTC"));
        Calendar calendar = GregorianCalendar.getInstance(UTC);
        calendar.set(1905, 0, 1, 0, 0, 0);
        calendar.set(Calendar.MILLISECOND, 0);
        START = calendar.getTimeInMillis();
        calendar.set(2035, 0, 1, 0, 0, 0);
        END = calendar.getTimeInMillis();
    }

    public static void main(String[] args) throws IOException {
        if (args.length > 1) {
            System.out.println("Usage: Java7Dump [zone]");
            return;
        }
        
        if (args.length == 1) {
            dumpZone(args[0]);
        } else {
            String[] ids = TimeZone.getAvailableIDs();
            Arrays.sort(ids);
            for (String id : ids) {
                dumpZone(id);
                System.out.printf("\r\n");
            }
        }
    }

    private static void dumpZone(String id) {
        System.out.printf("%s\r\n", id);
        TimeZone zone = TimeZone.getTimeZone(id);
        Long firstTransition = getNextTransition(START, zone);
        ZONE_NAME_FORMAT.setTimeZone(zone);
        if (firstTransition == null) {
            System.out.printf("Fixed: %s %s\r\n",
                printOffset(zone.getOffset(START)),
                ZONE_NAME_FORMAT.format(START));
            return;
        }
        Long now = firstTransition;
        while (now != null) {
            Date nowDate = new Date(now);
            // TODO: Name. Can't seem to find this...
            System.out.printf("%s %s %s %s\r\n",
                    INSTANT_FORMAT.format(now),
                    printOffset(zone.getOffset(now)),
                    zone.inDaylightTime(nowDate) ? "daylight" : "standard",
                    ZONE_NAME_FORMAT.format(nowDate));

            now = getNextTransition(now, zone);
        }
    }
    
    /**
     * Work out the next transition from the given starting point, in the given time zone.
     * This checks each day until END, and assumes there is no more than one transition per day.
     * Unfortunately, java.util.TimeZone doesn't provide a cleaner approach than this :(
     */
    private static Long getNextTransition(long start, TimeZone zone) {
        long startOffset = zone.getOffset(start);
        boolean startDaylight = zone.inDaylightTime(new Date(start));
        
        long now = start + ONE_DAY_MILLIS;
        Date nowDate = new Date();
        
        while (now < END) {
            nowDate.setTime(now);
            if (zone.getOffset(now) != startOffset || zone.inDaylightTime(nowDate) != startDaylight) {
                return now;
            }
            now += ONE_DAY_MILLIS;
        }
        return null;
    }

    private static String printOffset(int offsetMilliseconds) {
        long seconds = offsetMilliseconds / 1000;
        String sign = seconds < 0 ? "-" : "+";
        if (seconds < 0) {
            seconds = -seconds;
        }
        return String.format("%s%02d:%02d:%02d", sign, seconds / 3600,
                (seconds / 60) % 60, seconds % 60);
    }
}
