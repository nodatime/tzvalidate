#!/bin/bash

set -e

RELEASE=$(curl -s https://www.iana.org/time-zones | grep -oP '(?<=\/time-zones\/repository\/releases\/tzdata)[A-Za-z0-9]+')
HTML=`wget -q -O- https://nodatime.github.io/tzvalidate/`
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
# Note the use of SSH instead of HTTPS here, for use with deploy keys
git clone git@github.com:nodatime/tzvalidate.git -b gh-pages gh-pages
ZIPFILE=tzdata$RELEASE-tzvalidate.zip
zip $ZIPFILE tzdata$RELEASE.txt
cp $ZIPFILE gh-pages

sed -i "s/# Insert here/# Insert here\n- [$RELEASE]($ZIPFILE)/g" gh-pages/index.md 
cd gh-pages
git add $ZIPFILE index.md
git commit -m "Added $RELEASE"
