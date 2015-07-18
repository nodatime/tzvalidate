#!/bin/bash
if [ "$#" -ne 1 ] ; then
  echo "Usage: $0 <release directory>" >&2
  exit 1
fi

RELEASE=$PWD/$1

echo Processing ${RELEASE}

TRANSITION_FILE=$1-transition.txt
NOW_FILE=$1-now.txt
echo > ${TRANSITION_FILE}
echo > ${NOW_FILE}
for zone in `find ${RELEASE} -type f`; do
  zdump -v -c 1901,2036 ${zone} >> ${TRANSITION_FILE}
  zdump ${zone} >> ${NOW_FILE}
done

