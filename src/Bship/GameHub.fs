module Bship.GameHub

open System
open System.Collections.Concurrent
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging

let groupIdKey = "groupId"

let addWaiting
    (logger: ILogger)
    (waitingClients: ConcurrentDictionary<string, HubCallerContext>)
    (client: HubCallerContext)
    =
    let connectionId = client.ConnectionId

    let added = waitingClients.TryAdd(connectionId, client)
    logger.LogInformation("Adding client {ConnectionId} to the waiting list {Success}", connectionId, added)
    added

type GameHub(logger: ILogger<GameHub>) as self =
    inherit Hub()

    static let games =
        ConcurrentDictionary<string, Tuple<HubCallerContext, HubCallerContext>>()

    static let waitingClients = ConcurrentDictionary<string, HubCallerContext>()

    override this.OnConnectedAsync() =
        logger.LogWarning this.Context.ConnectionId

        task {
            match waitingClients.Count with
            // no one to pair with, adding to a waiting list
            | 0 -> addWaiting logger waitingClients this.Context |> ignore
            // pair with other client
            | _ ->
                let groupId = Guid.NewGuid()

                let otherClient = waitingClients |> Seq.head
                let removed = waitingClients.TryRemove(otherClient)

                logger.LogInformation(
                    "Retrieving a client {ConnectionId} from the waiting list {Success}",
                    otherClient.Value.ConnectionId,
                    removed
                )

                if removed = false then
                    addWaiting logger waitingClients this.Context |> ignore

                this.Context.Items.Add(groupIdKey, groupId.ToString())
                let added = games.TryAdd(groupId.ToString(), (this.Context, otherClient.Value))

                do! this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupId.ToString())
                do! this.Groups.AddToGroupAsync(otherClient.Value.ConnectionId, groupId.ToString())

                logger.LogInformation(
                    "Created a group {GroupId} with two clients {Connection1} {Connection2} {Success}",
                    groupId,
                    this.Context.ConnectionId,
                    otherClient.Value.ConnectionId,
                    added
                )

            do! self.OnConnectedAsync()
        }

    override this.OnDisconnectedAsync(ex) =
        let connectionId = this.Context.ConnectionId

        if waitingClients.ContainsKey connectionId then
            waitingClients.TryRemove connectionId |> ignore

        let _, gameId = this.Context.Items.TryGetValue groupIdKey
        // if connection has a gameId then disconnect both clients
        if gameId :? string then
            let gameId = gameId :?> string
            let success, (client1, client2) = games.TryGetValue gameId

            if success then
                client1.Abort()
                client2.Abort()

        self.OnDisconnectedAsync(ex)
