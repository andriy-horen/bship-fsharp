module Bship.GameHub

open System
open System.Collections.Concurrent
open Microsoft.AspNetCore.SignalR

type GameHub() as self =
    inherit Hub()

    static let games =
        ConcurrentDictionary<string, Tuple<HubCallerContext, HubCallerContext>>()

    static let waiting = ConcurrentDictionary<string, HubCallerContext>()

    override this.OnConnectedAsync() =
        task {
            match waiting.Count with
            // no one to pair with, adding to a waiting list
            | 0 -> waiting.TryAdd(this.Context.ConnectionId, this.Context) |> ignore
            // pair with other client
            | _ ->
                let groupId = Guid.NewGuid()
                
                let otherClient = waiting |> Seq.head
                waiting.TryRemove(otherClient) |> ignore

                this.Context.Items.Add("gameId", groupId.ToString())
                games.TryAdd(groupId.ToString(), (this.Context, otherClient.Value)) |> ignore

                do! this.Groups.AddToGroupAsync(this.Context.ConnectionId, groupId.ToString())
                do! this.Groups.AddToGroupAsync(otherClient.Value.ConnectionId, groupId.ToString())

            do! self.OnConnectedAsync()
        }

    override this.OnDisconnectedAsync(ex) =
        let connectionId = this.Context.ConnectionId
        waiting.TryRemove connectionId |> ignore

        self.OnDisconnectedAsync(ex)
