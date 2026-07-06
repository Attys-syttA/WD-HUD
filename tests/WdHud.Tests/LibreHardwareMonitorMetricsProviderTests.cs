using LibreHardwareMonitor.Hardware;
using WdHud.Infrastructure;
using Xunit;

namespace WdHud.Tests;

public sealed class LibreHardwareMonitorMetricsProviderTests
{
    [Fact]
    public void IsDiscreteGpu_ClassifiesNvidiaAsDiscrete()
    {
        Assert.True(LibreHardwareMonitorMetricsProvider.IsDiscreteGpu(
            HardwareType.GpuNvidia,
            "NVIDIA GeForce RTX 5080"));
    }

    [Fact]
    public void IsDiscreteGpu_ClassifiesAmdRadeonTmGraphicsAsIntegrated()
    {
        Assert.False(LibreHardwareMonitorMetricsProvider.IsDiscreteGpu(
            HardwareType.GpuAmd,
            "AMD Radeon(TM) Graphics"));
    }

    [Fact]
    public void IsDiscreteGpu_ClassifiesNamedIntegratedAmdAsIntegrated()
    {
        Assert.False(LibreHardwareMonitorMetricsProvider.IsDiscreteGpu(
            HardwareType.GpuAmd,
            "AMD Radeon Integrated Graphics"));
    }

    [Fact]
    public void SelectTemperature_IgnoresZeroAndNonFiniteValues()
    {
        var selected = LibreHardwareMonitorMetricsProvider.SelectTemperature(
            [
                ("Core (Tctl/Tdie)", 0),
                ("CPU", double.NaN),
                ("Package", double.PositiveInfinity)
            ]);

        Assert.Null(selected);
    }

    [Fact]
    public void SelectTemperature_PrefersPackageStyleSensor()
    {
        var selected = LibreHardwareMonitorMetricsProvider.SelectTemperature(
            [
                ("Ambient", 30),
                ("Core (Tctl/Tdie)", 61)
            ]);

        Assert.Equal(61, selected);
    }

    [Theory]
    [InlineData("CPU Package", true)]
    [InlineData("CPU PECI", true)]
    [InlineData("CPU", true)]
    [InlineData("Motherboard", false)]
    [InlineData("Ambient", false)]
    public void IsCpuTemperatureFallbackSensor_MatchesOnlyCpuLikeNames(string sensorName, bool expected)
    {
        Assert.Equal(expected, LibreHardwareMonitorMetricsProvider.IsCpuTemperatureFallbackSensor(sensorName));
    }

    [Fact]
    public void SelectTemperature_ChoosesBestCpuFallbackSensor()
    {
        var selected = LibreHardwareMonitorMetricsProvider.SelectTemperature(
            new (string, double?)[]
            {
                ("Motherboard", 33),
                ("CPU Package", 68),
                ("CPU PECI", 65)
            }
            .Where(sensor => LibreHardwareMonitorMetricsProvider.IsCpuTemperatureFallbackSensor(sensor.Item1)));

        Assert.Equal(68, selected);
    }
}
