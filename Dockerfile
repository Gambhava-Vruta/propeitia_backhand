# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /source

# Copy csproj and restore
COPY *.csproj .
RUN dotnet restore

# Copy source and publish
COPY . .
RUN dotnet publish -c Release -o /app

# Run Stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Set Render PORT environment variables
ENV ASPNETCORE_URLS=http://+:80
# Reduce memory usage on Render free tier (512MB) to prevent OOM
ENV DOTNET_gcServer=0
ENV DOTNET_EnableDiagnostics=0
# Use invariant globalization to save ~30MB
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false
EXPOSE 80

ENTRYPOINT ["dotnet", "Propertia.dll"]
