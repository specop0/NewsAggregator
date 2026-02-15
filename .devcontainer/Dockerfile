# Image with Node.js
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-with-node
RUN bash -E $(curl -fsSL https://deb.nodesource.com/setup_24.x | bash - ); apt install -y nodejs

# Default User
USER 1000:1000