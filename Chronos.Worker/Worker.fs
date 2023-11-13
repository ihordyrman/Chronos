namespace Chronos.Worker

open System
open System.Diagnostics
open System.Text
open System.Threading
open System.Threading.Tasks
open Chronos.Core
open Chronos.Core.Windows.Native
open Chronos.Core.Windows
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    let mutable applicationActivities: list<ApplicationActivity> = List.empty

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

    override this.ExecuteAsync(ctx: CancellationToken) =
        task {
            // note: GetMessage is a blocking call, so we need to run it in a separate thread.
            // additionally, activeWindowEventHandler should be created in the same thread as GetMessage to be able to receive events.
            Task.Run(fun () ->
                let mutable shouldStop = false

                let handler = new ActiveWindowEventHandler()
                handler.MakeAHook()
                handler.Hook.AddHandler(eventHandler)

                while not shouldStop do
                    let mutable msg: Msg = Unchecked.defaultof<Msg>
                    let response = NativeFunctions.GetMessage(&msg, IntPtr.Zero, 0u, 0u)

                    if response = 0 || response = -1 || ctx.IsCancellationRequested then
                        shouldStop <- true

                    NativeFunctions.TranslateMessage(&msg) |> ignore
                    NativeFunctions.DispatchMessage(&msg) |> ignore)
            |> ignore

            while not ctx.IsCancellationRequested do
                do! Async.Sleep(1000)
                this.DisplayApplications()
        }



    member this.DisplayApplications() =
        if applicationActivities.Length > 0 then
            let apps =
                applicationActivities
                |> Seq.groupBy (fun x -> x.ApplicationName)
                |> Seq.map (fun (key, group) ->
                    let totalTime =
                        group
                        |> Seq.sumBy (fun y ->
                            match y.End with
                            | Some endValue -> (endValue - y.Start).TotalSeconds
                            | None -> (DateTime.Now - y.Start).TotalSeconds)

                    key, totalTime)
                |> Seq.toList

            let appsString =
                apps
                |> List.map (fun (appName, totalTime) -> $"%s{appName} - %F{totalTime} seconds")
                |> String.concat ", "

            logger.LogInformation(appsString)
