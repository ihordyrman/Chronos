namespace Chronos.Core.Native;

// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msg
public struct Msg
{
    public nint Hwnd;
    public uint Message;
    public nint WParam;
    public nint LParam;
    public uint Time;
    public Point Pt;
}
