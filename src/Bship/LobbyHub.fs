module Bship.LobbyHub

open Bship.Models
open Microsoft.AspNetCore.SignalR
open Microsoft.Extensions.Logging


type GameStartRequest = { Fleet: Fleet }

type LobbyHub(logger: ILogger<LobbyHub>) =
    inherit Hub()

    member this.GameStart(gameStartRequest: GameStartRequest) = this.Clients.All.SendAsync("GameStart")
    
    override this.OnConnectedAsync() = base.OnConnectedAsync()

    override this.OnDisconnectedAsync(ex) = base.OnDisconnectedAsync(ex)
