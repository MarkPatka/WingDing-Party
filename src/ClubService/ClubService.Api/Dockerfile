# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
RUN apt-get update && apt-get install -y --no-install-recommends libgssapi-krb5-2 && rm -rf /var/lib/apt/lists/*
USER $APP_UID
WORKDIR /app
EXPOSE 5400
EXPOSE 5401


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/ClubService/ClubService.Api/ClubService.Api.csproj", "src/ClubService/ClubService.Api/"]
COPY ["src/ClubService/ClubService.Application/ClubService.Application.csproj", "src/ClubService/ClubService.Application/"]
COPY ["src/ClubService/ClubService.Domain/ClubService.Domain.csproj", "src/ClubService/ClubService.Domain/"]
COPY ["src/ClubService/ClubService.Contracts/ClubService.Contracts.csproj", "src/ClubService/ClubService.Contracts/"]
COPY ["src/ClubService/ClubService.Infrastructure/ClubService.Infrastructure.csproj", "src/ClubService/ClubService.Infrastructure/"]
RUN dotnet restore "./src/ClubService/ClubService.Api/ClubService.Api.csproj"
COPY . .
WORKDIR "/src/src/ClubService/ClubService.Api"
RUN dotnet build "./ClubService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ClubService.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClubService.Api.dll"]