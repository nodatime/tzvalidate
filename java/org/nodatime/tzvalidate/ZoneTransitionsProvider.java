// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;

/**
 * Represents the ability to dump the details of a single time zone.
 */
public interface ZoneTransitionsProvider {
    /**
     * Initializes the dumper with the given options.
     */
    void initialize(DumpOptions options) throws IOException;
    
    /**
     * Retrieves all zone IDs available to this dumper. This method
     * will only be called after {@link #initialize(DumpOptions)}.
     */
    Iterable<String> getZoneIds();
    
    /**
     * Retrieves the time zone transitions for the specified zone ID.
     * @param id ID of the zone to retrieve transitions for
     * @param fromYear First year (inclusive) in which to find transitions
     * @param toYear End year (exclusive) in which to find transitions
     * @return the transitions for the zone, in the given year range
     */
    ZoneTransitions getTransitions(String id, int fromYear, int toYear);
}
