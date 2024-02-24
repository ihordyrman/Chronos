namespace Chronos.Server.Data.Database

open System
open System.Data
open Chronos.Server.Data

type SqLiteConnectionManager(connection: IDbConnection) =
    let mutable innerConnection = connection

    member this.Connection =
        this.OpenConnection()
        innerConnection

    member private this.OpenConnection() =
        match innerConnection.State with
        | ConnectionState.Open
        | ConnectionState.Connecting -> ()
        | _ -> innerConnection.Open()



    member this.BeginTransaction() = this.Connection.BeginTransaction()

    interface IConnectionManager with
        member this.Connection = this.Connection
        member this.OpenConnection() = this.OpenConnection()
        member this.BeginTransaction() = this.BeginTransaction()

    interface IDisposable with
        member this.Dispose() =
            if innerConnection.State <> ConnectionState.Closed then
                innerConnection.Close()
