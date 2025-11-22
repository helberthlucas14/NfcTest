# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["src/Nfc.Api/Nfc.Api.csproj", "src/Nfc.Api/"]
COPY ["src/Nfc.Application/Nfc.Application.csproj", "src/Nfc.Application/"]
COPY ["src/Nfc.Domain/Nfc.Domain.csproj", "src/Nfc.Domain/"]
COPY ["src/Nfc.Domain.Core/Nfc.Domain.Core.csproj", "src/Nfc.Domain.Core/"]
COPY ["src/Nfc.Infra.CrossCutting.IoC/Nfc.Infra.CrossCutting.IoC.csproj", "src/Nfc.Infra.CrossCutting.IoC/"]
COPY ["src/Nfc.Infra.CrossCutting.Commons/Nfc.Infra.CrossCutting.Commons.csproj", "src/Nfc.Infra.CrossCutting.Commons/"]
COPY ["src/Nfc.Infra.Data.EF/Nfc.Infra.Data.EF.csproj", "src/Nfc.Infra.Data.EF/"]
COPY ["src/Nfc.Infra.Data.Redis/Nfc.Infra.Data.Redis.csproj", "src/Nfc.Infra.Data.Redis/"]
COPY ["src/Nfc.Infra.HangFire/Nfc.Infra.HangFire.csproj", "src/Nfc.Infra.HangFire/"]
COPY ["src/Nfc.Infra.Storage/Nfc.Infra.Storage.csproj", "src/Nfc.Infra.Storage/"]
COPY ["src/Nfc.Infra.Observability/Nfc.Infra.Observability.csproj", "src/Nfc.Infra.Observability/"]
RUN dotnet restore "./src/Nfc.Api/Nfc.Api.csproj"
COPY . .
WORKDIR "/src/src/Nfc.Api"
RUN dotnet build "./Nfc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Nfc.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Nfc.Api.dll"]