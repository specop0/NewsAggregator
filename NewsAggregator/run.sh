#!/bin/bash

# appsettings.Development.json
echo dotnet run --environment DOTNET_ENVIRONMENT=Development
dotnet run --environment DOTNET_ENVIRONMENT=Development

# environment variable
# env CORS__AllowedOrigins__0="http://localhost:40081" dotnet run