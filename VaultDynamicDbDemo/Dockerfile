# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["VaultDynamicDbDemo.csproj", "."]
RUN dotnet restore "./VaultDynamicDbDemo.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./VaultDynamicDbDemo.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./VaultDynamicDbDemo.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

#COPY entrypoint.sh /entrypoint.sh
#ENTRYPOINT ["dotnet", "VaultDynamicDbDemo.dll"]

RUN echo '#!/usr/bin/env sh' > ./entrypoint.sh && \
    echo 'i=0' >> ./entrypoint.sh && \
    echo 'while [ ! -s /vault/secrets/db-secrets.json ]; do' >> ./entrypoint.sh && \
    echo '  printf "Waiting for secrets to be mounted... (%ds elapsed)\\r" "$i"' >> ./entrypoint.sh && \
    echo '  sleep 1' >> ./entrypoint.sh && \
    echo '  i=$((i + 1))' >> ./entrypoint.sh && \
    echo 'done' >> ./entrypoint.sh && \
    echo '' >> ./entrypoint.sh && \
    echo 'echo "Secrets are ready. Starting the ASP.NET application..."' >> ./entrypoint.sh && \
    echo 'exec dotnet "$APP_PATH"' >> ./entrypoint.sh && \
    chmod +x ./entrypoint.sh

USER notroot:notroot
ENV DOTNET_EnableDiagnostics=1
ENV DOTNET_EnableDiagnostics_IPC=0
ENV DOTNET_EnableDiagnostics_Debugger=0
ENV DOTNET_EnableDiagnostics_Profiler=1
ENV ASPNETCORE_HOSTBUILDER_RELOADCONFIGONCHANGE="false"
ENV APP_PATH="VaultDynamicDbDemo.dll"

ENTRYPOINT ["/app/entrypoint.sh"]
