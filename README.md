This repository is a temporary home for code to generate "validation" text files
for the IANA-hosted time zone data.

For background, see http://mm.icann.org/pipermail/tz/2015-July/022349.html

If the approach proves successful, more discussion around the file format,
appropriate generation tools etc will be required. Initially, all code within
this repository should be seen as highly experimental, and likely to be hacked
together with little thought to longevity.

Current implementations and requirements
====

Noda Time
----

Loads an existing nzd (Noda Zone Data) file and reports transitions.

Code: `csharp/TzValidate/NodaDump`

Requires: Noda Time (restore nuget package on solution)

Sample command line: `NodaDump http://nodatime.org/tzdb/tzdb2015e.nzd`

zdump
----

C# code to invoke `zdump` for each time zone ID known to Noda Time, and then munge
the output into the regular format. 

Requires:

- A "modern" version of `zdump` to be built already, as per the IANA web site.
- Noda Time (restore nuget package on solution)
 
Code: `csharp/TzValidate/MungeZdump`

Sample command line: `mono MungeZdump.exe http://nodatime.org/tzdb/tzdb2015e.nzd ~/tzdir/etc/zdump`

Note: Modern `zdump` doesn't output anything useful when asked for verbose output for a fixed zone like "EST".
There are awkward ways around this, but they really are annoying. More work needed. (It's also annoying that
we can't easily run this on Windows, as far as I can tell. It makes testing all of this somewhat painful. Oh,
and it's also really slow, at least the way we're doing it...)

Java 7
----

Uses `java.util.TimeZone`, probing each day for transitions (as there's no "next transition" method).

Requires:

- A JRE update to the time zone database you want to use. See [the Timezone updater tool](http://www.oracle.com/technetwork/java/javase/tzupdater-readme-136440.html)
- Java 7 (it probably works with Java 1.4 or something ridiculous, but importantly it doesn't require Java 8)

Code: `java/org/nodatime/tzvalidate/Java7Dump`

Sample command line: `java -cp bin org.nodatime.tzvalidate.Java7Dump`

Java 8
----

Uses `java.time.*` to find zones and transitions.

Requires:

- A JRE update to the time zone database you want to use. See [the Timezone updater tool](http://www.oracle.com/technetwork/java/javase/tzupdater-readme-136440.html)
- Java 8+

Code: `java/org/nodatime/tzvalidate/Java8Dump`

Sample command line: `java -cp bin org.nodatime.tzvalidate.Java8Dump`

Joda Time
----

Uses Joda Time's built-in tz compiler to work from source data. 

Requires:

- Joda Time of some version - probably nothing too recent, but I use 2.8.1

Code: `java/org/nodatime/tzvalidate/JodaDump`

Sample command line: `java -cp bin;lib/joda-time-2.8.1.jar org.nodatime.tzvalidate.JodaDump ../tzdata/2015e`

ICU4J
----

Uses the time zone database built into ICU4J.

Requires:

- ICU4J - probably nothing too recent, but I use 55.1
- An updated time zone database in the jar file; see [the ICU4J Time Zone Update Utility](http://icu-project.org/download/icutzu.html) for more details.  

Code: `java/org/nodatime/tzvalidate/IcuDump`

Sample command line: `java -cp bin;lib/icu4j-55_1.jar org.nodatime.tzvalidate.IcuDump`

Note: ICU4J appears not to have any way of getting at time zone abbreviations (PST, PDT etc). I've tried various approaches, but nothing gives
the expected results. Improvements welcome!

Ruby
----

Uses the [`tzinfo`](https://tzinfo.github.io/) package to get detailed data. 

Requires:

- `tzinfo` gem 
- `tzinfo-data` gem

Code: `ruby/tzdump.rb`

Sample command line: `ruby ruby/tzdump.rb`
