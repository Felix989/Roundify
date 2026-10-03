using Microsoft.Win32;
using System.IO;

namespace Roundify;

public static class StartupManager
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "Roundify";

    public static void SetEnabled(bool enabled)
    {
        // When running under `dotnet run`, ProcessPath points to dotnet.exe rather than Roundify.exe.
        // Never write a broken startup entry for development runs.
        string? exePath = Environment.ProcessPath;

        if (string.IsNullOrWhiteSpace(exePath))
            return;

        string fileName = Path.GetFileName(exePath);

        if (!fileName.Equals("Roundify.exe", StringComparison.OrdinalIgnoreCase))
            return;

        try
        {
            using RegistryKey key = Registry.CurrentUser.CreateSubKey(RunKey, writable: true);

            if (enabled)
            {
                key.SetValue(AppName, $"\"{exePath}\"");
            }
            else
            {
                key.DeleteValue(AppName, throwOnMissingValue: false);
            }
        }
        catch
        {
            // HKCU should normally be writable without elevation, but failure here must not kill Roundify.
        }
    }
}
