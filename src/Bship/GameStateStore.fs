module Bship.GameStateStore

open System.Collections.Concurrent
open Bship.Models


module GameStateStore =
    let private store = ConcurrentDictionary<string, GameState>()
    
    let agent = MailboxProcessor.Start(fun inbox ->
        let rec loop state =
            async {
                let! msg = inbox.Receive()
                let newState =
                    match msg with
                    | GameCommandAction.GameStart ->
                        let fleets = ([], [])
                        let events = [GameEvent.GameStart]
                        { fleets = fleets; events = events }
                    | GameCommandAction.PlayerMove { Player = player; Point = point } ->
                        let gameState = store.[player]
                        let newEvents = gameState.events @ [GameEvent.PlayerMove { Player = player; Point = point }]
                        { gameState with events = newEvents }
                store.TryUpdate(player, newState, store.[player]) |> ignore
                return! loop newState
            }
        loop { fleets = ([], []); events = [] }
    )