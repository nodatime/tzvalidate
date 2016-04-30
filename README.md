This repository is a temporary home for code to generate "validation" text files
for the IANA-hosted time zone data.

For background, see http://mm.icann.org/pipermail/tz/2015-July/022349.html

If the approach proves successful, more discussion around the file format,
appropriate generation tools etc will be required. Initially, all code within
this repository should be seen as highly experimental, and likely to be hacked
together with little thought to longevity.

Data
====

I'm temporarily hosting data sourced from `zic`/`zdump` (which I view as canonical)
at http://nodatime.org/tzvalidate. You can download a zip file of each release you're
interested in, or one containing everything we've got.

Current implementations and requirements
====

All tools support the following command line options:

- `-f`: The "from" year - where to start looking for transitions. Defaults to "before any
  real transitions" (in a platform-specific way; often 1AD).
- `-t`: The "to" year - where to stop looking for transitions. Defaults to 2035.
- `-z`: The ID of single zone to dump, where applicable

 The "initially" line is unaffected by the from/to year combination.

Noda Time
----

The Noda Time code is now within the [Noda Time repository](https://github.com/nodatime/nodatime) to
enable direct handling of tz data.

Loads data from one of three sources:

- A Noda Time nzd file (local or via http/https)
- A tzdata directory
- A tzdata tar.gz file (local or via http/https)

Additional command line options:

- `-s`: (required) source file or directory

Reports initial offset and all transitions within bounds. Code exists in the main Noda Time repository,
in order to use an assembly which isn't exported in releases.

Sample command lines:

- `NodaTime.TzValidate.NodaDump -s http://nodatime.org/tzdb/tzdb2015e.nzd`
- `NodaTime.TzValidate.NodaDump -s www\tzdb\tzdb2015e.nzd`
- `NodaTime.TzValidate.NodaDump -s data\tzdb\2015e`
- `NodaTime.TzValidate.NodaDump -s http://www.iana.org/time-zones/repository/releases/tzdata2015e.tar.gz`
- `NodaTime.TzValidate.NodaDump -s downloads\tzdata2015e.tar.gz`
- `NodaTime.TzValidate.NodaDump -s http://nodatime.org/tzdb/tzdb2015e.nzd -f 1900 -t 2000`

TimeZoneInfo (.NET)
----

Reports transitions in .NET's "native" `TimeZoneInfo` class. Unless you're running on a system
using time zone data built from the IANA sources, this is unlikely to be similar to any other output
- in particular, many zones have recurrences which effectively start in 1AD.

Code: `csharp/src/NodaTime.TzValidate.TimeZoneInfoDump`

Supports the regular command line options, and no additional options.

zic
----

C# code to report transitions stored in the output of `zic`.

Requires:

- A "modern" version of `zic` to have compiled a data source already, as per the IANA web site.
 
Code: `csharp/src/NodaTime.TzValidate.ZicDump`

Additional command line options:

- `-s`: (required) source file or directory; either a file for a single zone,
  or a directory to be searched recursively for files
- `-i`: (optional) whether or not to fake an initial offset for zones which don't
  report a transition at the big bang; values are "true" or "false" - defaults to "true"

Sample command line: `dnx run -s tzdb2015e`

Java 7
----

Uses `java.util.TimeZone`, probing each day for transitions (as there's no "next transition" method).

Requires:

- A JRE update to the time zone database you want to use. See [the Timezone updater tool](http://www.oracle.com/technetwork/java/javase/tzupdater-readme-136440.html)
- Java 7 (it probably works with Java 1.4 or something ridiculous, but importantly it doesn't require Java 8)
- [Apache Commons CLI](https://commons.apache.org/proper/commons-cli/) - v1.3.1 or later

Code: `java/org/nodatime/tzvalidate/Java7Dump`

Sample command line: `java -cp bin;lib/commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java7Dump`

Java 8
----

Uses `java.time.*` to find zones and transitions.

Requires:

- A JRE update to the time zone database you want to use. See [the Timezone updater tool](http://www.oracle.com/technetwork/java/javase/tzupdater-readme-136440.html)
- Java 8+
- [Apache Commons CLI](https://commons.apache.org/proper/commons-cli/) - v1.3.1 or later

Code: `java/org/nodatime/tzvalidate/Java8Dump`

Sample command line: `java -cp bin;lib/commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java8Dump`

Joda Time
----

Uses Joda Time's built-in tz compiler to work from source data. 

Requires:

- [Joda Time](http://joda.org/joda-time) of some version - probably nothing too recent, but I use 2.8.1
- [Apache Commons CLI](https://commons.apache.org/proper/commons-cli/) - v1.3.1 or later

Code: `java/org/nodatime/tzvalidate/JodaDump`

Additional command line options:

- `-s`: (required) directory containing tzdata source

Sample command line: `java -cp bin;lib/joda-time-2.8.1.jar;lib/commons-cli-1.3.1.jar org.nodatime.tzvalidate.JodaDump -s ../tzdata/2015e`

ICU4J
----

Uses the time zone database built into ICU4J.

Requires:

- [ICU4J](http://site.icu-project.org/home) - probably nothing too recent, but I use 55.1
- An updated time zone database in the jar file; see [the ICU4J Time Zone Update Utility](http://icu-project.org/download/icutzu.html) for more details.  
- [Apache Commons CLI](https://commons.apache.org/proper/commons-cli/) - v1.3.1 or later

Code: `java/org/nodatime/tzvalidate/IcuDump`

Sample command line: `java -cp bin;lib/icu4j-55_1.jar;lib/commons-cli-1.3.1.jar org.nodatime.tzvalidate.IcuDump`

Note: ICU4J appears not to have any way of getting at time zone abbreviations (PST, PDT etc). I've tried various approaches, but nothing gives
the expected results. Improvements welcome!

Ruby
----

Uses the [`tzinfo`](https://tzinfo.github.io/) package to get detailed data. 

Requires:

- `tzinfo` gem 
- `tzinfo-data` gem
- `trollop` gem

Code: `ruby/tzdump.rb`

Sample command line: `ruby ruby/tzdump.rb`
