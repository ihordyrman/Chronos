namespace Chronos.Server.Data

open System

type Process = { Id: Guid; Name: string }
type App = { Id: Guid; Name: string; ProcessId: Guid }
type Activity = { Id: Guid; AppId: Guid; Start: DateTime; End: DateTime; ProcessId: Guid }
