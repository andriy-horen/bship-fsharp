module Bship.GameService

open Microsoft.Extensions.Logging

type GameService(logger: ILogger<GameService>) =
    do logger.LogInformation("GameService initialized")

    member this.GetGame() = "Game"
