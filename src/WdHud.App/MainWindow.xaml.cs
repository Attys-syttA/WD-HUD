using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Input;
using WdHud.Contracts;
using WdHud.Core;
using WdHud.Infrastructure;
using Forms = System.Windows.Forms;
using Drawing = System.Drawing;

namespace WdHud.App;

public partial class MainWindow : Window
{
    private readonly HudViewModel viewModel;
    private readonly IStartupRegistrationService? startupRegistrationService;
    private readonly Forms.NotifyIcon trayIcon;
    private bool isExiting;

    public MainWindow()
    {
        InitializeComponent();

        var formatter = new HudMetricsFormatter();
        var settingsValidator = new HudSettingsValidator();
        var settingsStore = new JsonSettingsStore(settingsValidator);
        var provider = new LibreHardwareMonitorMetricsProvider(new GpuSelectionPolicy(), new HudMetricsNormalizer());
        var behaviorService = new WindowBehaviorService();
        if (!string.IsNullOrWhiteSpace(Environment.ProcessPath))
        {
            startupRegistrationService = new WindowsStartupRegistrationService(Environment.ProcessPath);
        }

        viewModel = new HudViewModel(formatter, settingsStore, provider);
        DataContext = viewModel;
        trayIcon = CreateTrayIcon();

        Loaded += OnLoaded;
        Closing += OnClosing;
        Closed += (_, _) => trayIcon.Dispose();
        SourceInitialized += (_, _) =>
        {
            var handle = new WindowInteropHelper(this).Handle;
            behaviorService.SetClickThrough(handle, viewModel.Settings.ClickThroughEnabled);
        };
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await viewModel.LoadAsync().ConfigureAwait(true);
        await EnsureStartupRegistrationAsync().ConfigureAwait(true);
        ApplyInitialPosition();
        Opacity = viewModel.Settings.Opacity;
        await viewModel.StartAsync().ConfigureAwait(true);
    }

    private async void OnClosing(object? sender, CancelEventArgs e)
    {
        if (!isExiting)
        {
            e.Cancel = true;
            MinimizeToTray();
            return;
        }

        viewModel.Stop();
        await SaveCurrentSettingsAsync().ConfigureAwait(true);
    }

    private void ApplyInitialPosition()
    {
        if (viewModel.Settings.WindowLeft >= 0 && viewModel.Settings.WindowTop >= 0)
        {
            Left = viewModel.Settings.WindowLeft;
            Top = viewModel.Settings.WindowTop;
            return;
        }

        Left = SystemParameters.WorkArea.Right - Width - 24;
        Top = SystemParameters.WorkArea.Top + 24;
    }

    private async Task EnsureStartupRegistrationAsync()
    {
        if (startupRegistrationService is null)
        {
            return;
        }

        startupRegistrationService.SetRegistered(true);
        if (!viewModel.Settings.StartWithWindows)
        {
            await SaveCurrentSettingsAsync(forceStartWithWindows: true).ConfigureAwait(true);
        }
    }

    private async Task SaveCurrentSettingsAsync(bool? forceStartWithWindows = null)
    {
        var current = viewModel.Settings;
        var settings = new HudSettings
        {
            WindowLeft = Left,
            WindowTop = Top,
            Opacity = Opacity,
            ClickThroughEnabled = current.ClickThroughEnabled,
            StartWithWindows = forceStartWithWindows ?? current.StartWithWindows,
            ShowTime = current.ShowTime,
            ShowCpuUsage = current.ShowCpuUsage,
            ShowRamUsage = current.ShowRamUsage,
            ShowGpuUsage = current.ShowGpuUsage,
            ShowCpuTemp = current.ShowCpuTemp,
            ShowGpuTemp = current.ShowGpuTemp,
            GpuSelectionMode = current.GpuSelectionMode
        };
        await viewModel.SaveAsync(settings).ConfigureAwait(true);
    }

    private Forms.NotifyIcon CreateTrayIcon()
    {
        var openItem = new Forms.ToolStripMenuItem("Open", null, (_, _) => RestoreFromTray());
        var exitItem = new Forms.ToolStripMenuItem("Exit", null, (_, _) => ExitApplication());
        var menu = new Forms.ContextMenuStrip();
        menu.Items.Add(openItem);
        menu.Items.Add(exitItem);

        var icon = new Forms.NotifyIcon
        {
            Icon = Drawing.SystemIcons.Application,
            Text = "WD-HUD",
            ContextMenuStrip = menu,
            Visible = true
        };
        icon.DoubleClick += (_, _) => RestoreFromTray();
        return icon;
    }

    private void OnMinimizeToTrayClicked(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        MinimizeToTray();
    }

    private void MinimizeToTray()
    {
        Hide();
        WindowState = WindowState.Minimized;
    }

    private void RestoreFromTray()
    {
        Show();
        WindowState = WindowState.Normal;
        Activate();
    }

    private void ExitApplication()
    {
        isExiting = true;
        Close();
    }

    private void OnHudMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ButtonState == MouseButtonState.Pressed)
        {
            DragMove();
        }
    }
}
