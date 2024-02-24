namespace Chronos.Server.Worker

open System
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open Chronos.Server.Core
open Chronos.Server.Core.Windows
open Chronos.Server.Data
open Chronos.Server.Data.Repositories
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type Worker(logger: ILogger<Worker>, activityRepository: IRepository<Activity>) =
    inherit BackgroundService()

    let activities = Queue<ApplicationActivity>()

    let mapActivity (activity: ApplicationActivity) : Activity =
        { Id = Guid.NewGuid()
          AppId = Guid.NewGuid()
          Start = activity.Start
          End = DateTime.Now
          ProcessId = Guid.NewGuid() }

    [<TailCall>]
    let rec synchronize (ctx: CancellationToken) =
        task {
            match ctx.IsCancellationRequested with
            | true -> return ()
            | false ->
                while activities.Count > 0 do
                    let activity = activities.Dequeue() |> mapActivity
                    let! _ = activityRepository.InsertAsync(activity)
                    ()

                do! Task.Delay 1000
                return! synchronize ctx
        }


    let activityHandler =
        Handler<ApplicationActivity>(fun _ activity ->
            activities.Enqueue(activity)
            logger.LogTrace $"New activity recorded: {activity}")

    override this.ExecuteAsync(ctx: CancellationToken) =
        synchronize ctx |> ignore

        Task.Run(fun () ->
            let activityMonitor = WindowsActivityMonitor() :> IActivityMonitoring
            activityMonitor.Activity.AddHandler(activityHandler)
            activityMonitor.StartMonitoring ctx)
        |> ignore

        Task.CompletedTask
