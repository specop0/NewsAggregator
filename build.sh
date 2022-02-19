#!/bin/bash

DIR=$(dirname "$(readlink -f "$0")")

cd $DIR/Parser
echo $DIR/Parser

echo dotnet publish --configuration Release --output bin/publish
dotnet publish --configuration Release --output bin/publish
