# Base image for the application
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY/MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY.csproj", "MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY/"]
RUN dotnet restore "MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY/MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY.csproj"
COPY . .
WORKDIR "/src/MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY"
RUN dotnet build "MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY.csproj" -c Release -o /app/publish /p:UseAppHost=false

# EF CLI Tools install stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS ef-tools
RUN dotnet tool install --global dotnet-ef --version 7.0.0
ENV PATH="$PATH:/root/.dotnet/tools"

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=ef-tools /root/.dotnet /root/.dotnet
COPY --from=ef-tools /root/.nuget /root/.nuget
ENV PATH="$PATH:/root/.dotnet/tools"
ENTRYPOINT ["dotnet", "MASTER-PROJECT-IN-LAYERED-ARCHITECTURE-GENERIC-REPOSITORY.dll"]
