using WdHud.Core;
using Xunit;

namespace WdHud.Tests;

public sealed class HudMetricStatusPolicyTests
{
    private readonly HudMetricStatusPolicy policy = new();

    [Fact]
    public void ClassifyLoad_Null_ReturnsUnknown()
    {
        Assert.Equal(HudMetricSeverity.Unknown, policy.ClassifyLoad(null));
    }

    [Theory]
    [InlineData(10.0, HudMetricSeverity.Cool)]
    [InlineData(50.0, HudMetricSeverity.Optimal)]
    [InlineData(80.0, HudMetricSeverity.Warm)]
    [InlineData(95.0, HudMetricSeverity.Critical)]
    public void ClassifyLoad_UsesExpectedThresholds(double value, HudMetricSeverity expected)
    {
        Assert.Equal(expected, policy.ClassifyLoad(value));
    }

    [Fact]
    public void ClassifyTemperature_Null_ReturnsUnknown()
    {
        Assert.Equal(HudMetricSeverity.Unknown, policy.ClassifyTemperature(null));
    }

    [Theory]
    [InlineData(35.0, HudMetricSeverity.Cool)]
    [InlineData(66.0, HudMetricSeverity.Optimal)]
    [InlineData(80.0, HudMetricSeverity.Warm)]
    [InlineData(90.0, HudMetricSeverity.Critical)]
    public void ClassifyTemperature_UsesExpectedThresholds(double value, HudMetricSeverity expected)
    {
        Assert.Equal(expected, policy.ClassifyTemperature(value));
    }
}
