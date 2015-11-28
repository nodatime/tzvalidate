#!/bin/bash
RELEASE=$1
OUTPUT=../output/${RELEASE}

FILES=
for i in africa antarctica asia australasia europe northamerica southamerica pacificnew etcetera backward systemv; do
  if [ -f "${RELEASE}/${i}" ]; then
    FILES="${FILES} ${RELEASE}/${i}"
  fi
done

echo Compiling ${RELEASE}
zic -y ${RELEASE}/yearistype.sh -d ${OUTPUT} ${FILES}

# Some tools (e.g. TimeZoneInfo in CoreCLR) need zone.tab
cp ${RELEASE}/zone.tab ${OUTPUT}
