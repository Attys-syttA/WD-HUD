using WdHud.Contracts;

namespace WdHud.Core;

public sealed class HudMetricsNormalizer
{
    public HudMetricsSnapshot Normalize(HudMetricsSnapshot snapshot)
    {
        return snapshot with
        {
            CpuUsagePercent = SanitizePercent(snapshot.CpuUsagePercent),
            RamUsagePercent = SanitizePercent(snapshot.RamUsagePercent),
            GpuUsagePercent = SanitizeNullablePercent(snapshot.GpuUsagePercent),
            CpuTemperatureC = SanitizeNullable(snapshot.CpuTemperatureC),
            GpuTemperatureC = SanitizeNullable(snapshot.GpuTemperatureC)
        };
    }

    private static double SanitizePercent(double value)
        => double.IsFinite(value) ? Math.Min(Math.Max(value, 0), 100) : 0;

    private static double? SanitizeNullablePercent(double? value)
        => value.HasValue && double.IsFinite(value.Value)
            ? Math.Min(Math.Max(value.Value, 0), 100)
            : null;

    private static double? SanitizeNullable(double? value)
        => value.HasValue && double.IsFinite(value.Value) ? value.Value : null;
}
