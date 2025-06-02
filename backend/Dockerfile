# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution file
COPY TeachPortal.sln .

# Copy project files for better caching
COPY src/TeachPortal.Domain/TeachPortal.Domain.csproj src/TeachPortal.Domain/
COPY src/TeachPortal.Application/TeachPortal.Application.csproj src/TeachPortal.Application/
COPY src/TeachPortal.Infrastructure/TeachPortal.Infrastructure.csproj src/TeachPortal.Infrastructure/
COPY src/TeachPortal.API/TeachPortal.API.csproj src/TeachPortal.API/
COPY tests/TeachPortal.Tests.Unit/TeachPortal.Tests.Unit.csproj tests/TeachPortal.Tests.Unit/

# Restore dependencies explicitly
RUN dotnet restore TeachPortal.sln

# Copy source code
COPY src/ src/
COPY tests/ tests/

# Build the application
RUN dotnet build TeachPortal.sln -c Release --no-restore

# Publish the application
RUN dotnet publish src/TeachPortal.API/TeachPortal.API.csproj -c Release -o /app/publish --no-build

# Use the official .NET 8 runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published application
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "TeachPortal.API.dll"] 