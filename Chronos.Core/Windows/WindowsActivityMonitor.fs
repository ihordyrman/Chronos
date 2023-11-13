namespace Chronos.Core.Windows

open System
open System.Diagnostics
open System.Text
open System.Threading
open Chronos.Core
open Chronos.Core.Windows.Native

type WindowsActivityMonitor() =

    let handler = new ActiveWindowEventHandler()
    let activityEvent = Event<ApplicationActivity>()

    let eventHandler =
        EventHandler<WindowArgs>(fun sender args ->
            let builder = StringBuilder(256)
            NativeFunctions.GetWindowText(args.WindowHandle, builder, 256) |> ignore

            let activeProcess =
                Process.GetProcesses() |> Array.tryFind (fun x -> x.MainWindowHandle = args.WindowHandle)

            match activeProcess with
            | Some value when value.MainModule.FileName <> null ->
                let fileVersionInfo = FileVersionInfo.GetVersionInfo(value.MainModule.FileName)

                let activity: ApplicationActivity =
                    { ProcessName = value.ProcessName
                      ApplicationName = fileVersionInfo.FileDescription
                      Title = builder.ToString()
                      Start = DateTime.Now
                      End = None }

                activityEvent.Trigger(activity)
            | _ -> ())

    interface IActivityMonitoring with

        member this.StartMonitoring(ctx: CancellationToken) =
            let mutable shouldStop = false
            handler.MakeAHook()
            handler.Hook.AddHandler(eventHandler)

            while not shouldStop || ctx.IsCancellationRequested do
                let mutable msg: Msg = Unchecked.defaultof<Msg>

                let response = NativeFunctions.GetMessage(&msg, IntPtr.Zero, 0u, 0u)

                if response = 0 || response = -1 then
                    shouldStop <- true

                NativeFunctions.TranslateMessage(&msg) |> ignore
                NativeFunctions.DispatchMessage(&msg) |> ignore

            handler.Hook.RemoveHandler(eventHandler)

        member this.Activity = activityEvent.Publish
