REM Note: This script expects the layout on Jon's laptop, with all
REM libraries already downloaded...

java -cp bin;lib\joda-time-2.9.4.jar;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.JodaDump ..\..\iana\2016d > joda.txt
java -cp bin;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java7Dump -noabbr -f 1901 > java7.txt
java -cp bin;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java8Dump -noabbr > java8.txt
java -cp bin;lib\icu4j-57_1.jar;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.IcuDump > icu4j.txt
java -cp bin;lib\commons-cli-1.3.1.jar;lib\time4j-core-4.17.jar;lib\time4j-i18n-4.17.jar;lib\time4j-olson-4.17.jar;lib\time4j-tzdata-1.10-2016f.jar org.nodatime.tzvalidate.Time4JDump -noabbr > time4j.txt
ruby ruby\tzdump.rb > ruby.txt
