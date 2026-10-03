@echo off
setlocal
cd /d "%~dp0"

echo Building Roundify (Release, x64)...
dotnet build Roundify.csproj -c Release -r win-x64 --self-contained false
if errorlevel 1 (
  echo.
  echo Build failed.
  exit /b 1
)

echo.
echo Build completed. Output is under bin\Release\net10.0-windows\win-x64\
endlocal
