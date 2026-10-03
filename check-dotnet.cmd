@echo off
setlocal
cd /d "%~dp0"

echo === .NET SDK ===
dotnet --info
echo.
echo === SDK version selected by global.json ===
dotnet --version
echo.
echo === Workloads ===
dotnet workload list
echo.
echo === Workload resolver directory ===
for /f "usebackq delims=" %%S in (`dotnet --version`) do set SDKVER=%%S
if exist "%ProgramFiles%\dotnet\sdk\%SDKVER%\Sdks\Microsoft.NET.SDK.WorkloadAutoImportPropsLocator\Sdk" (
  echo OK: WorkloadAutoImportPropsLocator is present.
) else (
  echo MISSING: Microsoft.NET.SDK.WorkloadAutoImportPropsLocator\Sdk
  echo.
  echo Your .NET SDK installation is incomplete. Run:
  echo   dotnet workload repair
  echo or reinstall the .NET 10 SDK.
)
endlocal
