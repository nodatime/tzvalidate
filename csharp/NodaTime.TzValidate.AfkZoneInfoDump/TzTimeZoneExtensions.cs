// Copyright 2018 Jon Skeet. All rights reserved.
// Use of this source code is governed by the Apache License 2.0,
// as found in the LICENSE.txt file.

using Afk.ZoneInfo;
using System;

namespace NodaTime.TzValidate.AfkZoneInfoDump
{
    /// <summary>
    /// Extension methods to give TzTimeZone an API more consistent with TimeZoneInfo
    /// </summary>
    internal static class TzTimeZoneExtensions
    {
        internal static TimeSpan GetUtcOffset(this TzTimeZone zone, DateTimeOffset dateTimeOffset)
        {
            var utc = dateTimeOffset.UtcDateTime;
            return zone.ToLocalTime(utc) - utc;
        }
    }
}
