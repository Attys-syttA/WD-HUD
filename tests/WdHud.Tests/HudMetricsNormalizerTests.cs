using WdHud.Contracts;
using WdHud.Core;
using Xunit;

namespace WdHud.Tests;

public sealed class HudMetricsNormalizerTests
{
    private readonly HudMetricsNormalizer normalizer = new();

    [Fact]
    public void Normalize_ClampsPercentValues()
    {
        var snapshot = new HudMetricsSnapshot(
            DateTime.Now,
            DateTime.UtcNow,
            -10,
            150,
            120,
            55,
            65,
            "GPU",
            true);

        var normalized = normalizer.Normalize(snapshot);

        Assert.Equal(0, normalized.CpuUsagePercent);
        Assert.Equal(100, normalized.RamUsagePercent);
        Assert.Equal(100, normalized.GpuUsagePercent);
    }

    [Fact]
    public void Normalize_RemovesNonFiniteValues()
    {
        var snapshot = new HudMetricsSnapshot(
            DateTime.Now,
            DateTime.UtcNow,
            double.NaN,
            double.PositiveInfinity,
            double.NegativeInfinity,
            double.NaN,
            double.PositiveInfinity,
            "GPU",
            true);

        var normalized = normalizer.Normalize(snapshot);

        Assert.Equal(0, normalized.CpuUsagePercent);
        Assert.Equal(0, normalized.RamUsagePercent);
        Assert.Null(normalized.GpuUsagePercent);
        Assert.Null(normalized.CpuTemperatureC);
        Assert.Null(normalized.GpuTemperatureC);
    }
}
