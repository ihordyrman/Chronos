namespace Chronos.Core.Native

open System

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
