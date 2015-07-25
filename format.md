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
- An initial line indicating the state of the time zone before the
  first transition. This has the same format as a transition line, but
  with `Initially:` instead of the transition instant
- A line per transition strictly before 2035-01-01T00:00:00Z
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
- Ending in 2035 gives reasonable confidence in validation without breaking
  past the 2038 Unix timestamp apocalypse. In particular, `zic` appears
  to generate data up until 2037 and then use a recurrence rule.
