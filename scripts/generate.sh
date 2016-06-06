#!/bin/bash

set -e

if [ "$#" -ne 1 ]; then
  echo "Usage: generate.sh <release, e.g. 2016d>"
  exit 1
fi

cd `dirname $0`/..

OUTDIR=tmp

rm -rf $OUTDIR
mkdir -p $OUTDIR/$1
pushd $OUTDIR/$1
mkdir $1
wget -q -O - https://www.iana.org/time-zones/repository/releases/tzdata$1.tar.gz | tar xz
wget -q -O - https://www.iana.org/time-zones/repository/releases/tzcode$1.tar.gz | tar xz

make -s

FILES=
for i in africa antarctica asia australasia europe northamerica southamerica pacificnew etcetera backward systemv; do
  if [ -f "${i}" ]; then
    FILES="${FILES} ${i}"
  fi
done

./zic -y yearistype.sh -d ../data ${FILES}

popd

dotnet restore csharp/src
dotnet build csharp/src/NodaTime.TzValidate.ZicDump --quiet
dotnet run -p csharp/src/NodaTime.TzValidate.ZicDump -- -s $OUTDIR/data -v $1 > $OUTDIR/tzdata$1.txt
