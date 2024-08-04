module Bship.Game

open Bship.Models

let getAllMoves gameState =
    gameState
    |> List.choose (fun event ->
        match event with
        | GameEvent.PlayerMove move -> Some move
        | _ -> None)

let getFleet gameState player =
    let fleet1, fleet2 = gameState.fleets

    match player with
    | Player.P1 -> fleet1
    | Player.P2 -> fleet2

let toPoints (((x1, y1), (x2, y2)): Ship) : Point list =
    [ for x in x1..x2 do
          for y in y1..y2 do
              yield x, y ]

let emptyGrid sizeX sizeY =
    Array2D.create sizeX sizeY GridSquare.Empty

let fleetToGrid fleet =
    let grid = emptyGrid 10 10

    fleet
    |> List.map toPoints
    |> List.collect id
    |> List.iter (fun (x, y) -> grid[x, y] <- GridSquare.Ship)

    grid

let reduceState gameState =
    let moves = getAllMoves gameState.events
    let fleet1, fleet2 = gameState.fleets

    match moves with
    | [] -> fleetToGrid(fleet1), fleetToGrid(fleet2) 
    | _ -> (emptyGrid 10 10, emptyGrid 10 10)
