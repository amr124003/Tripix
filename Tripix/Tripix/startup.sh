#!/bin/bash
echo "🚀 Running .NET Build on Monster..."

# احذف أي DLLs قديمة
rm -rf out/

# استرجاع الـ Dependencies
dotnet restore

# بناء المشروع وتحويله لـ DLLs
dotnet build --configuration Release

# نشر المشروع في مجلد `out`
dotnet publish --configuration Release --output out

# تشغيل الـ API
dotnet out/Tripix.dll
