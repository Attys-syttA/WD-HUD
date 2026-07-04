using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;
using WdHud.Contracts;
using WdHud.Core;
using WdHud.Infrastructure;

namespace WdHud.App;

public partial class MainWindow : Window
{
    private readonly HudViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();

        var formatter = new HudMetricsFormatter();
        var settingsValidator = new HudSettingsValidator();
        var settingsStore = new JsonSettingsStore(settingsValidator);
        var provider = new LibreHardwareMonitorMetricsProvider(new GpuSelectionPolicy(), new HudMetricsNormalizer());
        var behaviorService = new WindowBehaviorService();

        viewModel = new HudViewModel(formatter, settingsStore, provider);
        DataContext = viewModel;

        Loaded += OnLoaded;
        Closing += OnClosing;
        SourceInitialized += (_, _) =>
        {
            var handle = new WindowInteropHelper(this).Handle;
            behaviorService.SetClickThrough(handle, viewModel.Settings.ClickThroughEnabled);
        };
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        await viewModel.LoadAsync().ConfigureAwait(true);
        ApplyInitialPosition();
        Opacity = viewModel.Settings.Opacity;
        await viewModel.StartAsync().ConfigureAwait(true);
    }

    private async void OnClosing(object? sender, CancelEventArgs e)
    {
        viewModel.Stop();
        var current = viewModel.Settings;
        var settings = new HudSettings
        {
            WindowLeft = Left,
            WindowTop = Top,
            Opacity = Opacity,
            ClickThroughEnabled = current.ClickThroughEnabled,
            StartWithWindows = current.StartWithWindows,
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
}
