using System.IO;
using System.Text.Json;
using WdHud.Contracts;
using WdHud.Core;

namespace WdHud.Infrastructure;

public sealed class JsonSettingsStore : ISettingsStore
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly HudSettingsValidator validator;
    private readonly string settingsPath;

    public JsonSettingsStore(HudSettingsValidator validator)
        : this(validator, GetDefaultSettingsPath())
    {
    }

    public JsonSettingsStore(HudSettingsValidator validator, string settingsPath)
    {
        this.validator = validator;
        this.settingsPath = settingsPath;
    }

    public async Task<HudSettings> LoadAsync(CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath) ?? ".");

        if (!File.Exists(settingsPath))
        {
            await SaveAsync(HudSettings.Default, cancellationToken).ConfigureAwait(false);
            return HudSettings.Default;
        }

        try
        {
            await using var stream = File.OpenRead(settingsPath);
            var settings = await JsonSerializer.DeserializeAsync<HudSettings>(
                stream,
                SerializerOptions,
                cancellationToken).ConfigureAwait(false);

            return validator.Normalize(settings ?? HudSettings.Default);
        }
        catch (JsonException)
        {
            BackupCorruptSettings();
            await SaveAsync(HudSettings.Default, cancellationToken).ConfigureAwait(false);
            return HudSettings.Default;
        }
        catch (IOException)
        {
            return HudSettings.Default;
        }
    }

    public async Task SaveAsync(HudSettings settings, CancellationToken cancellationToken)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(settingsPath) ?? ".");

        var normalized = validator.Normalize(settings);
        var tempPath = $"{settingsPath}.tmp";

        await using (var stream = File.Create(tempPath))
        {
            await JsonSerializer.SerializeAsync(stream, normalized, SerializerOptions, cancellationToken)
                .ConfigureAwait(false);
        }

        File.Move(tempPath, settingsPath, overwrite: true);
    }

    private void BackupCorruptSettings()
    {
        if (!File.Exists(settingsPath))
        {
            return;
        }

        var backupPath = $"{settingsPath}.corrupt-{DateTime.UtcNow:yyyyMMddHHmmss}.bak";
        File.Move(settingsPath, backupPath, overwrite: false);
    }

    private static string GetDefaultSettingsPath()
    {
        var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "WD-HUD", "settings.json");
    }
}
