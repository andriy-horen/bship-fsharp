module Bship.GameHub

open System
open System.Collections.Concurrent
open Microsoft.AspNetCore.SignalR

type GameHub() as self =
    inherit Hub()

    static let games =
        ConcurrentDictionary<string, Tuple<HubCallerContext, HubCallerContext>>()

    static let waitingClients = ConcurrentDictionary<string, HubCallerContext>()

    override this.OnConnectedAsync() =
        task {
            match waitingClients.Count with
            // no one to pair with, adding to a waiting list
            | 0 -> waitingClients.TryAdd(this.Context.ConnectionId, this.Context) |> ignore
            // pair with other client
            | _ ->
                let groupId = Guid.NewGuid()

                let otherClient = waitingClients |> Seq.head
                waitingClients.TryRemove(otherClient) |> ignore

                this.Context.Items.Add("gameId", groupId.ToString())
                games.TryAdd(groupId.ToString(), (this.Context, otherClient.Value)) |> ignore

                do! this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupId.ToString())
                do! this.Groups.AddToGroupAsync(otherClient.Value.ConnectionId, groupId.ToString())

            do! self.OnConnectedAsync()
        }

    override this.OnDisconnectedAsync(ex) =
        let connectionId = this.Context.ConnectionId
        waitingClients.TryRemove connectionId |> ignore

        // TODO: client might not have been paired, so reading gameId will probably throw
        let gameId = this.Context.Items["gameId"] :?> string
        let success, (client1, client2) = games.TryGetValue gameId
        if success then
            client1.Abort()
            client2.Abort()

        self.OnDisconnectedAsync(ex)
