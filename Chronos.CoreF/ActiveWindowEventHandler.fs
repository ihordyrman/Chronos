namespace Chronos.Core.Native

open System
open Chronos.Core.Native
open FSharp.Control

type ActiveWindowEventHandler() =
    let mutable procDelegate : WinEventDelegate option = None
    let mutable innerHandle = nativeint 0
    let mutable hooked = false
    let hookEvent = Event<WindowArgs>()

    member this.Hooked with get() = hooked and set value = hooked <- value

    member this.MakeAHook() =
        let onEvent hWinEventHook eventType hWnd idObject idChild dwEventThread dwmsEventTime =
            hookEvent.Trigger(WindowArgs(hWinEventHook, eventType, hWnd, idObject, idChild, dwEventThread, dwmsEventTime))

        procDelegate <- Some onEvent
        innerHandle <- NativeFunctions.SetWinEventHook(
                        uint32 WindowsEvents.EVENT_SYSTEM_FOREGROUND,
                        uint32 WindowsEvents.EVENT_SYSTEM_FOREGROUND,
                        nativeint 0,
                        procDelegate.Value,
                        uint32 0,
                        uint32 0,
                        uint32 0x0003)
        hooked <- true

    // todo: need to figure out how to convert this
    // member this.Hook
    //     with add() handler = hookEvent.Publish.AddHandler handler

    // member this.Hook
    //     with add handler = hookEvent.AddHandler handler
    //     and remove handler = hookEvent.RemoveHandler handler


    interface IDisposable with
        member this.Dispose() =
            if hooked then
                NativeFunctions.UnhookWinEvent(innerHandle) |> ignore
                procDelegate <- None
                innerHandle <- nativeint 0
