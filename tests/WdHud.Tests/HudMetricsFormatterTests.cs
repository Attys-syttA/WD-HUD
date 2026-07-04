using WdHud.Core;
using Xunit;

namespace WdHud.Tests;

public sealed class HudMetricsFormatterTests
{
    private readonly HudMetricsFormatter formatter = new();

    [Fact]
    public void FormatPercent_RoundsAndAddsPercentSign()
    {
        Assert.Equal("19%", formatter.FormatPercent(18.6));
    }

    [Fact]
    public void FormatTemperature_RoundsAndAddsUnit()
    {
        Assert.Equal("58\u00B0C", formatter.FormatTemperature(57.7));
    }

    [Fact]
    public void FormatNullValues_ReturnsNotAvailable()
    {
        Assert.Equal("N/A", formatter.FormatPercent(null));
        Assert.Equal("N/A", formatter.FormatTemperature(null));
    }
}
