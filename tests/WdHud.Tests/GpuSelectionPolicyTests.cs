using WdHud.Contracts;
using WdHud.Core;
using Xunit;

namespace WdHud.Tests;

public sealed class GpuSelectionPolicyTests
{
    private readonly GpuSelectionPolicy policy = new();

    [Fact]
    public void Auto_PrefersDiscreteGpu()
    {
        var selected = policy.Select(
            [
                new GpuCandidate("AMD Radeon iGPU", false, 10, 44),
                new GpuCandidate("NVIDIA GeForce RTX 5080", true, 20, 55)
            ],
            GpuSelectionMode.Auto);

        Assert.Equal("NVIDIA GeForce RTX 5080", selected?.Name);
    }

    [Fact]
    public void IntegratedPreferred_PrefersIntegratedGpu()
    {
        var selected = policy.Select(
            [
                new GpuCandidate("NVIDIA GeForce RTX 5080", true, 20, 55),
                new GpuCandidate("AMD Radeon iGPU", false, 10, 44)
            ],
            GpuSelectionMode.IntegratedPreferred);

        Assert.Equal("AMD Radeon iGPU", selected?.Name);
    }

    [Fact]
    public void MissingValues_ReturnsNull()
    {
        var selected = policy.Select(
            [new GpuCandidate("Unknown GPU", true, null, null)],
            GpuSelectionMode.FirstAvailable);

        Assert.Null(selected);
    }
}
