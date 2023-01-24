FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln .
COPY Telemetry/*.csproj ./Telemetry/
COPY Telemetry.Entities/*.csproj ./Telemetry.Entities/
COPY Mongo.AspNetCore.Identity/*.csproj ./Mongo.AspNetCore.Identity/

# Restore as distinct layers
RUN dotnet restore

# Copy everything else and build app
COPY Telemetry/. ./Telemetry/
COPY Telemetry.Entities/. ./Telemetry.Entities/
COPY Mongo.AspNetCore.Identity/. ./Mongo.AspNetCore.Identity/

WORKDIR /app/Telemetry

# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app/Telemetry/out ./
ENTRYPOINT ["dotnet", "Telemetry.dll"]
