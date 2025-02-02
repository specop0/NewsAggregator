# Image with Node.js
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-with-node
RUN bash -E $(curl -fsSL https://deb.nodesource.com/setup_20.x | bash - ); apt install -y nodejs

# Build Preperation
FROM build-with-node AS build
WORKDIR /src
COPY NewsAggregator/NewsAggregator.csproj NewsAggregator/NewsAggregator.csproj
COPY NewsAggregator.Tests/NewsAggregator.Tests.csproj NewsAggregator.Tests/NewsAggregator.Tests.csproj
COPY NewsAggregator.sln NewsAggregator.sln
RUN dotnet restore NewsAggregator.sln
COPY NewsAggregator NewsAggregator

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
FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy-chiseled
WORKDIR /app
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "NewsAggregator.dll"]
EXPOSE 5010