using LibreHardwareMonitor.Hardware;
using WdHud.Contracts;
using WdHud.Core;

namespace WdHud.Infrastructure;

public sealed class LibreHardwareMonitorMetricsProvider : ISystemMetricsProvider, IDisposable
{
    private readonly Computer computer;
    private readonly GpuSelectionPolicy gpuSelectionPolicy;
    private readonly HudMetricsNormalizer normalizer;
    private bool isOpen;
    private bool disposed;

    public LibreHardwareMonitorMetricsProvider(GpuSelectionPolicy gpuSelectionPolicy, HudMetricsNormalizer normalizer)
    {
        this.gpuSelectionPolicy = gpuSelectionPolicy;
        this.normalizer = normalizer;
        computer = new Computer
        {
            IsCpuEnabled = true,
            IsGpuEnabled = true,
            IsMemoryEnabled = true
        };
    }

    public Task<HudMetricsSnapshot> GetSnapshotAsync(GpuSelectionMode gpuSelectionMode, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var localTime = DateTime.Now;
        try
        {
            EnsureOpen();
            return Task.FromResult(ReadSnapshot(localTime, gpuSelectionMode));
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception)
        {
            return Task.FromResult(HudMetricsSnapshot.Empty(localTime));
        }
    }

    public void Dispose()
    {
        if (disposed)
        {
            return;
        }

        if (isOpen)
        {
            computer.Close();
        }

        disposed = true;
    }

    private HudMetricsSnapshot ReadSnapshot(DateTime localTime, GpuSelectionMode gpuSelectionMode)
    {
        var cpuUsage = 0d;
        var ramUsage = 0d;
        double? cpuTemperature = null;
        var gpuCandidates = new List<GpuCandidate>();

        foreach (var hardware in computer.Hardware)
        {
            hardware.Update();
            foreach (var subHardware in hardware.SubHardware)
            {
                subHardware.Update();
            }

            switch (hardware.HardwareType)
            {
                case HardwareType.Cpu:
                    cpuUsage = ReadLoad(hardware) ?? cpuUsage;
                    cpuTemperature = ReadTemperature(hardware) ?? cpuTemperature;
                    break;
                case HardwareType.Memory:
                    ramUsage = ReadLoad(hardware) ?? ramUsage;
                    break;
                case HardwareType.GpuNvidia:
                case HardwareType.GpuAmd:
                case HardwareType.GpuIntel:
                    gpuCandidates.Add(new GpuCandidate(
                        hardware.Name,
                        IsDiscreteGpu(hardware),
                        ReadLoad(hardware),
                        ReadTemperature(hardware)));
                    break;
            }
        }

        var selectedGpu = gpuSelectionPolicy.Select(gpuCandidates, gpuSelectionMode);
        var snapshot = new HudMetricsSnapshot(
            LocalTime: localTime,
            TimestampUtc: localTime.ToUniversalTime(),
            CpuUsagePercent: cpuUsage,
            RamUsagePercent: ramUsage,
            GpuUsagePercent: selectedGpu?.UsagePercent,
            CpuTemperatureC: cpuTemperature,
            GpuTemperatureC: selectedGpu?.TemperatureC,
            SelectedGpuName: selectedGpu?.Name,
            IsGpuDiscrete: selectedGpu?.IsDiscrete ?? false);

        return normalizer.Normalize(snapshot);
    }

    private void EnsureOpen()
    {
        if (isOpen)
        {
            return;
        }

        computer.Open();
        isOpen = true;
    }

    private static double? ReadLoad(IHardware hardware)
    {
        return hardware.Sensors
            .Where(sensor => sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
            .OrderByDescending(sensor => IsTotalSensor(sensor.Name))
            .Select(sensor => (double?)sensor.Value!.Value)
            .FirstOrDefault();
    }

    private static double? ReadTemperature(IHardware hardware)
    {
        return hardware.Sensors
            .Where(sensor => sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
            .OrderByDescending(sensor => IsPackageSensor(sensor.Name))
            .Select(sensor => (double?)sensor.Value!.Value)
            .FirstOrDefault();
    }

    private static bool IsDiscreteGpu(IHardware hardware)
    {
        if (hardware.HardwareType == HardwareType.GpuNvidia)
        {
            return true;
        }

        if (hardware.HardwareType == HardwareType.GpuAmd)
        {
            return !hardware.Name.Contains("integrated", StringComparison.OrdinalIgnoreCase)
                && !hardware.Name.Contains("radeon graphics", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static bool IsTotalSensor(string name)
        => name.Contains("total", StringComparison.OrdinalIgnoreCase)
            || name.Contains("core", StringComparison.OrdinalIgnoreCase)
            || name.Contains("memory", StringComparison.OrdinalIgnoreCase);

    private static bool IsPackageSensor(string name)
        => name.Contains("package", StringComparison.OrdinalIgnoreCase)
            || name.Contains("core", StringComparison.OrdinalIgnoreCase)
            || name.Contains("gpu", StringComparison.OrdinalIgnoreCase);
}
