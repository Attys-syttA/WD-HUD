using WdHud.Contracts;
using WdHud.Core;
using Xunit;

namespace WdHud.Tests;

public sealed class HudSettingsValidatorTests
{
    [Fact]
    public void Normalize_ClampsOpacity()
    {
        var validator = new HudSettingsValidator();

        var low = validator.Normalize(new HudSettings { Opacity = 0.01 });
        var high = validator.Normalize(new HudSettings { Opacity = 2.0 });

        Assert.Equal(0.25, low.Opacity);
        Assert.Equal(1.0, high.Opacity);
    }

    [Fact]
    public void Default_StartsWithWindows()
    {
        Assert.True(HudSettings.Default.StartWithWindows);
    }
}
