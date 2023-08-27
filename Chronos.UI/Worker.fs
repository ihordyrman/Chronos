namespace Chronos.Worker

open System
open System.Collections.Generic
open System.Diagnostics
open System.Text
open System.Threading
open System.Threading.Tasks
open Chronos.Core
open Chronos.Core.Models
open Chronos.Core.Native
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    let mutable applicationActivities = HashSet<ApplicationActivity>()

    override this.ExecuteAsync(ctx: CancellationToken) =
        task {
            // todo: can I use computation expression here?
            use handler = new ActiveWindowEventHandler()
            handler.MakeAHook()

            handler.Hook.Add(fun e ->
                let builder = StringBuilder(256)
                NativeLibraries.GetWindowText(e.WindowHandle, builder, 256) |> ignore

                let activeProcess =
                    Process.GetProcesses() |> Array.tryFind (fun x -> x.MainWindowHandle = e.WindowHandle)

                match activeProcess with
                | Some value when value.MainModule.FileName <> null ->
                    let fileVersionInfo = FileVersionInfo.GetVersionInfo(value.MainModule.FileName)

                    let activity =
                        ApplicationActivity(value.ProcessName, fileVersionInfo.FileDescription, builder.ToString())

                    applicationActivities
                    |> Seq.iter (fun aa -> aa.End <- if aa.End.HasValue then aa.End else Nullable(DateTime.Now))


                    this.DisplayApplications()
                    applicationActivities.Add(activity) |> ignore

                | _ -> ())

            let mutable msg = Msg()
            let response = NativeLibraries.GetMessage(&msg, IntPtr.Zero, 0u, 0u)

            if response = 0 || response = -1 then
                return ()

            NativeLibraries.TranslateMessage(&msg) |> ignore
            NativeLibraries.DispatchMessage(&msg) |> ignore

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
