REM Note: This script expects the layout on Jon's laptop, with all
REM libraries already downloaded...

java -cp bin;lib\joda-time-2.8.1.jar;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.JodaDump ..\NodaTime\data\tzdb\2015e > joda.txt
java -cp bin;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java7Dump > java7.txt
java -cp bin;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java8Dump > java8.txt
java -cp bin;lib\icu4j-55_1.jar;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.IcuDump > icu4j.txt
ruby ruby\tzdump.rb > ruby.txt