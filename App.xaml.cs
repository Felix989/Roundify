using System.IO;
using System.Windows;
using System.Windows.Resources;
using Application = System.Windows.Application;

namespace Roundify;

public partial class App : Application
{
    private const string MutexName = "Local\\Roundify.SingleInstance";

    private Mutex? _mutex;
    private NotifyIcon? _trayIcon;
    private OverlayManager? _overlayManager;
    private MainWindow? _settingsWindow;
    private bool _isExiting;

    public RoundifySettings Settings { get; private set; } = new();

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _mutex = new Mutex(true, MutexName, out bool createdNew);

        if (!createdNew)
        {
            _mutex.Dispose();
            _mutex = null;
            Shutdown();
            return;
        }

        Settings = SettingsStore.Load();

        StartupManager.SetEnabled(Settings.StartWithWindows);

        _overlayManager = new OverlayManager();
        _overlayManager.Apply(Settings);

        CreateTrayIcon();
    }

    private void CreateTrayIcon()
    {
        _trayIcon = new NotifyIcon
        {
            Text = "Roundify - rounded screen corners",
            Visible = true,
            Icon = LoadIcon()
        };

        _trayIcon.DoubleClick += (_, _) => ShowSettings();

        ContextMenuStrip menu = new();

        ToolStripMenuItem enabledItem = new("Enable Roundify")
        {
            Checked = Settings.Enabled,
            CheckOnClick = true
        };
        enabledItem.CheckedChanged += (_, _) =>
        {
            Settings.Enabled = enabledItem.Checked;
            ApplySettings(save: true);
        };

        ToolStripMenuItem settingsItem = new("Settings...");
        settingsItem.Click += (_, _) => ShowSettings();

        ToolStripMenuItem startupItem = new("Start with Windows")
        {
            Checked = Settings.StartWithWindows,
            CheckOnClick = true
        };
        startupItem.CheckedChanged += (_, _) =>
        {
            Settings.StartWithWindows = startupItem.Checked;
            StartupManager.SetEnabled(Settings.StartWithWindows);
            SettingsStore.Save(Settings);
        };

        ToolStripMenuItem exitItem = new("Exit");
        exitItem.Click += (_, _) => ExitApplication();

        menu.Items.Add(enabledItem);
        menu.Items.Add(settingsItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(startupItem);
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add(exitItem);

        _trayIcon.ContextMenuStrip = menu;
    }

    private static Icon LoadIcon()
    {
        Uri uri = new("pack://application:,,,/Assets/roundify.ico");
        StreamResourceInfo? resource = GetResourceStream(uri);

        if (resource?.Stream == null)
            return SystemIcons.Application;

        using Stream stream = resource.Stream;
        using MemoryStream temp = new();
        stream.CopyTo(temp);
        temp.Position = 0;
        using Icon icon = new(temp);
        return (Icon)icon.Clone();
    }

    private void ShowSettings()
    {
        if (_settingsWindow == null)
        {
            _settingsWindow = new MainWindow(
                Settings,
                () => ApplySettings(save: false));
        }

        if (!_settingsWindow.IsVisible)
            _settingsWindow.Show();

        _settingsWindow.WindowState = WindowState.Normal;
        _settingsWindow.Activate();
        _settingsWindow.Topmost = true;
        _settingsWindow.Topmost = false;
        _settingsWindow.Focus();
    }

    public void ApplySettings(bool save)
    {
        _overlayManager?.Apply(Settings);

        if (Settings.StartWithWindows)
            StartupManager.SetEnabled(true);
        else
            StartupManager.SetEnabled(false);

        if (save)
            SettingsStore.Save(Settings);

        UpdateTrayChecks();
    }

    private void UpdateTrayChecks()
    {
        if (_trayIcon?.ContextMenuStrip == null)
            return;

        foreach (ToolStripItem item in _trayIcon.ContextMenuStrip.Items)
        {
            if (item is not ToolStripMenuItem menuItem)
                continue;

            if (menuItem.Text == "Enable Roundify")
                menuItem.Checked = Settings.Enabled;
            else if (menuItem.Text == "Start with Windows")
                menuItem.Checked = Settings.StartWithWindows;
        }
    }

    private void ExitApplication()
    {
        if (_isExiting)
            return;

        _isExiting = true;
        SettingsStore.Save(Settings);

        _settingsWindow?.AllowApplicationClose();
        _settingsWindow?.Close();

        Shutdown();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        _isExiting = true;
        SettingsStore.Save(Settings);

        _overlayManager?.Dispose();
        _trayIcon?.Dispose();
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        _mutex = null;

        base.OnExit(e);
    }
}
