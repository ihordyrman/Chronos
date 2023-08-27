namespace Chronos.Core.Native;

public class WindowEventHandler
{
    // http://www.pinvoke.net/default.aspx/user32/WinEventDelegate.html
    public delegate void WinEventDelegate(
        nint hWinEventHook,
        uint eventType,
        nint hWnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime);
}
