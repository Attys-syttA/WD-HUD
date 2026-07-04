namespace WdHud.Contracts;

public interface IWindowBehaviorService
{
    void SetClickThrough(IntPtr windowHandle, bool enabled);
}
