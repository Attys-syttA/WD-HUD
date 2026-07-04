namespace WdHud.Contracts;

public interface IStartupRegistrationService
{
    bool IsRegistered();

    void SetRegistered(bool enabled);
}
