#!/bin/bash

# Build script for TeachPortal backend
echo "Building TeachPortal backend..."

# Clean previous builds
echo "Cleaning previous builds..."
rm -rf publish/

# Restore dependencies
echo "Restoring dependencies..."
dotnet restore TeachPortal.sln

# Build the application
echo "Building application..."
dotnet build TeachPortal.sln -c Release

# Publish the application
echo "Publishing application..."
dotnet publish src/TeachPortal.API/TeachPortal.API.csproj -c Release -o publish/

echo "Build completed! Ready for Docker deployment."
echo "Run: docker-compose build backend" 