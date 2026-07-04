namespace WdHud.Core;

public sealed class HudMetricStatusPolicy
{
    public HudMetricSeverity ClassifyLoad(double? percent)
    {
        if (!percent.HasValue)
        {
            return HudMetricSeverity.Unknown;
        }

        var value = Clamp(percent.Value, 0, 100);
        return value switch
        {
            < 20 => HudMetricSeverity.Cool,
            < 70 => HudMetricSeverity.Optimal,
            < 90 => HudMetricSeverity.Warm,
            _ => HudMetricSeverity.Critical
        };
    }

    public HudMetricSeverity ClassifyTemperature(double? celsius)
    {
        if (!celsius.HasValue)
        {
            return HudMetricSeverity.Unknown;
        }

        return celsius.Value switch
        {
            < 45 => HudMetricSeverity.Cool,
            < 75 => HudMetricSeverity.Optimal,
            < 85 => HudMetricSeverity.Warm,
            _ => HudMetricSeverity.Critical
        };
    }

    private static double Clamp(double value, double minimum, double maximum)
        => Math.Min(Math.Max(value, minimum), maximum);
}
