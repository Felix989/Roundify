# Roundify

~Description:

This is a lightweight Windows program that makes your screen's corners rounded, similar to Displaperture, but for Windows.

Roundify creates a transparent overlay around the edges of your monitor(s). The center of the screen remains visible while the corners and optional border are masked.

~Disclaimer:

The program is currently in an early Alpha state and is still under development.

The current version is designed to work with different resolutions, aspect ratios, multiple monitors, and different DPI settings.

A compiled/deployed EXE is not automatically installed into Windows. Roundify is currently a portable application, so you can place the published EXE anywhere you want.

If you want to build the project yourself you will need Windows and the .NET 10 SDK (or Visual Studio with the .NET desktop development workload).

~Usage:

Run `Roundify.exe` from the published folder.

Roundify starts in the system tray. Right-click the Roundify tray icon to:

- Enable or disable Roundify.
- Open Settings.
- Enable or disable Start with Windows.
- Exit Roundify.

In Settings you can change the corner radius, border thickness, and always-on-top behavior.

~Requirements:

For development/building:

- Windows 10 version 1809 or later, or Windows 11.
- .NET 10 SDK.
- Visual Studio with the .NET desktop development workload, or the .NET SDK command-line tools.

For the self-contained published version:

- No separate .NET runtime installation is required.

~Build / Deployment:

The repository contains two build scripts.

**`build-win-x64.cmd`**

This performs a normal Release build for Windows x64:

`dotnet build Roundify.csproj -c Release -r win-x64 --self-contained false`

The build output is placed under:

`bin\\Release\\net10.0-windows\\win-x64\\`

The executable is normally:

`bin\\Release\\net10.0-windows\\win-x64\\Roundify.exe`

This is intended primarily for development/testing. Because it is **not self-contained**, the target PC needs the compatible .NET desktop runtime installed.

**`publish-win-x64.cmd`**

This creates the recommended portable deployment:

`dotnet publish Roundify.csproj -c Release -r win-x64 --self-contained true`

It also enables single-file publishing and several Release optimizations.

The final executable is:

`publish\\win-x64\\Roundify.exe`

This published version is self-contained, so users do **not** need to install .NET separately.

You can also publish manually with:

`dotnet publish Roundify.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -p:EnableCompressionInSingleFile=true -p:PublishReadyToRun=true -p:PublishTrimmed=false -p:DebugType=None -o publish\\win-x64`

~Installation:

There is currently no MSI/EXE installer. Roundify is portable.

For example, you can extract the published executable to:

`C:\\Apps\\Roundify\\Roundify.exe`

or:

`%LOCALAPPDATA%\\Programs\\Roundify\\Roundify.exe`

Running the EXE does not copy files into `C:\\Program Files` and does not require administrator privileges.

~Auto launch at startup:

Roundify has a **Start with Windows** option in the tray menu and Settings window.

When enabled, Roundify creates a per-user Windows startup entry at:

`HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run`

The value name is:

`Roundify`

The value points to the current `Roundify.exe` path.

This means Roundify launches automatically when the current Windows user logs in. Administrator privileges are not required for this per-user startup entry.

If you move `Roundify.exe` after enabling startup, the stored startup path can become invalid. Move the EXE first, then disable and re-enable **Start with Windows** so the registry entry is updated.

~Configuration:

Roundify stores its user configuration in:

`%LOCALAPPDATA%\\Roundify\\settings.json`

On a typical installation this is:

`C:\\Users\\<YourUserName>\\AppData\\Local\\Roundify\\settings.json`

The file contains settings such as:

- Enabled/disabled state.
- Corner radius.
- Border thickness.
- Always-on-top.
- Start with Windows.

The configuration is created automatically when Roundify saves settings for the first time.

~How to remove Roundify:

Roundify currently has no traditional uninstaller because it is a portable application.

1. Exit Roundify from the system tray.
2. Disable **Start with Windows** before deleting the executable.
3. Delete the folder containing `Roundify.exe`.
4. Optionally delete `%LOCALAPPDATA%\\Roundify` to remove your settings.

If you already deleted the EXE and forgot to disable startup, remove the startup entry with PowerShell:

`Remove-ItemProperty -Path "HKCU:\\Software\\Microsoft\\Windows\\CurrentVersion\\Run" -Name "Roundify" -ErrorAction SilentlyContinue`

Or remove the `Roundify` value manually from:

`HKEY_CURRENT_USER\\Software\\Microsoft\\Windows\\CurrentVersion\\Run`

~Performance:

Roundify is designed to use very little CPU while idle.

The overlay does not continuously poll the desktop with a timer. Overlay windows are updated when settings change and when Windows reports a display configuration change.

The application also uses a single-instance mutex, per-monitor overlay windows, and WPF geometry rather than a continuously animated bitmap.

Actual CPU and memory usage will vary depending on Windows version, number of monitors, DPI settings, and whether the Settings window is open.

~Features:

Implemented:

- Application icon.
- Run invisibly in the background.
- System tray controls.
- Low-CPU event-driven overlay updates.
- Adjustable corner radius.
- Adjustable border thickness.
- Start with Windows.
- JSON configuration file.
- Multiple monitor support.
- More screen resolutions and aspect ratios.
- Per-monitor DPI scaling.
- Optional always-on-top overlay.
- Single-instance protection.
- Self-contained Windows x64 publishing.

Features to be added:

- Custom border color.
- Border transparency/opacity.
- Per-application exclusions.
- Better fullscreen/game handling.
- Custom overlay profiles.
- Global hotkeys.
- More advanced Settings UI.
- Installer/uninstaller.
- Automatic updates.
- Additional performance tuning.

~Remarks:

Please, if you can help develop my project or have any ideas, open an Issue ticket or make a Pull Request with a detailed description of your code, what you changed, why you changed it, and how it was tested.

Please keep the project lightweight and avoid unnecessary dependencies.

The project intentionally does **not** run as a Windows Service. The overlay has to run in the user's interactive desktop session, so **Start with Windows** launches the normal Roundify tray application when the user logs in.

~Project files:

- `Roundify.sln` - Visual Studio solution.
- `Roundify.csproj` - Main WPF project.
- `build-win-x64.cmd` - Normal Release x64 build.
- `publish-win-x64.cmd` - Self-contained single-file Windows x64 deployment.
- `check-dotnet.cmd` - Diagnostic helper for checking the installed .NET SDK/workload environment.
- `global.json` - Pins the project to SDK 10.0.401 with latest-patch roll-forward.
- `%LOCALAPPDATA%\\Roundify\\settings.json` - User configuration created at runtime.

~Troubleshooting:

If you see errors such as:

`MSB4276: The default SDK resolver failed to resolve SDK "Microsoft.NET.SDK.WorkloadAutoImportPropsLocator"`

or:

`MSB6011: Invalid parameters passed to the Microsoft.NET.Build.Tasks.RunCsWinRTGenerator task`

run:

`check-dotnet.cmd`

If the workload resolver directory is missing, repair the .NET SDK/workload installation before building.

Roundify itself does not require WinUI, Windows App SDK, or CsWinRT packages.

The project uses `global.json` so Visual Studio and the command line use the intended .NET SDK rather than silently selecting a different SDK.