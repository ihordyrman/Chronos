namespace Chronos.Worker

open System.Data
open Chronos.Data
open Chronos.Data.Database
open Chronos.Data.Repositories
open Microsoft.Data.Sqlite
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let createHostBuilder args =
        Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(fun hostContext services ->
                services.AddTransient<IDbConnection>(fun _ ->
                    new SqliteConnection("Data Source=C:\Projects\Personal\Chronos\Chronos.Data\dev_chronos.db"))
                |> ignore

                services.AddTransient<IRepository<Activity>, ActivityRepository>() |> ignore
                services.AddSingleton<IConnectionManager, SqLiteConnectionManager>() |> ignore
                services.AddHostedService<Worker>() |> ignore
                services.AddLogging() |> ignore)

    [<EntryPoint>]
    let main args =
        createHostBuilder(args).Build().Run()
        0
