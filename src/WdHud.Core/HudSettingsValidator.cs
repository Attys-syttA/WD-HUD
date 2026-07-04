using WdHud.Contracts;

namespace WdHud.Core;

public sealed class HudSettingsValidator
{
    public HudSettings Normalize(HudSettings settings)
    {
        return new HudSettings
        {
            WindowLeft = settings.WindowLeft,
            WindowTop = settings.WindowTop,
            Opacity = Math.Min(Math.Max(settings.Opacity, 0.25), 1.0),
            ClickThroughEnabled = settings.ClickThroughEnabled,
            StartWithWindows = settings.StartWithWindows,
            ShowTime = settings.ShowTime,
            ShowCpuUsage = settings.ShowCpuUsage,
            ShowRamUsage = settings.ShowRamUsage,
            ShowGpuUsage = settings.ShowGpuUsage,
            ShowCpuTemp = settings.ShowCpuTemp,
            ShowGpuTemp = settings.ShowGpuTemp,
            GpuSelectionMode = Enum.IsDefined(settings.GpuSelectionMode) ? settings.GpuSelectionMode : GpuSelectionMode.Auto
        };
    }
}
