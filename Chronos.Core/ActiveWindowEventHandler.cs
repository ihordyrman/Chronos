using Chronos.Core.Native;

namespace Chronos.Core;

public class ActiveWindowEventHandler : IDisposable
{
    private nint innerHandle = IntPtr.Zero;
    private WindowEventHandler.WinEventDelegate? procDelegate;

    public bool Hooked { get; set; }

    public void Dispose()
    {
        if (Hooked)
        {
            NativeLibraries.UnhookWinEvent(innerHandle);
            procDelegate = null;
            innerHandle = IntPtr.Zero;
        }
    }

    public event EventHandler<WindowArgs>? Hook;

    public void MakeAHook()
    {
        procDelegate = OnEvent;
        innerHandle = NativeLibraries.SetWinEventHook(
            (uint)WindowsEvents.EVENT_SYSTEM_FOREGROUND,
            (uint)WindowsEvents.EVENT_SYSTEM_FOREGROUND,
            IntPtr.Zero,
            procDelegate,
            0u,
            0u,
            0x0003);

        Hooked = true;
    }

    public void OnEvent(
        nint hWinEventHook,
        uint eventType,
        nint hWnd,
        int idObject,
        int idChild,
        uint dwEventThread,
        uint dwmsEventTime)
        => Hook.Invoke(
            this,
            new WindowArgs(hWinEventHook, eventType, hWnd, idObject, idChild, dwEventThread, dwmsEventTime));
}
