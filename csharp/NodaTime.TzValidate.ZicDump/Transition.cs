// Copyright 2015 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using System;
using System.Globalization;

namespace NodaTime.TzValidate.ZicDump
{
    /// <summary>
    /// A transition within a zone file.
    /// </summary>
    public sealed class Transition
    {
        /// <summary>
        /// The instant at which the transition occurred, or null for
        /// the first transition in a zone, specifying its initial period.
        /// The offset will always be 0.
        /// </summary>
        public DateTimeOffset? Instant { get; }

        /// <summary>
        /// The total offset from UTC from the given instant until the next
        /// transition.
        /// </summary>
        public TimeSpan Offset { get; }
        
        /// <summary>
        /// <c>true</c> if this is a transition into daylight saving time;
        /// false otherwise.
        /// </summary>
        public bool IsDaylight { get; }

        /// <summary>
        /// The abbreviation for this period, e.g. "CST" or "CDT".
        /// </summary>
        public string Abbreviation { get; }

        public Transition(DateTimeOffset? instant, TimeSpan offset, bool isDst, string abbreviation)
        {
            this.Instant = instant;
            this.Offset = offset;
            this.IsDaylight = isDst;
            this.Abbreviation = abbreviation;
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2} {3}",
                Instant == null ? "Initially:          " : Instant.Value.ToString("yyyy-MM-dd HH:mm:ss'Z'", CultureInfo.InvariantCulture),
                // Urgh - no custom format for TimeSpan to specify a sign.
                // Good job we know that the offset is always within a day...
                (Offset.Ticks >= 0 ? "+" : "-") + Offset.ToString("hh':'mm':'ss", CultureInfo.InvariantCulture),
                IsDaylight ? "daylight" : "standard",
                Abbreviation);
        }
    }
}
