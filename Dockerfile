# #See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install the root certificate
RUN apt-get update && apt-get install -y ca-certificates && \
    update-ca-certificates

# Copy the root certificate and install it into the trusted store
COPY rootCA.crt /usr/local/share/ca-certificates/rootCA.crt
RUN chmod 644 /usr/local/share/ca-certificates/rootCA.crt && \
    update-ca-certificates

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Serversideprogrammering.App/Serversideprogrammering.App.csproj", "Serversideprogrammering.App/"]
RUN dotnet restore "./Serversideprogrammering.App/./Serversideprogrammering.App.csproj"
COPY . .
WORKDIR "/src/Serversideprogrammering.App"
RUN dotnet build "./Serversideprogrammering.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Serversideprogrammering.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

#FROM base AS final
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh .
RUN sed 's/\x0D$//' -i entrypoint.sh
RUN chmod a+rx entrypoint.sh

RUN dotnet tool install --global dotnet-ef
ENV PATH="$PATH:/root/.dotnet/tools"

# ENTRYPOINT ["./entrypoint.sh"]

#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["Serversideprogrammering.App/Serversideprogrammering.App.csproj", "Serversideprogrammering.App/"]
# RUN dotnet restore "./Serversideprogrammering.App/Serversideprogrammering.App.csproj"
# COPY . .
# WORKDIR "/src/Serversideprogrammering.App"
# RUN dotnet build "./Serversideprogrammering.App.csproj" -c $BUILD_CONFIGURATION -o /app/build

# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./Serversideprogrammering.App.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# FROM mcr.microsoft.com/dotnet/sdk:8.0 AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# COPY Serversideprogrammering.App/Serversideprogrammering.App.csproj .
# COPY entrypoint.sh .
# RUN sed 's/\x0D$//' -i entrypoint.sh
# RUN chmod a+rx entrypoint.sh

# # Switch to root user to install dotnet-ef tool
# USER root
# RUN dotnet tool install --global dotnet-ef
# ENV PATH="$PATH:/root/.dotnet/tools"

# # Switch back to app user
# USER app

#ENTRYPOINT ["./entrypoint.sh"]

ENTRYPOINT ["dotnet", "Serversideprogrammering.App.dll"]
