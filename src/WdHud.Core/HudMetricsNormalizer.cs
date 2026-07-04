using WdHud.Contracts;

namespace WdHud.Core;

public sealed class HudMetricsNormalizer
{
    public HudMetricsSnapshot Normalize(HudMetricsSnapshot snapshot)
    {
        return snapshot with
        {
            CpuUsagePercent = Clamp(snapshot.CpuUsagePercent),
            RamUsagePercent = Clamp(snapshot.RamUsagePercent),
            GpuUsagePercent = ClampNullable(snapshot.GpuUsagePercent)
        };
    }

    private static double Clamp(double value) => Math.Min(Math.Max(value, 0), 100);

    private static double? ClampNullable(double? value) => value.HasValue ? Clamp(value.Value) : null;
}
