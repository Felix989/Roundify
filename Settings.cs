using System.IO;
using System.Text.Json;

namespace Roundify;

public sealed class RoundifySettings
{
    public bool Enabled { get; set; } = true;
    public int CornerRadius { get; set; } = 30;
    public int BorderThickness { get; set; } = 0;
    public bool AlwaysOnTop { get; set; } = true;
    public bool StartWithWindows { get; set; } = false;
}

public static class SettingsStore
{
    private static readonly string Folder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "Roundify");

    private static readonly string FilePath = Path.Combine(Folder, "settings.json");

    public static RoundifySettings Load()
    {
        try
        {
            if (!File.Exists(FilePath))
                return new RoundifySettings();

            string json = File.ReadAllText(FilePath);
            return JsonSerializer.Deserialize<RoundifySettings>(json) ?? new RoundifySettings();
        }
        catch
        {
            return new RoundifySettings();
        }
    }

    public static void Save(RoundifySettings settings)
    {
        try
        {
            Directory.CreateDirectory(Folder);

            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(settings, options);
            File.WriteAllText(FilePath, json);
        }
        catch
        {
            // Settings are non-critical; never crash the tray utility because of a write failure.
        }
    }
}
