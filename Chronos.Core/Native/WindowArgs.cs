namespace Chronos.Core.Native;

public class WindowArgs : EventArgs
{
    public WindowArgs(
        nint hookHandle,
        uint eventType,
        nint windowHandle,
        int objectId,
        int childId,
        uint eventThreadId,
        uint eventTime)
    {
        Handle = hookHandle;
        EventType = eventType;
        WindowHandle = windowHandle;
        ObjectId = objectId;
        ChildId = childId;
        EventThreadId = eventThreadId;
        EventTime = eventTime;
    }

    public nint Handle { get; }

    public uint EventType { get; }

    public nint WindowHandle { get; }

    public int ObjectId { get; }

    public int ChildId { get; }

    public uint EventThreadId { get; }

    public uint EventTime { get; }
}
