// Copyright 2016 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

package org.nodatime.tzvalidate;

import java.io.IOException;
import java.io.Writer;

/**
 * Represents the ability to dump the details of a single time zone.
 */
public interface ZoneDumper {
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
     * Dumps the given time zone information, from the given start
     * year (inclusive) to the given end year (exclusive). Lines should be
     * terminated with a line-feed character (U+000A). No blank line should be
     * written after the zone.
     */
    void dumpZone(String id, int fromYear, int toYear, Writer writer)
        throws IOException;
}
