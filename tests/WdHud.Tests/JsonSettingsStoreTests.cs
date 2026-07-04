using WdHud.Contracts;
using WdHud.Core;
using WdHud.Infrastructure;
using Xunit;

namespace WdHud.Tests;

public sealed class JsonSettingsStoreTests
{
    [Fact]
    public async Task LoadAsync_CorruptJson_BacksUpAndReturnsDefault()
    {
        var directory = Path.Combine(Path.GetTempPath(), $"wdhud-{Guid.NewGuid():N}");
        Directory.CreateDirectory(directory);
        var settingsPath = Path.Combine(directory, "settings.json");
        await File.WriteAllTextAsync(settingsPath, "{ broken json");

        var store = new JsonSettingsStore(new HudSettingsValidator(), settingsPath);
        var settings = await store.LoadAsync(CancellationToken.None);

        Assert.Equal(HudSettings.Default.Opacity, settings.Opacity);
        Assert.True(Directory.GetFiles(directory, "settings.json.corrupt-*.bak").Length == 1);
    }
}
