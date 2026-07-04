namespace WdHud.Contracts;

public sealed record HudMetricsSnapshot(
    DateTime LocalTime,
    DateTime TimestampUtc,
    double CpuUsagePercent,
    double RamUsagePercent,
    double? GpuUsagePercent,
    double? CpuTemperatureC,
    double? GpuTemperatureC,
    string? SelectedGpuName,
    bool IsGpuDiscrete)
{
    public static HudMetricsSnapshot Empty(DateTime localTime) => new(
        LocalTime: localTime,
        TimestampUtc: localTime.ToUniversalTime(),
        CpuUsagePercent: 0,
        RamUsagePercent: 0,
        GpuUsagePercent: null,
        CpuTemperatureC: null,
        GpuTemperatureC: null,
        SelectedGpuName: null,
        IsGpuDiscrete: false);
}
