using WdHud.Contracts;

namespace WdHud.Core;

public sealed class GpuSelectionPolicy
{
    public GpuCandidate? Select(IEnumerable<GpuCandidate> candidates, GpuSelectionMode mode)
    {
        var list = candidates
            .Where(candidate => candidate.UsagePercent.HasValue || candidate.TemperatureC.HasValue)
            .ToList();

        if (list.Count == 0)
        {
            return null;
        }

        return mode switch
        {
            GpuSelectionMode.FirstAvailable => list[0],
            GpuSelectionMode.IntegratedPreferred => list.FirstOrDefault(candidate => !candidate.IsDiscrete) ?? list[0],
            GpuSelectionMode.DiscretePreferred => list.FirstOrDefault(candidate => candidate.IsDiscrete) ?? list[0],
            _ => list.FirstOrDefault(candidate => candidate.IsDiscrete) ?? list[0]
        };
    }
}
