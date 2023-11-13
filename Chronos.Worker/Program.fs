namespace Chronos.Worker

open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Hosting

module Program =
    let createHostBuilder args =
        Host
            .CreateDefaultBuilder(args)
            .ConfigureServices(fun hostContext services ->
                services.AddHostedService<Worker>() |> ignore
                services.AddLogging() |> ignore)

    [<EntryPoint>]
    let main args =
        createHostBuilder(args).Build().Run()
        0 // exit code
