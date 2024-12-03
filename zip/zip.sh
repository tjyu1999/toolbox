#!/bin/bash

if [ $# -ne 3 ]; then
    echo "$0 pyver {URL} {FILENAME}"
    echo "$0 csver {URL} {FILENAME}"
    exit 1
fi

COMMAND=$1
URL=$2
FILENAME=$3

if [ -d "$FILENAME" ]; then
    echo "Remove existing folder $FILENAME."
    rm -rf "$FILENAME"
fi

if [ -f "$FILENAME.zip" ]; then
    echo "Remove existing file $FILENAME.zip."
    echo
    rm -f "$FILENAME.zip"
fi

if [ "$COMMAND" == "pyver" ]; then
    python3 pyver.py "$URL" "$FILENAME"
elif [ "$COMMAND" == "csver" ]; then
    cd csver/ || { echo "Failed to enter directory."; exit 1; }
    dotnet run "$URL" "$FILENAME"
else
    echo "Invalid command: $COMMAND"
    exit 1
fi
