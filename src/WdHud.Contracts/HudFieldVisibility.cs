namespace WdHud.Contracts;

public sealed record HudFieldVisibility(
    bool ShowTime,
    bool ShowCpuUsage,
    bool ShowRamUsage,
    bool ShowGpuUsage,
    bool ShowCpuTemp,
    bool ShowGpuTemp)
{
    public static HudFieldVisibility Default { get; } = new(
        ShowTime: true,
        ShowCpuUsage: true,
        ShowRamUsage: true,
        ShowGpuUsage: true,
        ShowCpuTemp: true,
        ShowGpuTemp: true);
}
