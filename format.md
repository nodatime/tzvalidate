The aim of the project is to produce tools for a variety of APIs/platforms,
where each tool will produce output in the same format. If two tools
produce the same output for the same version of time zone data, we
can be reasonably confident that they have interpreted the source
data in the same way - at least for the period of time covered by
the tool.

Current format
----

The zones present are sorted using an ordinal comparison of UTF-16
code units. (In practice, all zone IDs are currently ASCII, so this
is an ordinal comparison of ASCII values.) All known zones are included,
even if they aliases for other zones.

The information for each zone consists of:

- The zone ID (e.g. "London/Europe"
- A line per transition strictly after 1905-01-01T00:00:00Z and
  strictly before 2035-01-01T00:00:00Z
- A blank line

Each transition line consists of:

- The instant of the transition, in the format `yyyy-MM-ddTHH:mm:ssZ`
  (where both `T` and `Z` are literals)
- A space
- The offset from UTC after the transition, in the format "+hh:mm:ss"
  (where the + is either '+' or '-', but present in either case; '+'
  indicates an offset ahead of UTC, e.g. Eastern Europe)
- A space
- The string "daylight" or "standard" to indicate whether the
  period after the transition is in daylight or standard time
- A space
- The abbreviated name of the zone at this time, e.g. "CST" or "CDT"

Lines are separated by "\r\n" - the Windows default line
separator - until such time as that becomes annoying. (It's
convenient for me, Jon Skeet, as I mostly work on Windows...)

If there are no transitions for the zone (within the range 1900-2050), a
single line is included below the zone ID, with a format of:

- The literal "Fixed: "
- The offset, in the format above
- The name of the zone within the period 1900-2050

(It is possible that the zone isn't genuinely fixed for all time, of course -
only within the period 1905-2035.)

Motivation for the format
====

- One line per transition makes for simple diffs. (A format of
  "zone intervals" of constant name/offset instead of transitions
  makes for simpler reading in some ways, but then a change in
  transition affects multiple lines.)
- The only variable-width part of the transition format is the name of
  the zone after the transition. This allows the data to be effectively
  read as a table.
- The transition instant is expressed in UTC and in ISO-8601 extended
  format to be as simple as possible to parse in code if desired.
- The range of 1905-2035 is just about within the bounds that zdump supports

Ideal format
----

Currently, there's no indication of the split between "standard
offset" and "daylight savings"; only an indication of whether a
transition is into standard time or daylight time. Daylight savings
comprise 1 hour (forward) in almost all cases, but a few time zones
(e.g. Antarctica/Troll) do not follow this pattern.

The current restriction is due to the output of `zdump` - which in
turn is due to the output of `zic` not including the amount of
daylight savings. If we regard `zic` as the canonical implementation,
we'd either need a new file format, a clone of the `zic` code, or
a modified version of `zic` which optionally wrote out a text dump
file as part of its operation.

The ideal format would potentially include two offsets instead of an
offset and "standard" or "daylight". The two offsets to represent
could be any pair of:

- Wall and standard
- Wall and daylight
- Standard and daylight

The third value can easily be derived from the other two in any
case, but each makes a different use case simple. We could include
all three, at the cost of redundancy/verbosity.
