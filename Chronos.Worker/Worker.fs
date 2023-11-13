namespace Chronos.Worker

open System.Threading
open System.Threading.Tasks
open Chronos.Core
open Chronos.Core.Windows
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type Worker(logger: ILogger<Worker>) =
    inherit BackgroundService()

    let activityHandler =
        Handler<ApplicationActivity>(fun _ activity -> logger.LogInformation $"New activity recorded: {activity}")

    override this.ExecuteAsync(ctx: CancellationToken) =
        Task.Run(fun () ->
            let activityMonitor = WindowsActivityMonitor() :> IActivityMonitoring
            activityMonitor.Activity.AddHandler(activityHandler)
            activityMonitor.StartMonitoring ctx)
        |> ignore

        Task.CompletedTask
