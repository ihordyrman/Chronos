namespace Chronos.Worker

open System
open System.Collections.Generic
open System.Diagnostics
open System.Text
open System.Threading
open System.Threading.Tasks
open Chronos.Core.Native
open Chronos.CoreF
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    let mutable applicationActivities = HashSet<ApplicationActivity>()
    let mutable handler = new ActiveWindowEventHandler()

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

                // DisplayApplications()
                applicationActivities.Add(activity) |> ignore

            | _ -> ())

    override this.ExecuteAsync(ctx: CancellationToken) =
        task {
            handler <- new ActiveWindowEventHandler()
            handler.MakeAHook()
            handler.Hook.AddHandler(eventHandler)

            // todo: message should be rewritten to F# class, since it's mutable
            // let mutable msg = Msg()
            // let response = NativeFunctions.GetMessage(&msg, IntPtr.Zero, 0u, 0u)
            //
            // if response = 0 || response = -1 then
            //     return ()
            //
            // NativeFunctions.TranslateMessage(&msg) |> ignore
            // NativeFunctions.DispatchMessage(&msg) |> ignore

            do! Task.Delay(1000)
        }


    member this.DisplayApplications() =
        let apps =
            applicationActivities
            |> Seq.groupBy (fun x -> x.ApplicationName)
            |> Seq.map (fun (key, group) ->
                let totalTime = group |> Seq.sumBy (fun y -> (y.End.Value - y.Start).TotalSeconds)
                key, totalTime)
            |> Seq.toList

        for appName, totalTime in apps do
            logger.LogInformation("{0} - {1:F2} seconds", appName, totalTime)
