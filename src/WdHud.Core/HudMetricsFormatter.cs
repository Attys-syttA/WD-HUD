using System.Globalization;

namespace WdHud.Core;

public sealed class HudMetricsFormatter
{
    public string FormatTime(DateTime value) => value.ToString("HH:mm", CultureInfo.InvariantCulture);

    public string FormatPercent(double? value)
    {
        if (!value.HasValue)
        {
            return "N/A";
        }

        return $"{Math.Round(Clamp(value.Value, 0, 100)):0}%";
    }

    public string FormatTemperature(double? value)
    {
        if (!value.HasValue)
        {
            return "N/A";
        }

        return $"{Math.Round(value.Value):0}\u00B0C";
    }

    private static double Clamp(double value, double minimum, double maximum)
        => Math.Min(Math.Max(value, minimum), maximum);
}
