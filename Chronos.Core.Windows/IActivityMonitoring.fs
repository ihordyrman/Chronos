namespace Chronos.Core

type IActivityMonitoring =
    abstract member StartMonitoring: unit -> unit
    abstract member StopMonitoring: unit -> unit
    abstract member Activity: IEvent<ApplicationActivity>
