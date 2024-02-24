namespace Chronos.Server.Worker

open System
open System.Data
open Chronos.Server.Data
open Chronos.Server.Data.Database
open Chronos.Server.Data.Repositories
open Microsoft.Data.Sqlite
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting
open FluentMigrator.Runner

module Program =

    let dbPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Chronos\\Chronos.db"

    let private createDatabaseIfNotExists () =

        if not (System.IO.File.Exists(dbPath)) then
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath)) |> ignore
            let serviceProvider =
                ServiceCollection()
                    .AddFluentMigratorCore()
                    .ConfigureRunner(fun builder ->
                        builder
                            .AddSQLite()
                            .WithGlobalConnectionString($"Data Source={dbPath}")
                            .ScanIn(typeof<InitialMigration>.Assembly)
                            .For.Migrations()

                        |> ignore)
                    .BuildServiceProvider(false)

            let scope = serviceProvider.CreateScope()
            let runner = scope.ServiceProvider.GetService<IMigrationRunner>()
            runner.MigrateUp()

    let createHostBuilder args =
        createDatabaseIfNotExists ()

        Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(fun hostContext services ->
                services.AddTransient<IDbConnection>(fun _ -> new SqliteConnection($"Data Source={dbPath}"))
                |> ignore

                services.AddTransient<IRepository<Activity>, ActivityRepository>() |> ignore
                services.AddSingleton<IConnectionManager, SqLiteConnectionManager>() |> ignore
                services.AddHostedService<Worker>() |> ignore
                services.AddLogging() |> ignore)

    [<EntryPoint>]
    let main args =
        createHostBuilder(args).Build().Run()
        0
