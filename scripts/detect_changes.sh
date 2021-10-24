#!/bin/bash

set -e
RELEASE=$(curl -s https://www.iana.org/time-zones | \
  grep -oP '\<span id="version"\>[A-Za-z0-9]+</span\>'| \
  sed 's/<span id="version">//g' | \
  sed 's/<\/span>//g')
echo "Detected release ${RELEASE} at IANA"
HTML=$(wget -q -O- https://nodatime.github.io/tzvalidate/)
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
git clone https://github.com/nodatime/tzvalidate.git -b gh-pages gh-pages
ZIPFILE=tzdata$RELEASE-tzvalidate.zip
HASHFILE=tzdata$RELEASE-sha256.txt
HASH=`cat tzdata$RELEASE.txt | head -n 5 | grep Body-SHA-256 | cut -d: -f2 | sed 's/ //g'`
zip $ZIPFILE tzdata$RELEASE.txt
cp $ZIPFILE gh-pages

sed -i "s/# Insert here/# Insert here\n- [$RELEASE]($ZIPFILE): $HASH/g" gh-pages/index.md
cd gh-pages
echo $HASH > $HASHFILE
git add $ZIPFILE $HASHFILE index.md
git commit -m "Added $RELEASE"
