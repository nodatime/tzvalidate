package org.nodatime.tzvalidate;

import java.io.IOException;
import java.io.Writer;
import java.text.DateFormat;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
import java.util.Locale;
import java.util.TimeZone;

/**
 * Collects and then formats the transitions for a single ID.
 * This class expresses instants in time as {@see Date} instances as a simple lowest common denominator.
 * This class is not thread-safe.
 */
public final class ZoneTransitions {
    private final String id;
    private final List<Transition> transitions = new ArrayList<>();

    public ZoneTransitions(String id) {
        this.id = id;
    }

    /**
     * Adds a transition for this zone.
     * @param when The instant at which the transition occurs, or null for the initial state. This is cloned.
     * @param offsetMilliseconds The UTC offset in milliseconds after the transition (positive for offsets ahead of UTC; negative for offsets behind UTC)
     * @param daylight Whether daylight saving is in effect after the transition
     * @param abbreviation The time zone abbreviation after the transition
     */
    public void addTransition(Date when, long offsetMilliseconds, boolean daylight, String abbreviation) {
        transitions.add(new Transition(when, offsetMilliseconds, daylight, abbreviation));
    }

    public void writeTo(Writer writer, boolean includeAbbreviations) throws IOException {
        writer.write(id + "\n");
        for (Transition transition : transitions) {
            transition.writeTo(writer, includeAbbreviations);
        }
    }
    
    private static final class Transition {
        private static final TimeZone UTC = TimeZone.getTimeZone("UTC");
        private static final ThreadLocal<DateFormat> FORMATTER = new ThreadLocal<DateFormat>() {
            @Override
            protected DateFormat initialValue() {
                DateFormat format = new SimpleDateFormat("yyyy-MM-dd HH:mm:ss'Z'", Locale.US);
                format.setTimeZone(UTC);
                return format;
            }
        };
        
        private final Date when;
        private final long offsetSeconds;
        private final boolean daylight;
        private final String abbreviation;

        private Transition(Date when, long offsetMilliseconds, boolean daylight, String abbreviation) {
            this.when = when == null ? null : new Date(when.getTime());
            this.offsetSeconds = offsetMilliseconds / 1000;
            this.daylight = daylight;
            this.abbreviation = abbreviation;
        }
        
        public void writeTo(Writer writer, boolean includeAbbreviation) throws IOException {
            if (when != null) {
                writer.write(FORMATTER.get().format(when) + " ");
            } else {
                writer.write("Initially:           ");
            }
            long absoluteSeconds = Math.abs(offsetSeconds);
            String sign = offsetSeconds < 0 ? "-" : "+";
            writer.write(String.format("%s%02d:%02d:%02d %s",
                sign, absoluteSeconds / 3600, (absoluteSeconds / 60) % 60, absoluteSeconds % 60,
                daylight ? "daylight" : "standard"));
            if (includeAbbreviation) {
                writer.write(" " + abbreviation);
            }
            writer.write("\n");
        }
    }
}
