#!/bin/bash

if [ $# -ne 2 ]; then
    echo "$0 pyver {URL}"
    echo "$0 csver {URL}"
    exit 1
fi

COMMAND=$1
URL=$2

if [ "$COMMAND" == "pyver" ]; then
    python3 pyver.py "$URL"
elif [ "$COMMAND" == "csver" ]; then
    cd csver
    dotnet run "$URL"
else
    echo "Invalid command: $COMMAND"
    exit 1
fi
