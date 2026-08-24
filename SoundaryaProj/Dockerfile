# ============================
# 1. Build Stage
# ============================
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy the project file
COPY ["EnergyConsumption.csproj", "./"]

# Restore NuGet packages
RUN dotnet restore "EnergyConsumption.csproj"

# Copy the remaining source code
COPY . .

# Build and publish the application
RUN dotnet publish "EnergyConsumption.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


# ============================
# 2. Runtime Stage
# ============================
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

# Copy published application from build stage
COPY --from=build /app/publish .

# Application listens on port 8080
EXPOSE 8080

# Start the application
ENTRYPOINT ["dotnet", "EnergyConsumption.dll"]