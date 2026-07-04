using WdHud.Contracts;

namespace WdHud.Core;

public sealed record GpuCandidate(
    string Name,
    bool IsDiscrete,
    double? UsagePercent,
    double? TemperatureC)
{
    public HudMetricsSnapshot ToSnapshot(DateTime localTime, double cpuUsagePercent, double ramUsagePercent)
        => new(
            LocalTime: localTime,
            TimestampUtc: localTime.ToUniversalTime(),
            CpuUsagePercent: cpuUsagePercent,
            RamUsagePercent: ramUsagePercent,
            GpuUsagePercent: UsagePercent,
            CpuTemperatureC: null,
            GpuTemperatureC: TemperatureC,
            SelectedGpuName: Name,
            IsGpuDiscrete: IsDiscrete);
}
