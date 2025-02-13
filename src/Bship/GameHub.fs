module Bship.GameHub

open System.Collections.Concurrent
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging

type ConnectedUser =
    | InGame of GameId: string * HubCallerContext: HubCallerContext
    | Waiting of HubCallerContext: HubCallerContext

let connectedUsers = ConcurrentDictionary<string, ConnectedUser>()

type GameHub(logger: ILogger<GameHub>) =
    inherit Hub()

    // TODO: rewrite logging messages to be consistent and aligned e.g. user first or connection first
    // TODO: for all Try* methods add logging for the result
    override this.OnConnectedAsync() =
        task {
            let userId = this.Context.UserIdentifier
            let connectionId = this.Context.ConnectionId

            logger.LogInformation("new connection {Connection} (usedId: {User})", connectionId, userId)

            match connectedUsers.TryRemove userId with
            | true, oldConnectedUser ->
                let newConnectedUser, oldConnection =
                    match oldConnectedUser with
                    | Waiting connection -> (Waiting this.Context, connection)
                    | InGame(gameId, connection) -> (InGame(gameId, this.Context), connection)

                connectedUsers.TryAdd(userId, newConnectedUser) |> ignore
                oldConnection.Abort()
            | false, _ -> connectedUsers.TryAdd(userId, Waiting this.Context) |> ignore
        }

    override this.OnDisconnectedAsync(ex) =
        let userId = this.Context.UserIdentifier
        let connectionId = this.Context.ConnectionId

        logger.LogInformation("Connection closed {Connection} by user {User}", userId, connectionId)

        match connectedUsers.TryGetValue userId with
        | true, connectedUser ->
            match connectedUser with
            | Waiting connection
            | InGame(_, connection) when connection.ConnectionId = connectionId ->
                logger.LogInformation("Removing user {User} from connected users", userId)
                connectedUsers.TryRemove userId |> ignore
            | _ ->
                logger.LogInformation(
                    "User {User} reconnected, closing current connection {Connection}",
                    userId,
                    connectionId
                )
        | _ -> logger.LogWarning("User {User} not found in connected users", userId)

        base.OnDisconnectedAsync(ex)
