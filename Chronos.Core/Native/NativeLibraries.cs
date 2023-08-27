using System.Runtime.InteropServices;
using System.Text;

namespace Chronos.Core.Native;

public static class NativeLibraries
{
    // https://www.pinvoke.net/default.aspx/user32.GetMessage
    [DllImport("user32.dll")]
    public static extern int GetMessage(out Msg lpMsg, nint hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    // https://www.pinvoke.net/default.aspx/user32.GetWindowText
    [DllImport("user32.dll")]
    public static extern int GetWindowText(nint hWnd, StringBuilder builder, int count);

    // https://www.pinvoke.net/default.aspx/user32.TranslateMessage
    [DllImport("user32.dll")]
    public static extern bool TranslateMessage([In] ref Msg lpMsg);

    // https://www.pinvoke.net/default.aspx/user32.DispatchMessage
    [DllImport("user32.dll")]
    public static extern nint DispatchMessage([In] ref Msg lpmsg);

    // https://www.pinvoke.net/default.aspx/user32.SetWinEventHook
    [DllImport("user32.dll")]
    public static extern nint SetWinEventHook(
        uint eventMin,
        uint eventMax,
        nint hmodWinEventProc,
        WindowEventHandler.WinEventDelegate? lpfnWinEventProc,
        uint idProcess,
        uint idThread,
        uint dwFlags);

    // https://www.pinvoke.net/default.aspx/user32.UnhookWinEvent
    [DllImport("user32.dll")]
    public static extern bool UnhookWinEvent(nint hWinEventHook);

    // https://www.pinvoke.net/default.aspx/user32.GetWindowThreadProcessId
    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(nint hWnd, ref uint lpdwProcessId);

    // https://www.pinvoke.net/default.aspx/user32.GetForegroundWindow
    [DllImport("user32.dll")]
    public static extern nint GetForegroundWindow();
}
