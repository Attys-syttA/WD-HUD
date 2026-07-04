namespace WdHud.Contracts;

public sealed class HudSettings
{
    public double WindowLeft { get; init; } = -1;
    public double WindowTop { get; init; } = -1;
    public double Opacity { get; init; } = 0.82;
    public bool ClickThroughEnabled { get; init; }
    public bool StartWithWindows { get; init; }
    public bool ShowTime { get; init; } = true;
    public bool ShowCpuUsage { get; init; } = true;
    public bool ShowRamUsage { get; init; } = true;
    public bool ShowGpuUsage { get; init; } = true;
    public bool ShowCpuTemp { get; init; } = true;
    public bool ShowGpuTemp { get; init; } = true;
    public GpuSelectionMode GpuSelectionMode { get; init; } = GpuSelectionMode.Auto;

    public HudFieldVisibility Visibility => new(
        ShowTime,
        ShowCpuUsage,
        ShowRamUsage,
        ShowGpuUsage,
        ShowCpuTemp,
        ShowGpuTemp);

    public static HudSettings Default { get; } = new();
}
