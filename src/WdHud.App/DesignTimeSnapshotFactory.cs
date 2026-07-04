using WdHud.Contracts;

namespace WdHud.App;

public static class DesignTimeSnapshotFactory
{
    public static HudMetricsSnapshot Create()
    {
        var now = DateTime.Now;
        return new HudMetricsSnapshot(
            LocalTime: now,
            TimestampUtc: now.ToUniversalTime(),
            CpuUsagePercent: 19,
            RamUsagePercent: 41,
            GpuUsagePercent: 12,
            CpuTemperatureC: 58,
            GpuTemperatureC: 43,
            SelectedGpuName: "Synthetic GPU",
            IsGpuDiscrete: true);
    }
}
