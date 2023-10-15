namespace Chronos.Core.Windows.Native


// https://docs.microsoft.com/en-us/windows/win32/api/winuser/ns-winuser-msg
[<Struct>]
type Msg =
    { Hwnd: nativeint
      Message: uint32
      WParam: nativeint
      LParam: nativeint
      Time: uint32
      Pt: Point }
