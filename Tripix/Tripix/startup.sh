#!/bin/bash
echo "Starting Monster Deployment..."

dotnet restore
dotnet build --configuration Release
dotnet publish --configuration Release --output out
dotnet out/Tripix.dll
