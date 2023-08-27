module AppMonitoringService

open System
open System.Threading.Tasks
open Chronos.Data.Repositories
open Chronos.Data1

let saveApplicationAsync (appRepository: IRepository<App>) (app: App) =
    async {
        let! existingApp = appRepository.GetAsync(app.Id) |> Async.AwaitTask

        match existingApp with
        | None -> do appRepository.InsertAsync(app) |> Async.AwaitTask |> ignore
        | Some _ -> do appRepository.UpdateAsync(app) |> Async.AwaitTask |> ignore
    }

let saveProcessAsync (processRepository: IRepository<Process>) (process: Process) =
    async {
        let! existingProcess = processRepository.GetAsync(process.Id) |> Async.AwaitTask

        match existingProcess with
        | None -> do processRepository.InsertAsync(process) |> Async.AwaitTask |> ignore
        | Some _ -> do processRepository.UpdateAsync(process) |> Async.AwaitTask |> ignore
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
