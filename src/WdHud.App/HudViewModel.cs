using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;
using WdHud.Contracts;
using WdHud.Core;

namespace WdHud.App;

public sealed class HudViewModel : INotifyPropertyChanged
{
    private readonly HudMetricsFormatter formatter;
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

    public HudViewModel(
        HudMetricsFormatter formatter,
        ISettingsStore settingsStore,
        ISystemMetricsProvider metricsProvider)
    {
        this.formatter = formatter;
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

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
