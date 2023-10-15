namespace Chronos.Core.Windows.Native

// http://www.pinvoke.net/default.aspx/user32/WinEventDelegate.html
type WinEventDelegate =
    delegate of
        hWinEventHook: nativeint *
        eventType: uint32 *
        hwnd: nativeint *
        idObject: int *
        idChild: int *
        dwEventThread: uint32 *
        dwmsEventTime: uint32 ->
            unit
