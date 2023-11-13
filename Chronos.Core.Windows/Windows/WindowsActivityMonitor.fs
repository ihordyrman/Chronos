namespace Chronos.Core.Windows

open System
open System.Diagnostics
open System.Text
open Chronos.Core
open Chronos.Core.Windows.Native

type WindowsActivityMonitor() =

    let mutable applicationActivities: list<ApplicationActivity> = List.empty
    let handler = new ActiveWindowEventHandler()

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

                if applicationActivities.Length >= 1 then
                    let lastAppActivity =
                        { (applicationActivities |> List.rev |> List.head) with
                            End = Some DateTime.Now }

                    let listWithoutLast = applicationActivities |> List.rev |> List.tail
                    applicationActivities <- listWithoutLast @ [ lastAppActivity; activity ]
                else
                    applicationActivities <- applicationActivities @ [ activity ]

            | _ -> ())

    interface IActivityMonitoring with

        member this.StartMonitoring() =
            let mutable shouldStop = false
            handler.MakeAHook()
            handler.Hook.AddHandler(eventHandler)

            while not shouldStop do
                let mutable msg: Msg = Unchecked.defaultof<Msg>
                let response = NativeFunctions.GetMessage(&msg, IntPtr.Zero, 0u, 0u)

                if response = 0 || response = -1 then
                    shouldStop <- true

                NativeFunctions.TranslateMessage(&msg) |> ignore
                NativeFunctions.DispatchMessage(&msg) |> ignore

        member this.StopMonitoring() = handler.Hook.RemoveHandler(eventHandler)

        member this.Activity = failwith "todo"
