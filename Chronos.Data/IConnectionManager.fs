namespace Chronos.Data

open System
open System.Data

type IConnectionManager =
    abstract member Connection: IDbConnection with get
    abstract member OpenConnection: unit -> unit
    abstract member BeginTransaction: unit -> IDbTransaction
    inherit IDisposable
