module Bship.Program

open System
open Bship.GameHub
open Microsoft.AspNetCore.Builder
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.DependencyInjection

[<EntryPoint>]
let main args =
    let builder = WebApplication.CreateBuilder(args)

    builder.Services.AddCors().AddLogging().AddSignalR() |> ignore

    let app = builder.Build()
    
    app.MapGet("/", Func<string>(fun () -> "Hello World!")) |> ignore

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
    |> ignore

    app.Run()
    0
