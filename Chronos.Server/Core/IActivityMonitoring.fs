namespace Chronos.Server.Core

open System.Threading

type IActivityMonitoring =
    abstract member StartMonitoring: CancellationToken -> unit
    abstract member Activity: IEvent<ApplicationActivity>
