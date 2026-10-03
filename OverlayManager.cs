using Microsoft.Win32;
using Application = System.Windows.Application;
using FormsScreen = System.Windows.Forms.Screen;

namespace Roundify;

public sealed class OverlayManager : IDisposable
{
    private readonly Dictionary<string, OverlayWindow> _overlays = new(StringComparer.OrdinalIgnoreCase);
    private bool _disposed;
    private bool _refreshing;

    public OverlayManager()
    {
        SystemEvents.DisplaySettingsChanged += DisplaySettingsChanged;
    }

    public void Apply(RoundifySettings settings)
    {
        if (_disposed || _refreshing)
            return;

        _refreshing = true;

        try
        {
            if (!settings.Enabled)
            {
                CloseAll();
                return;
            }

            FormsScreen[] screens = FormsScreen.AllScreens;
            HashSet<string> activeNames = new(
                screens.Select(static screen => screen.DeviceName),
                StringComparer.OrdinalIgnoreCase);

            foreach (FormsScreen screen in screens)
            {
                string key = screen.DeviceName;

                if (!_overlays.TryGetValue(key, out OverlayWindow? overlay))
                {
                    overlay = new OverlayWindow();
                    _overlays.Add(key, overlay);
                    overlay.Show();
                }

                overlay.UpdateScreen(screen.Bounds, settings);
            }

            foreach (string key in _overlays.Keys.Where(key => !activeNames.Contains(key)).ToArray())
            {
                _overlays[key].Close();
                _overlays.Remove(key);
            }
        }
        finally
        {
            _refreshing = false;
        }
    }

    private void DisplaySettingsChanged(object? sender, EventArgs e)
    {
        if (_disposed)
            return;

        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
        {
            if (_disposed)
                return;

            if (Application.Current is App app)
                Apply(app.Settings);
        }));
    }

    private void CloseAll()
    {
        foreach (OverlayWindow overlay in _overlays.Values)
            overlay.Close();

        _overlays.Clear();
    }

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;
        SystemEvents.DisplaySettingsChanged -= DisplaySettingsChanged;
        CloseAll();
    }
}
