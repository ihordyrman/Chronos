namespace Chronos.Server.Core.Windows

open System
open System.Diagnostics
open System.Text
open System.Threading
open Chronos.Server.Core
open Chronos.Server.Core.Windows.Native

type WindowsActivityMonitor() =

    let handler = new ActiveWindowEventHandler()
    let activityEvent = Event<ApplicationActivity>()

    let getActiveWindowProcessId () : uint32 =
        let foregroundWindowHandle = NativeFunctions.GetForegroundWindow()
        let mutable processId = 0u
        let _ = NativeFunctions.GetWindowThreadProcessId(foregroundWindowHandle, &processId)
        processId

    let eventHandler =
        EventHandler<WindowArgs>(fun sender args ->
            let builder = StringBuilder(256)
            NativeFunctions.GetWindowText(args.WindowHandle, builder, 256) |> ignore

            let activeProcess =
                Process.GetProcesses() |> Array.tryFind (fun x -> x.MainWindowHandle = args.WindowHandle)

            match activeProcess with
            | Some value ->
                let fileVersionInfo = FileVersionInfo.GetVersionInfo(value.MainModule.FileName)

                let activity: ApplicationActivity =
                    { ProcessName = value.ProcessName
                      ApplicationName = fileVersionInfo.FileDescription
                      Title = builder.ToString()
                      Start = DateTime.Now
                      End = None }

                activityEvent.Trigger(activity)
            | None ->
                let mutable foundProcess: Process option = None

                let enumWindowsCallback (hWnd: nativeint) (lParam: nativeint) : bool =
                    let mutable windowProcessId = 0u
                    NativeFunctions.GetWindowThreadProcessId(hWnd, &windowProcessId) |> ignore

                    if windowProcessId = (lParam |> uint32) then
                        foundProcess <- Process.GetProcesses() |> Array.tryFind (fun x -> x.Id = int windowProcessId)
                        false
                    else
                        true

                let activeWindowProcessId = getActiveWindowProcessId ()
                NativeFunctions.EnumWindows(enumWindowsCallback, activeWindowProcessId) |> ignore

                match foundProcess with
                | Some foundProcess when foundProcess.MainModule.FileName <> null ->
                    let fileVersionInfo = FileVersionInfo.GetVersionInfo(foundProcess.MainModule.FileName)

                    let activity: ApplicationActivity =
                        { ProcessName = foundProcess.ProcessName
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
