using System.ComponentModel;
using System.Windows;

namespace Roundify;

public partial class MainWindow : Window
{
    private readonly RoundifySettings _settings;
    private readonly Action _apply;
    private bool _initializing = true;
    private bool _allowApplicationClose;

    public MainWindow(RoundifySettings settings, Action apply)
    {
        InitializeComponent();

        _settings = settings;
        _apply = apply;

        EnabledCheckBox.IsChecked = settings.Enabled;
        RadiusSlider.Value = settings.CornerRadius;
        BorderSlider.Value = settings.BorderThickness;
        TopmostCheckBox.IsChecked = settings.AlwaysOnTop;
        StartupCheckBox.IsChecked = settings.StartWithWindows;

        UpdateLabels();
        _initializing = false;
    }

    private void RadiusSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        if (_initializing)
            return;

        _settings.CornerRadius = (int)Math.Round(RadiusSlider.Value);
        UpdateLabels();
        _apply();
    }

    private void BorderSlider_ValueChanged(
        object sender,
        RoutedPropertyChangedEventArgs<double> e)
    {
        if (_initializing)
            return;

        _settings.BorderThickness = (int)Math.Round(BorderSlider.Value);
        UpdateLabels();
        _apply();
    }

    private void SettingChanged(object sender, RoutedEventArgs e)
    {
        if (_initializing)
            return;

        _settings.Enabled = EnabledCheckBox.IsChecked == true;
        _settings.AlwaysOnTop = TopmostCheckBox.IsChecked == true;
        _settings.StartWithWindows = StartupCheckBox.IsChecked == true;

        _apply();
    }

    private void UpdateLabels()
    {
        RadiusText.Text = $"{(int)Math.Round(RadiusSlider.Value)} px";
        BorderText.Text = $"{(int)Math.Round(BorderSlider.Value)} px";
    }

    public void AllowApplicationClose()
    {
        _allowApplicationClose = true;
    }

    protected override void OnClosing(CancelEventArgs e)
    {
        if (!_allowApplicationClose)
        {
            e.Cancel = true;
            Hide();
            SettingsStore.Save(_settings);
            return;
        }

        SettingsStore.Save(_settings);
        base.OnClosing(e);
    }
}
