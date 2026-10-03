using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Roundify;

public partial class OverlayWindow : Window
{
    private IntPtr _hwnd;
    private RoundifySettings _settings = new();

    public OverlayWindow()
    {
        InitializeComponent();
        SourceInitialized += OnSourceInitialized;
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        _hwnd = new WindowInteropHelper(this).Handle;

        IntPtr current = NativeMethods.GetWindowLongPtr(_hwnd, NativeMethods.GWL_EXSTYLE);
        long style = current.ToInt64();

        style |= NativeMethods.WS_EX_TOOLWINDOW;
        style |= NativeMethods.WS_EX_NOACTIVATE;
        style |= NativeMethods.WS_EX_TRANSPARENT;

        NativeMethods.SetWindowLongPtr(_hwnd, NativeMethods.GWL_EXSTYLE, new IntPtr(style));

        HwndSource? source = HwndSource.FromHwnd(_hwnd);
        source?.AddHook(WndProc);
    }

    private IntPtr WndProc(
        IntPtr hwnd,
        int msg,
        IntPtr wParam,
        IntPtr lParam,
        ref bool handled)
    {
        if (msg == NativeMethods.WM_NCHITTEST)
        {
            handled = true;
            return new IntPtr(NativeMethods.HTTRANSPARENT);
        }

        return IntPtr.Zero;
    }

    public void UpdateScreen(
        System.Drawing.Rectangle bounds,
        RoundifySettings settings)
    {
        _settings = settings;
        Topmost = settings.AlwaysOnTop;

        if (_hwnd == IntPtr.Zero)
        {
            Left = bounds.Left;
            Top = bounds.Top;
            Width = bounds.Width;
            Height = bounds.Height;
        }
        else
        {
            IntPtr insertAfter = settings.AlwaysOnTop
                ? NativeMethods.HWND_TOPMOST
                : NativeMethods.HWND_TOP;

            NativeMethods.SetWindowPos(
                _hwnd,
                insertAfter,
                bounds.Left,
                bounds.Top,
                bounds.Width,
                bounds.Height,
                NativeMethods.SWP_NOACTIVATE |
                NativeMethods.SWP_SHOWWINDOW);
        }

        UpdateMask();
    }

    private void UpdateMask()
    {
        if (_hwnd == IntPtr.Zero || ActualWidth <= 0 || ActualHeight <= 0)
            return;

        uint dpiValue = NativeMethods.GetDpiForWindow(_hwnd);
        double scale = dpiValue == 0 ? 1.0 : dpiValue / 96.0;

        double width = ActualWidth;
        double height = ActualHeight;

        double radius = Math.Max(0, _settings.CornerRadius) / scale;
        double border = Math.Max(0, _settings.BorderThickness) / scale;

        border = Math.Min(border, Math.Min(width, height) / 2.0);

        double innerWidth = Math.Max(0, width - border * 2);
        double innerHeight = Math.Max(0, height - border * 2);

        double innerRadius = Math.Max(0, radius - border);
        innerRadius = Math.Min(innerRadius, Math.Min(innerWidth, innerHeight) / 2.0);

        Geometry outer = new RectangleGeometry(
            new Rect(0, 0, width, height));

        Geometry inner = new RectangleGeometry(
            new Rect(border, border, innerWidth, innerHeight),
            innerRadius,
            innerRadius);

        Mask.Data = new CombinedGeometry(
            GeometryCombineMode.Exclude,
            outer,
            inner);
    }
}
