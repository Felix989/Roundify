@echo off
setlocal
cd /d "%~dp0"

set OUTPUT=publish\win-x64
if exist "%OUTPUT%" rmdir /s /q "%OUTPUT%"

 echo Publishing Roundify as a self-contained Windows x64 executable...
dotnet publish Roundify.csproj -c Release -r win-x64 --self-contained true ^
  -p:PublishSingleFile=true ^
  -p:IncludeNativeLibrariesForSelfExtract=true ^
  -p:EnableCompressionInSingleFile=true ^
  -p:PublishReadyToRun=true ^
  -p:PublishTrimmed=false ^
  -p:DebugType=None ^
  -o "%OUTPUT%"

if errorlevel 1 (
  echo.
  echo Publish failed.
  exit /b 1
)

echo.
echo Published executable:
echo %CD%\%OUTPUT%\Roundify.exe
endlocal
