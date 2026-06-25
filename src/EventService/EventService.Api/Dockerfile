# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0-noble AS base
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 5300
EXPOSE 5301

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0-noble AS build
ARG BUILD_CONFIGURATION=Release
# Preserve the repo's src/ layout so Directory.Packages.props (Central Package
# Management) stays a common ancestor of EventService and the SharedKernel project,
# and the ..\..\SharedKernel project reference resolves the same way as on the host.
WORKDIR /src
COPY ["Directory.Packages.props", "./"]
COPY src/EventService/ ./EventService/
COPY src/SharedKernel/ ./SharedKernel/
RUN dotnet restore "EventService/EventService.Api/EventService.Api.csproj"

WORKDIR "/src/EventService/EventService.Api"
RUN dotnet build "EventService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "EventService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "EventService.Api.dll"]