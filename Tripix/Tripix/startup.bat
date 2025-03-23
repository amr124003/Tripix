@echo off
echo 🚀 Running .NET Build on Monster...

REM حذف أي DLLs قديمة
rmdir /s /q out

REM استرجاع الـ Dependencies
dotnet restore

REM بناء المشروع وتحويله لـ DLLs
dotnet build --configuration Release

REM نشر المشروع في مجلد out
dotnet publish --configuration Release --output out

REM تشغيل الـ API
dotnet out\Tripix.dll
