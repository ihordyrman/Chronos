namespace Chronos.Core.Windows.Native

// Docs: https://docs.microsoft.com/en-us/windows/win32/winauto/event-constants
type WindowsEvents =
    | EVENT_MIN = 0x00000001u
    | EVENT_SYSTEM_FOREGROUND = 0x0003u
    | EVENT_SYSTEM_CAPTURESTART = 0x0008u
    | EVENT_SYSTEM_CAPTUREEND = 0x0009u
    | EVENT_MAX = 0x7FFFFFFFu
