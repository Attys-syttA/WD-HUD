using System.Runtime.InteropServices;
using WdHud.Contracts;

namespace WdHud.Infrastructure;

public sealed class WindowBehaviorService : IWindowBehaviorService
{
    private const int GwlExstyle = -20;
    private const int WsExTransparent = 0x00000020;
    private const int WsExLayered = 0x00080000;

    public void SetClickThrough(IntPtr windowHandle, bool enabled)
    {
        if (windowHandle == IntPtr.Zero)
        {
            return;
        }

        var style = GetWindowLong(windowHandle, GwlExstyle);
        var desired = enabled
            ? style | WsExTransparent | WsExLayered
            : style & ~WsExTransparent;

        if (desired != style)
        {
            SetWindowLong(windowHandle, GwlExstyle, desired);
        }
    }

    [DllImport("user32.dll")]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
}
