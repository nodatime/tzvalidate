#!/bin/bash

set -e

# Note: the .* here can be removed when https://github.com/nodatime/nodatime/issues/502 has been fixed
RELEASE=`wget -q -O- http://nodatime.org/tzdb/latest.txt | sed -r 's/.*tzdb([a-z0-d]+)\.nzd.*/\1/g'`

HTML=`wget -q -O- http://nodatime.github.io/tzvalidate/`
MATCH=">${RELEASE}<"

if [[ $HTML == *$MATCH* ]]
then
  echo Already got $RELEASE. Exiting.
  exit 0
fi

echo Generating $RELEASE.

ROOT=`dirname $0`/..
OUT=$ROOT/tmp

$ROOT/scripts/generate.sh $RELEASE

cd $OUT
git clone https://github.com/nodatime/tzvalidate.git -b gh-pages gh-pages
ZIPFILE=tzdata$RELEASE-tzvalidate.zip
zip $ZIPFILE tzdata$RELEASE.txt
cp $ZIPFILE gh-pages

sed -i "s/# Insert here/# Insert here\n- [$RELEASE]($ZIPFILE)/g" gh-pages/index.md 
cd gh-pages
git add $ZIPFILE index.md
git commit -m "Added $RELEASE"
