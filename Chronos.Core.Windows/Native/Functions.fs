namespace Chronos.Core.Windows.Native

open System.Runtime.InteropServices
open System.Text

module NativeFunctions =

    // https://www.pinvoke.net/default.aspx/user32.GetMessage
    [<DllImport("user32.dll")>]
    extern int GetMessage(Msg& lpMsg, nativeint hWnd, uint32 wMsgFilterMin, uint32 wMsgFilterMax)

    // https://www.pinvoke.net/default.aspx/user32.GetWindowText
    [<DllImport("user32.dll")>]
    extern int GetWindowText(nativeint hWnd, StringBuilder builder, int count)

    // https://www.pinvoke.net/default.aspx/user32.TranslateMessage
    [<DllImport("user32.dll")>]
    extern bool TranslateMessage(Msg& lpMsg)

    // https://www.pinvoke.net/default.aspx/user32.DispatchMessage
    [<DllImport("user32.dll")>]
    extern nativeint DispatchMessage(Msg& lpmsg)

    // https://www.pinvoke.net/default.aspx/user32.SetWinEventHook
    [<DllImport("user32.dll")>]
    extern nativeint SetWinEventHook(
        uint32 eventMin,
        uint32 eventMax,
        nativeint hmodWinEventProc,
        System.Delegate lpfnWinEventProc,
        uint32 idProcess,
        uint32 idThread,
        uint32 dwFlags
    )

    // https://www.pinvoke.net/default.aspx/user32.UnhookWinEvent
    [<DllImport("user32.dll")>]
    extern bool UnhookWinEvent(nativeint hWinEventHook)

    // https://www.pinvoke.net/default.aspx/user32.GetWindowThreadProcessId
    [<DllImport("user32.dll")>]
    extern uint32 GetWindowThreadProcessId(nativeint hWnd, uint32& lpdwProcessId)

    // https://www.pinvoke.net/default.aspx/user32.GetForegroundWindow
    [<DllImport("user32.dll")>]
    extern nativeint GetForegroundWindow()
