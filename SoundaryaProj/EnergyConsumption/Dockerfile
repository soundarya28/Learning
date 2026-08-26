FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /src

# Copy project files while preserving their real structure
COPY ["SoundaryaProj/EnergyConsumption/EnergyConsumption.csproj", "SoundaryaProj/EnergyConsumption/"]
COPY ["SoundaryaProj/Models/Models.csproj", "SoundaryaProj/Models/"]

# Restore
RUN dotnet restore "SoundaryaProj/EnergyConsumption/EnergyConsumption.csproj"

# Copy source code
COPY SoundaryaProj/EnergyConsumption/ SoundaryaProj/EnergyConsumption/
COPY SoundaryaProj/Models/ SoundaryaProj/Models/

# Publish
RUN dotnet publish "SoundaryaProj/EnergyConsumption/EnergyConsumption.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore


FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "EnergyConsumption.dll"]
