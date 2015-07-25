// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.util.Arrays;
import java.util.Date;
import java.util.Locale;

import org.apache.commons.cli.ParseException;

import com.ibm.icu.text.SimpleDateFormat;
import com.ibm.icu.util.BasicTimeZone;
import com.ibm.icu.util.Calendar;
import com.ibm.icu.util.GregorianCalendar;
import com.ibm.icu.util.TimeZone;
import com.ibm.icu.util.TimeZoneTransition;

public final class IcuDump {
    private static final TimeZone UTC = TimeZone.getTimeZone("UTC");
    private static final SimpleDateFormat INSTANT_FORMAT = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm:ss'Z'", Locale.US);
    // A poor attempt to get the time zone abbreviation. It's locale-sensitive, and then only
    // applies some of the time.
    // See http://stackoverflow.com/questions/31626356
    private static final SimpleDateFormat ZONE_NAME_FORMAT = new SimpleDateFormat("zzz", Locale.US);
    
    static {
        INSTANT_FORMAT.setTimeZone(UTC);
    }

    public static void main(String[] args) throws IOException, ParseException {
        DumpOptions options = DumpOptions.parse("IcuDump", args, false);
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
        BasicTimeZone zone = (BasicTimeZone) TimeZone.getTimeZone(id);
        ZONE_NAME_FORMAT.setTimeZone(zone);
        
        Calendar calendar = GregorianCalendar.getInstance(UTC);
        calendar.set(options.getFromYear(), 0, 1, 0, 0, 0);
        calendar.set(Calendar.MILLISECOND, 0);
        long start = calendar.getTimeInMillis();
        calendar.set(options.getToYear(), 0, 1, 0, 0, 0);
        long end = calendar.getTimeInMillis();
        // 1 AD to get the initial value
        calendar.set(1, 0, 1, 0, 0, 0);
        long early = calendar.getTimeInMillis();

        System.out.printf("Initially:           %s\r\n", formatOffsetAndName(zone, early));  
        
        TimeZoneTransition transition = zone.getNextTransition(start, true /* inclusive */);
        
        Date date = new Date();
        while (transition != null && transition.getTime() < end) {
            long now = transition.getTime();
            date.setTime(now);
            System.out.printf("%s %s\r\n",
                INSTANT_FORMAT.format(date),
                formatOffsetAndName(zone, now));
            transition = zone.getNextTransition(now, false /* exclusive */);
        }
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
