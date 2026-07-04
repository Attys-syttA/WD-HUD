namespace WdHud.Contracts;

public interface ISettingsStore
{
    Task<HudSettings> LoadAsync(CancellationToken cancellationToken);

    Task SaveAsync(HudSettings settings, CancellationToken cancellationToken);
}
