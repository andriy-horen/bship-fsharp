module Bship.GameMatchingService

open System
open System.Collections.Concurrent
open System.Threading
open Bship.GameHub
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Hosting
open Microsoft.Extensions.Logging

type GameMatchingService(hubContext: IHubContext<GameHub>, logger: ILogger<GameMatchingService>) =
    inherit BackgroundService()

    member val lastTick = DateTimeOffset.MinValue with get, set

    override this.ExecuteAsync(ct) =

        task {
            let timerInterval = TimeSpan.FromSeconds(3.)
            let cleanupInterval = TimeSpan.FromSeconds(10.)

            use timer = new PeriodicTimer(timerInterval)

            while not ct.IsCancellationRequested do
                let! _ = timer.WaitForNextTickAsync(ct)

                if DateTimeOffset.UtcNow - this.lastTick >= cleanupInterval then
                    // logger.LogInformation($"Entries: {connectedUsers.Keys.Count}")
                    this.lastTick <- DateTimeOffset.UtcNow

                try
                    let users =
                        connectedUsers.Values
                        |> Seq.choose (function
                            | Waiting ctx -> Some ctx
                            | _ -> None)
                        |> Seq.chunkBySize 2
                        |> Seq.choose (function
                            | [| a; b |] -> Some(a, b)
                            | _ -> None)
                        |> Seq.iter (fun (ctx1, ctx2) ->
                            logger.LogInformation(
                                "Matched users {User1} and {User2}",
                                ctx1.UserIdentifier,
                                ctx2.UserIdentifier
                            )

                            let gameId = Guid.NewGuid().ToString()

                            connectedUsers.TryUpdate(ctx1.UserIdentifier, InGame(gameId, ctx1), Waiting ctx1)
                            |> ignore

                            connectedUsers.TryUpdate(ctx2.UserIdentifier, InGame(gameId, ctx2), Waiting ctx2)
                            |> ignore

                            hubContext.Clients
                                .Client(ctx1.ConnectionId)
                                .SendAsync("Matched", ctx2.UserIdentifier)
                            |> ignore

                            hubContext.Clients
                                .Client(ctx2.ConnectionId)
                                .SendAsync("Matched", ctx1.UserIdentifier)
                            |> ignore)

                    ()
                with ex ->
                    logger.LogError(ex, "Error checking for waiting users")
        }
