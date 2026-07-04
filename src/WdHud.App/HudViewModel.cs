using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using WdHud.Contracts;
using WdHud.Core;
using Media = System.Windows.Media;

namespace WdHud.App;

public sealed class HudViewModel : INotifyPropertyChanged
{
    private readonly HudMetricsFormatter formatter;
    private readonly HudMetricStatusPolicy statusPolicy;
    private readonly ISettingsStore settingsStore;
    private readonly ISystemMetricsProvider metricsProvider;
    private readonly DispatcherTimer timer;
    private HudSettings settings = HudSettings.Default;
    private string timeText = "--:--";
    private string cpuUsageText = "N/A";
    private string ramUsageText = "N/A";
    private string gpuUsageText = "N/A";
    private string cpuTempText = "N/A";
    private string gpuTempText = "N/A";
    private Media.Brush cpuUsageBrush = UnknownBrush;
    private Media.Brush ramUsageBrush = UnknownBrush;
    private Media.Brush gpuUsageBrush = UnknownBrush;
    private Media.Brush cpuTempBrush = UnknownBrush;
    private Media.Brush gpuTempBrush = UnknownBrush;

    private static readonly Media.Brush UnknownBrush = CreateBrush("#FFFFFFFF");
    private static readonly Media.Brush CoolBrush = CreateBrush("#FF65B8FF");
    private static readonly Media.Brush OptimalBrush = CreateBrush("#FF58D68D");
    private static readonly Media.Brush WarmBrush = CreateBrush("#FFFFB14A");
    private static readonly Media.Brush CriticalBrush = CreateBrush("#FFFF5A5F");

    public HudViewModel(
        HudMetricsFormatter formatter,
        ISettingsStore settingsStore,
        ISystemMetricsProvider metricsProvider)
    {
        this.formatter = formatter;
        statusPolicy = new HudMetricStatusPolicy();
        this.settingsStore = settingsStore;
        this.metricsProvider = metricsProvider;
        timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };
        timer.Tick += async (_, _) => await RefreshAsync().ConfigureAwait(true);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public HudSettings Settings => settings;

    public string TimeText
    {
        get => timeText;
        private set => SetField(ref timeText, value);
    }

    public string CpuUsageText
    {
        get => cpuUsageText;
        private set => SetField(ref cpuUsageText, value);
    }

    public string RamUsageText
    {
        get => ramUsageText;
        private set => SetField(ref ramUsageText, value);
    }

    public string GpuUsageText
    {
        get => gpuUsageText;
        private set => SetField(ref gpuUsageText, value);
    }

    public string CpuTempText
    {
        get => cpuTempText;
        private set => SetField(ref cpuTempText, value);
    }

    public string GpuTempText
    {
        get => gpuTempText;
        private set => SetField(ref gpuTempText, value);
    }

    public Media.Brush CpuUsageBrush
    {
        get => cpuUsageBrush;
        private set => SetField(ref cpuUsageBrush, value);
    }

    public Media.Brush RamUsageBrush
    {
        get => ramUsageBrush;
        private set => SetField(ref ramUsageBrush, value);
    }

    public Media.Brush GpuUsageBrush
    {
        get => gpuUsageBrush;
        private set => SetField(ref gpuUsageBrush, value);
    }

    public Media.Brush CpuTempBrush
    {
        get => cpuTempBrush;
        private set => SetField(ref cpuTempBrush, value);
    }

    public Media.Brush GpuTempBrush
    {
        get => gpuTempBrush;
        private set => SetField(ref gpuTempBrush, value);
    }

    public Visibility TimeVisibility => settings.ShowTime ? Visibility.Visible : Visibility.Collapsed;

    public async Task LoadAsync()
    {
        settings = await settingsStore.LoadAsync(CancellationToken.None).ConfigureAwait(true);
        OnPropertyChanged(nameof(Settings));
        OnPropertyChanged(nameof(TimeVisibility));
    }

    public async Task SaveAsync(HudSettings updatedSettings)
    {
        settings = updatedSettings;
        await settingsStore.SaveAsync(updatedSettings, CancellationToken.None).ConfigureAwait(true);
    }

    public async Task StartAsync()
    {
        await RefreshAsync().ConfigureAwait(true);
        timer.Start();
    }

    public void Stop() => timer.Stop();

    private async Task RefreshAsync()
    {
        var snapshot = await metricsProvider.GetSnapshotAsync(settings.GpuSelectionMode, CancellationToken.None)
            .ConfigureAwait(true);

        TimeText = formatter.FormatTime(snapshot.LocalTime);
        CpuUsageText = formatter.FormatPercent(snapshot.CpuUsagePercent);
        RamUsageText = formatter.FormatPercent(snapshot.RamUsagePercent);
        GpuUsageText = formatter.FormatPercent(snapshot.GpuUsagePercent);
        CpuTempText = formatter.FormatTemperature(snapshot.CpuTemperatureC);
        GpuTempText = formatter.FormatTemperature(snapshot.GpuTemperatureC);
        CpuUsageBrush = BrushFor(statusPolicy.ClassifyLoad(snapshot.CpuUsagePercent));
        RamUsageBrush = BrushFor(statusPolicy.ClassifyLoad(snapshot.RamUsagePercent));
        GpuUsageBrush = BrushFor(statusPolicy.ClassifyLoad(snapshot.GpuUsagePercent));
        CpuTempBrush = BrushFor(statusPolicy.ClassifyTemperature(snapshot.CpuTemperatureC));
        GpuTempBrush = BrushFor(statusPolicy.ClassifyTemperature(snapshot.GpuTemperatureC));
    }

    private void SetField(ref string field, string value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void SetField(ref Media.Brush field, Media.Brush value, [CallerMemberName] string? propertyName = null)
    {
        if (ReferenceEquals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private static Media.Brush BrushFor(HudMetricSeverity severity)
    {
        return severity switch
        {
            HudMetricSeverity.Cool => CoolBrush,
            HudMetricSeverity.Optimal => OptimalBrush,
            HudMetricSeverity.Warm => WarmBrush,
            HudMetricSeverity.Critical => CriticalBrush,
            _ => UnknownBrush
        };
    }

    private static Media.Brush CreateBrush(string color)
    {
        var brush = new Media.SolidColorBrush((Media.Color)Media.ColorConverter.ConvertFromString(color));
        brush.Freeze();
        return brush;
    }
}
