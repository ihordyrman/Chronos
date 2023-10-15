namespace Chronos.Core.Windows.Native

open System
open Chronos.Core.Windows.Native
open FSharp.Control

type ActiveWindowEventHandler() =
    let mutable procDelegate: WinEventDelegate option = None
    let mutable innerHandle = nativeint 0
    let mutable hooked = false
    let hook = Event<EventHandler<WindowArgs>, WindowArgs>()

    member this.Hook = hook.Publish

    member this.TriggerEvent(args: WindowArgs) = hook.Trigger(this, args)

    interface IDisposable with
        member this.Dispose() =
            if hooked then
                NativeFunctions.UnhookWinEvent(innerHandle) |> ignore
                procDelegate <- None
                innerHandle <- nativeint 0

    member this.Hooked
        with get () = hooked
        and set value = hooked <- value

    member this.MakeAHook() =
        let onEvent hWinEventHook eventType hWnd idObject idChild dwEventThread dwmsEventTime =
            this.TriggerEvent(
                WindowArgs(hWinEventHook, eventType, hWnd, idObject, idChild, dwEventThread, dwmsEventTime)
            )

        procDelegate <- Some onEvent

        innerHandle <-
            NativeFunctions.SetWinEventHook(
                uint32 WindowsEvents.EVENT_SYSTEM_FOREGROUND,
                uint32 WindowsEvents.EVENT_SYSTEM_FOREGROUND,
                nativeint 0,
                procDelegate.Value,
                uint32 0,
                uint32 0,
                uint32 0x0003
            )

        hooked <- true
