# Building and publishing

## Visual Studio

Open `Roundify.sln`.

Select `Release` and `x64`, then **Build > Build Solution**.

## Command line

From the project folder:

```powershell
dotnet restore
dotnet build .\Roundify.csproj -c Release -r win-x64
```

## Self-contained single-file EXE

```powershell
.\publish-win-x64.cmd
```

Output:

```text
publish\win-x64\Roundify.exe
```

## Debugging

When debugging from Visual Studio, the app starts directly to the tray. Double-click the tray icon (or right-click it and choose Settings) to open the settings window.
