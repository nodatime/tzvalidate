The aim of the project is to produce tools for a variety of APIs/platforms,
where each tool will produce output in the same format. If two tools
produce the same output for the same version of time zone data, we
can be reasonably confident that they have interpreted the source
data in the same way - at least for the period of time covered by
the tool.

Overall format
====

The entire file is a text file encoded using UTF-8, with no leading
byte-order mark. Where the description mentions a line break, this
is a Unix-style line break: the U+000A (`\n`) character.

The file consists of an optional header section, follow by a body.

Header section (optional)
====

This is simply a list of key/value pairs, in the form

    *key* `:` *value*
    
followed by an empty line to indicate that the following line is the
start of the body.

The keys and values may contain (broadly) any letters, numbers,
punctuation, although the key must not contain a colon (`:`).
Separator characters **must not** occur in keys or values.
(Future versions of this format may nail down the set of characters
more thoroughly.)

The order of headers is not significant.

Leading and trailing whitespace is insignificant for headers, so the
following header lines are equivalent:

    HeaderKey:HeaderValue
      HeaderKey  :  HeaderValue
      
Common headers
----

The intention of the headers section is to provide metadata about
the body section. A generator may produce any headers it deems
useful, but the following header keys have an expected value format
and meaning:

- `Format`: a version indicator for the format of this file. This
  document describes version `tzvalidate-0.1`.
- `Version`: a version indicator for the data described in the file.
  Example: `2016d`
- `Range`: a range of years covered by transitions in the
  file. A year number expresses the first instant of that year in UTC,
  and the upper bound is exclusive. If the file covers all transitions
  from the start of time, the year 1 should be used as the start point.
  Examples: `1-2035` (the canonical format), `1900-2000`
  (transitions on or after 1900-01-01T00:00:00Z and strictly
  before 2000-01-01 00:00:00Z).
- `Generator`: The name of the tool generating the file. Examples:
  `zdump`, `NodaTime.TzValidate.NodaDump`.
- `GeneratorUrl`: The URL to visit for more information and/or source
  code of the tool generating the file.
- `Body-SHA-256`: A SHA-256 hash of the body section of the file.

Body section
====

The zones present are sorted using an ordinal comparison of Unicode
code points. (In practice, all zone IDs are currently ASCII, so this
is an ordinal comparison of ASCII values.) All known zones are included,
even if they are aliases for other zones.

The information for each zone consists of:

- The zone ID (e.g. "London/Europe"
- An initial line indicating the state of the time zone before the
  first transition. This has the same format as a transition line,
  but with `Initially:` followed by 11 spaces instead of the
  transition instant.
- A line per transition within the generated range.
- A blank line

Note that even the final zone in the file ends with a blank line,
for simplicity of generation.

The canonical range of transitions (i.e. in distributed data) is from the start of
time until 2035-01-01 00:00:00Z (exclusive).

Each transition line consists of:

- The instant of the transition, in the format `yyyy-MM-dd HH:mm:ssZ`
  (where `Z` is a literal)
- A space
- The offset from UTC after the transition, in the format "+hh:mm:ss"
  (where the + is either '+' or '-', but present in either case; '+'
  indicates an offset ahead of UTC, e.g. Eastern Europe)
- A space
- The string "daylight" or "standard" to indicate whether the
  period after the transition is in daylight or standard time
- A space
- The abbreviated name of the zone at this time, e.g. "CST" or "CDT".
  This is the abbreviation for the period from this transition to
  the next transition.
  
As a complete example, the 2016c data should generate the following
lines for the `America/La_Paz` zone. 

    America/La_Paz
    Initially:           -04:32:36 standard LMT
    1890-01-01 04:32:36Z -04:32:36 standard CMT
    1931-10-15 04:32:36Z -03:32:36 daylight BOST
    1932-03-21 03:32:36Z -04:00:00 standard BOT

Motivation for the format
====

- By specifying a precise format including encoding, two tools
  generating the same data should create byte-wise identical bodies,
  allowing hashes to be published as well as the full text files.
  There are many situations where a hash can reasonably be included
  alongside other data, but the full validation text file could not
  due to space considerations.
- One line per transition makes for simple diffs. (A format of
  "zone intervals" of constant name/offset instead of transitions
  makes for simpler reading in some ways, but then a change in
  transition affects multiple lines.)
- The only variable-width part of the transition format is the name of
  the zone after the transition. This allows the data to be effectively
  read as a table.
- The extra spaces after "Initially:" make the initial offset line up
  with the offsets of transitions
- The transition instant is expressed in UTC and in a format which is
  simple to parse in code if desired. (It's not quite ISO-8601 as the
  'T' between the date and time format proved to hamper readability,
  but it should be easy to parse on any platform.)
- Ending in 2035 gives reasonable confidence in validation without breaking
  past the 2038 Unix timestamp apocalypse. In particular, `zic` appears
  to generate data up until 2037 and then use a recurrence rule.
