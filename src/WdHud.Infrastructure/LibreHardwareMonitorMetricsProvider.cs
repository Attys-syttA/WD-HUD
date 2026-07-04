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
            IsMemoryEnabled = true,
            IsMotherboardEnabled = true,
            IsControllerEnabled = true
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
        computer.Accept(UpdateVisitor.Instance);

        var cpuUsage = 0d;
        var ramUsage = 0d;
        double? cpuTemperature = null;
        var gpuCandidates = new List<GpuCandidate>();

        foreach (var hardware in computer.Hardware)
        {
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
                        IsDiscreteGpu(hardware.HardwareType, hardware.Name),
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
        return EnumerateSensors(hardware)
            .Where(sensor => sensor.SensorType == SensorType.Load && sensor.Value.HasValue)
            .OrderByDescending(sensor => IsTotalSensor(sensor.Name))
            .Select(sensor => (double?)sensor.Value!.Value)
            .FirstOrDefault();
    }

    private static double? ReadTemperature(IHardware hardware)
    {
        return SelectTemperature(
            EnumerateSensors(hardware)
            .Where(sensor => sensor.SensorType == SensorType.Temperature && sensor.Value.HasValue)
            .Select(sensor => (sensor.Name, (double?)sensor.Value!.Value)));
    }

    internal static double? SelectTemperature(IEnumerable<(string Name, double? Value)> sensors)
    {
        return sensors
            .Where(sensor => sensor.Value is > 0 && double.IsFinite(sensor.Value.Value))
            .OrderByDescending(sensor => TemperatureSensorPriority(sensor.Name))
            .Select(sensor => sensor.Value)
            .FirstOrDefault();
    }

    private static IEnumerable<ISensor> EnumerateSensors(IHardware hardware)
    {
        foreach (var sensor in hardware.Sensors)
        {
            yield return sensor;
        }

        foreach (var subHardware in hardware.SubHardware)
        {
            foreach (var sensor in EnumerateSensors(subHardware))
            {
                yield return sensor;
            }
        }
    }

    internal static bool IsDiscreteGpu(HardwareType hardwareType, string hardwareName)
    {
        if (hardwareType == HardwareType.GpuNvidia)
        {
            return true;
        }

        if (hardwareType == HardwareType.GpuAmd)
        {
            var normalizedName = NormalizeGpuName(hardwareName);
            return !normalizedName.Contains("integrated", StringComparison.OrdinalIgnoreCase)
                && !normalizedName.Contains("radeon graphics", StringComparison.OrdinalIgnoreCase);
        }

        return false;
    }

    private static string NormalizeGpuName(string value)
        => value
            .Replace("(TM)", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("(R)", string.Empty, StringComparison.OrdinalIgnoreCase);

    private static bool IsTotalSensor(string name)
        => name.Contains("total", StringComparison.OrdinalIgnoreCase)
            || name.Contains("core", StringComparison.OrdinalIgnoreCase)
            || name.Contains("memory", StringComparison.OrdinalIgnoreCase);

    private static int TemperatureSensorPriority(string name)
    {
        if (name.Contains("tctl", StringComparison.OrdinalIgnoreCase)
            || name.Contains("tdie", StringComparison.OrdinalIgnoreCase)
            || name.Contains("package", StringComparison.OrdinalIgnoreCase)
            || name.Contains("gpu core", StringComparison.OrdinalIgnoreCase))
        {
            return 3;
        }

        if (name.Contains("core", StringComparison.OrdinalIgnoreCase)
            || name.Contains("cpu", StringComparison.OrdinalIgnoreCase)
            || name.Contains("gpu", StringComparison.OrdinalIgnoreCase))
        {
            return 2;
        }

        return 1;
    }

    private sealed class UpdateVisitor : IVisitor
    {
        public static readonly UpdateVisitor Instance = new();

        private UpdateVisitor()
        {
        }

        public void VisitComputer(IComputer computer)
        {
            computer.Traverse(this);
        }

        public void VisitHardware(IHardware hardware)
        {
            hardware.Update();

            foreach (var subHardware in hardware.SubHardware)
            {
                subHardware.Accept(this);
            }
        }

        public void VisitParameter(IParameter parameter)
        {
        }

        public void VisitSensor(ISensor sensor)
        {
        }
    }
}
