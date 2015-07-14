// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.util.Arrays;
import java.util.Date;
import java.util.Locale;

import com.ibm.icu.text.SimpleDateFormat;
import com.ibm.icu.util.BasicTimeZone;
import com.ibm.icu.util.Calendar;
import com.ibm.icu.util.GregorianCalendar;
import com.ibm.icu.util.TimeZone;
import com.ibm.icu.util.TimeZoneTransition;

public final class IcuDump {
    private static final TimeZone UTC = TimeZone.getTimeZone("UTC");
    private static final SimpleDateFormat INSTANT_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
    private static final SimpleDateFormat ZONE_NAME_FORMAT = new SimpleDateFormat("zzz", Locale.US);
    private static final long START;
    private static final long END;
    
    static {
        INSTANT_FORMAT.setTimeZone(UTC);
        Calendar calendar = new GregorianCalendar(UTC);
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
        BasicTimeZone zone = (BasicTimeZone) TimeZone.getTimeZone(id);
        TimeZoneTransition transition = zone.getNextTransition(START, false /* not inclusive */);
        ZONE_NAME_FORMAT.setTimeZone(zone);
        if (transition == null) {
            System.out.printf("Fixed: %s %s\r\n",
                printOffset(zone.getOffset(START)),
                ZONE_NAME_FORMAT.format(START));
            return;
        }
        while (transition != null && transition.getTime() < END) {
            long now = transition.getTime();
            Date nowDate = new Date(now);
            System.out.printf("%s %s %s %s\r\n",
                    INSTANT_FORMAT.format(nowDate),
                    printOffset(zone.getOffset(now)),
                    zone.inDaylightTime(nowDate) ? "daylight" : "standard",
                    ZONE_NAME_FORMAT.format(nowDate));

            transition = zone.getNextTransition(now,  false);
        }
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
