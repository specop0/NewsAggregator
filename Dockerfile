# Image with Node.js & AOT requirements
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS base
RUN bash -E $(curl -fsSL https://deb.nodesource.com/setup_24.x | bash - ); apt install -y nodejs
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
       clang zlib1g-dev

# Build Preperation
FROM base AS build
WORKDIR /src
COPY NewsAggregator.App/package.json NewsAggregator.App/package.json
COPY NewsAggregator.App/package-lock.json NewsAggregator.App/package-lock.json
RUN cd NewsAggregator.App && npm install
COPY NewsAggregator/NewsAggregator.csproj NewsAggregator/NewsAggregator.csproj
COPY NewsAggregator.Tests/NewsAggregator.Tests.csproj NewsAggregator.Tests/NewsAggregator.Tests.csproj
COPY NewsAggregator.sln NewsAggregator.sln
RUN dotnet restore NewsAggregator.sln
COPY NewsAggregator NewsAggregator
COPY NewsAggregator.App NewsAggregator.App

# Test
FROM build AS test
WORKDIR /src
COPY NewsAggregator.Tests NewsAggregator.Tests
RUN dotnet build NewsAggregator.sln --no-restore
ENTRYPOINT [ "dotnet", "test", "--logger:trx", "--no-build" ]

# Publish
FROM build AS publish
WORKDIR /src
RUN dotnet publish NewsAggregator/NewsAggregator.csproj --configuration Release --output publish

# Run
FROM mcr.microsoft.com/dotnet/runtime-deps:9.0-noble-chiseled
WORKDIR /app
COPY --from=publish /src/publish .
ENTRYPOINT ["./NewsAggregator"]
EXPOSE 5010