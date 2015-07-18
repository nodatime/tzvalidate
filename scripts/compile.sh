#!/bin/bash
RELEASE=$1
OUTPUT=../output/${RELEASE}

echo Compiling ${RELEASE}
zic -y ${RELEASE}/yearistype.sh -d ${OUTPUT} ${RELEASE}/{africa,antarctica,asia,australasia,europe,northamerica,southamerica,pacificnew,etcetera,backward,systemv}
