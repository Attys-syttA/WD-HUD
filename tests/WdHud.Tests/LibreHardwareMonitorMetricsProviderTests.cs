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
}
