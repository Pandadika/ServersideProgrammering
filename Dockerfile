#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY ["WebApp/WebApp.csproj", "WebApp/"]
COPY ["WebApi/WebApi.csproj", "WebApi/"]
COPY ["Shared/Shared.csproj", "Shared/"]

RUN dotnet restore "./WebApp/./WebApp.csproj"
RUN dotnet restore "./WebApi/./WebApi.csproj"
COPY . .

WORKDIR "/src/WebApp"
RUN dotnet build "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/build
WORKDIR "/src/WebApi"
RUN dotnet build "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish-app
WORKDIR "/src/WebApp"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApp.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM build AS publish-api
WORKDIR "/src/WebApi"
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final-app
WORKDIR /app
USER root
RUN mkdir -p /app/data && chown -R app:app /app/data
RUN mkdir -p /app/keys && chown -R app:app /app/keys 
USER app
COPY --from=publish-app /app/publish .
ENTRYPOINT ["dotnet", "WebApp.dll"]

FROM base AS final-api
WORKDIR /app
USER root
RUN mkdir -p /app/data && chown -R app:app /app/data
RUN mkdir -p /app/keys && chown -R app:app /app/keys 
USER app
COPY --from=publish-api /app/publish .
ENTRYPOINT ["dotnet", "WebApi.dll"]
