REM Note: This script expects the layout on Jon's laptop, with all
REM libraries already downloaded...

java -cp bin;lib\joda-time-2.9.4.jar;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.JodaDump ..\..\iana\2016d > joda.txt
java -cp bin;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java7Dump -noabbr > java7.txt
java -cp bin;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.Java8Dump -noabbr > java8.txt
java -cp bin;lib\icu4j-57_1.jar;lib\commons-cli-1.3.1.jar org.nodatime.tzvalidate.IcuDump > icu4j.txt
ruby ruby\tzdump.rb > ruby.txt