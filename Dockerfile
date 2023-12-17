# Image with Node.js
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-with-node
RUN bash -E $(curl -fsSL https://deb.nodesource.com/setup_18.x | bash - ); apt install -y nodejs

# Build Preperation
FROM build-with-node as build
WORKDIR /src
COPY NewsAggregator NewsAggregator
RUN dotnet restore NewsAggregator/NewsAggregator.csproj

# Test
FROM build AS test
WORKDIR /src
COPY NewsAggregator.Tests/NewsAggregator.Tests.csproj NewsAggregator.Tests/
RUN dotnet restore NewsAggregator.Tests/NewsAggregator.Tests.csproj
COPY NewsAggregator.Tests NewsAggregator.Tests
RUN dotnet build NewsAggregator.Tests/NewsAggregator.Tests.csproj --no-restore
ENTRYPOINT [ "dotnet", "test", "NewsAggregator.Tests/NewsAggregator.Tests.csproj", "--logger:trx", "--no-build" ]

# Publish
FROM build as publish
WORKDIR /src
RUN dotnet publish NewsAggregator/NewsAggregator.csproj --configuration Release --output publish

# Run
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=publish /src/publish .
ENTRYPOINT ["dotnet", "NewsAggregator.dll"]
EXPOSE 5010