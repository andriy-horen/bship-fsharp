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
            let cleanupInterval = TimeSpan.FromSeconds(30.)

            use timer = new PeriodicTimer(timerInterval)

            while not ct.IsCancellationRequested do
                let! _ = timer.WaitForNextTickAsync(ct)

                if DateTimeOffset.UtcNow - this.lastTick >= cleanupInterval then
                    // logger.LogInformation("Deleting old games")
                    this.lastTick <- DateTimeOffset.UtcNow

                try
                    // logger.LogInformation("Checking for waiting users")
                    ()
                with ex ->
                    logger.LogError(ex, "Error checking for waiting users")
        }
