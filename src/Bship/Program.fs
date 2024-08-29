module Bship.Program

open Bship.GameHub
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Logging
open Giraffe

let webApp = choose [ route "/" >=> text "Hello F# Api" ]

let configureApp (app: IApplicationBuilder) =
    app
        .UseCors(fun cors ->
            cors
                .WithOrigins("http://localhost:8081")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
            |> ignore)
        .UseRouting()
        .UseEndpoints(fun endpoints -> endpoints.MapHub<GameHub>("/game") |> ignore)
        .UseGiraffe
        webApp

let configureServices (services: IServiceCollection) =
    services
        .AddCors()
        .AddLogging()
        .AddGiraffe()
    |> ignore

    services.AddSignalR() |> ignore

let configure (webHostBuilder: IWebHostBuilder) =
    webHostBuilder
        .ConfigureLogging(fun logging -> logging.ClearProviders().AddConsole() |> ignore)
        .Configure(configureApp)
        .ConfigureServices(configureServices)

[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(configure >> ignore)
        .Build()
        .Run()

    0
