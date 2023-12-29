module AppMonitoringService

open System
open System.Threading.Tasks
open Chronos.Data.Repositories
open Chronos.Data

let saveApplicationAsync (appRepository: IRepository<App>) (app: App) =
    async {
        let! existingApp = appRepository.GetAsync(app.Id) |> Async.AwaitTask

        match existingApp with
        | None -> do appRepository.InsertAsync(app) |> Async.AwaitTask |> ignore
        | Some _ -> do appRepository.UpdateAsync(app) |> Async.AwaitTask |> ignore
    }

let saveProcessAsync (processRepository: IRepository<Process>) (ps: Process) =
    async {
        let! existingProcess = processRepository.GetAsync(ps.Id) |> Async.AwaitTask

        match existingProcess with
        | None -> do processRepository.InsertAsync(ps) |> Async.AwaitTask |> ignore
        | Some _ -> do processRepository.UpdateAsync(ps) |> Async.AwaitTask |> ignore
    }

let saveActivityAsync (activityRepository: IRepository<Activity>) (activity: Activity) =
    async { Task.Delay(1000) |> ignore }

let disposeRepositories
    (appRepository: IDisposable)
    (activityRepository: IDisposable)
    (processRepository: IDisposable)
    =
    appRepository.Dispose()
    activityRepository.Dispose()
    processRepository.Dispose()
