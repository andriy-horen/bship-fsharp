module Bship.Program

open System
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection
open Giraffe

let webApp = choose [ route "/" >=> text "Hello F# Api" ]

let configureApp (app: IApplicationBuilder) = app.UseGiraffe webApp

let configureServices (services: IServiceCollection) =
    services.AddSignalR() |> ignore
    services.AddGiraffe() |> ignore

let configure (webHostBuilder: IWebHostBuilder) =
    webHostBuilder.Configure(configureApp).ConfigureServices(configureServices)

[<EntryPoint>]
let main _ =
    Host
        .CreateDefaultBuilder()
        .ConfigureWebHostDefaults(configure >> ignore)
        .Build()
        .Run()
    0
