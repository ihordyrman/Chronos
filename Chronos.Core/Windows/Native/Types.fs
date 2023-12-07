namespace Chronos.Core.Windows.Native

open System

// https://docs.microsoft.com/en-us/previous-versions/dd162805(v=vs.85)
[<Struct>]
type Point =
    { X: int64 // X coordinate
      Y: int64 } // Y coordinate

// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msg
[<Struct>]
type Msg =
    { Hwnd: nativeint
      Message: uint32
      WParam: nativeint
      LParam: nativeint
      Time: uint32
      Pt: Point }

// https://docs.microsoft.com/en-us/windows/win32/winauto/event-constants
type WindowsEvents =
    | EVENT_MIN = 0x00000001u
    | EVENT_SYSTEM_FOREGROUND = 0x0003u
    | EVENT_SYSTEM_CAPTURESTART = 0x0008u
    | EVENT_SYSTEM_CAPTUREEND = 0x0009u
    | EVENT_MAX = 0x7FFFFFFFu

// https://learn.microsoft.com/en-us/previous-versions/windows/desktop/legacy/ms633498(v=vs.85)
type EnumWindowsProc = delegate of nativeint * nativeint -> bool

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


type WindowArgs
    (
        handle: nativeint,
        eventType: uint32,
        windowHandle: nativeint,
        objectId: int,
        childId: int,
        eventThreadId: uint32,
        eventTime: uint32
    ) =

    inherit EventArgs()

    member val Handle = handle with get
    member val EventType = eventType with get
    member val WindowHandle = windowHandle with get
    member val ObjectId = objectId with get
    member val ChildId = childId with get
    member val EventThreadId = eventThreadId with get
    member val EventTime = eventTime with get
