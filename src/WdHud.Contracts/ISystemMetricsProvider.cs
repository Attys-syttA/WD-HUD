namespace WdHud.Contracts;

public interface ISystemMetricsProvider
{
    Task<HudMetricsSnapshot> GetSnapshotAsync(GpuSelectionMode gpuSelectionMode, CancellationToken cancellationToken);
}
