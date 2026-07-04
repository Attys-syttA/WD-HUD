using Microsoft.Win32;
using WdHud.Contracts;

namespace WdHud.Infrastructure;

public sealed class WindowsStartupRegistrationService : IStartupRegistrationService
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "WD-HUD";
    private readonly string executablePath;

    public WindowsStartupRegistrationService(string executablePath)
    {
        this.executablePath = executablePath;
    }

    public bool IsRegistered()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: false);
        var value = key?.GetValue(ValueName) as string;
        return string.Equals(value, Quote(executablePath), StringComparison.OrdinalIgnoreCase);
    }

    public void SetRegistered(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, writable: true)
            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath, writable: true);

        if (enabled)
        {
            key.SetValue(ValueName, Quote(executablePath), RegistryValueKind.String);
            return;
        }

        key.DeleteValue(ValueName, throwOnMissingValue: false);
    }

    private static string Quote(string path) => $"\"{path}\"";
}
