namespace Chronos.Server.Data.Repositories

open System
open Chronos.Server.Data.Repositories
open Chronos.Server.Data
open Microsoft.Extensions.Logging

type ActivityRepository(manager: IConnectionManager, logger: ILogger<ActivityRepository>) =

    interface IRepository<Activity> with
        member this.InsertAsync(activity: Activity) =
            task {
                logger.LogInformation("Inserting activity {activity}", activity.AppId)
                return true
            }

        member this.UpdateAsync(activity: Activity) = task { return true }

        member this.DeleteAsync(activity: Activity) = task { return true }

        member this.GetAsync(id: Guid) =
            task {
                return
                    Some(
                        { Id = Guid.NewGuid()
                          End = DateTime.Now
                          Start = DateTime.Now
                          AppId = Guid.NewGuid()
                          ProcessId = Guid.NewGuid() }
                    )
            }

        member this.Dispose() = ()
