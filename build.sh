#!/bin/bash

DIR=$(dirname "$(readlink -f "$0")")

echo dotnet publish $DIR/NewsAggregator/NewsAggregator.csproj --configuration Release --output publish
dotnet publish $DIR/NewsAggregator/NewsAggregator.csproj --configuration Release --output publish
