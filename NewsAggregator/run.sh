#!/bin/bash

# appsettings.Development.json
echo dotnet run --environment Development
dotnet run --environment Development

# environment variable
# env CORS__AllowedOrigins__0="http://localhost:40081" dotnet run