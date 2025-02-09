module Bship.GameHub

open System
open System.Collections.Concurrent
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging

let connectedUsers = ConcurrentDictionary<string, HubCallerContext>()

type GameHub(logger: ILogger<GameHub>) =
    inherit Hub()

    override this.OnConnectedAsync() =
        task {
            let user = this.Context.UserIdentifier
            let connection = this.Context.ConnectionId

            logger.LogInformation("User connected: {User}; Connection: {Connection}", user, connection)

            connectedUsers.TryAdd(user, this.Context) |> ignore
        }

    override this.OnDisconnectedAsync(ex) =
        let user = this.Context.UserIdentifier
        let connection = this.Context.ConnectionId

        logger.LogInformation("User disconnected: {User}; Connection: {Connection}", user, connection)

        connectedUsers.TryRemove user |> ignore

        base.OnDisconnectedAsync(ex)
