// ReSharper disable InconsistentNaming

namespace Chronos.Core.Native;

// Docs: https://docs.microsoft.com/en-us/windows/win32/winauto/event-constants
public enum WindowsEvents : uint
{
    /// <summary>
    ///     The lowest possible event.
    /// </summary>
    EVENT_MIN = 0x00000001,

    /// <summary>
    ///     The foreground window has changed. The system sends this event even if the foreground window has changed to another
    ///     window in
    ///     the same thread. Server applications never send this event.
    ///     For this event, the WinEventProc callback function's hwnd parameter is the handle to the window that is in the
    ///     foreground,
    ///     the idObject parameter is OBJID_WINDOW, and the idChild parameter is CHILDID_SELF.
    /// </summary>
    EVENT_SYSTEM_FOREGROUND = 0x0003,

    /// <summary>
    ///     A window has received mouse capture. This event is sent by the system, never by servers.
    /// </summary>
    EVENT_SYSTEM_CAPTURESTART = 0x0008,

    /// <summary>
    ///     A window has lost mouse capture. This event is sent by the system, never by servers.
    /// </summary>
    EVENT_SYSTEM_CAPTUREEND = 0x0009,

    /// <summary>
    ///     The highest possible event
    /// </summary>
    EVENT_MAX = 0x7FFFFFFF
}
