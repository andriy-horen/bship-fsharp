module Bship.Game

open Bship.Models

let getAllMoves gameState =
    gameState
    |> List.choose (fun event ->
        match event with
        | GameEvent.PlayerMove move -> Some move
        | _ -> None)

let toPoints (((x1, y1), (x2, y2)): Ship) : Point list =
    [ for x in x1..x2 do
          for y in y1..y2 do
              yield x, y ]

let emptyGrid sizeX sizeY =
    Array2D.create sizeX sizeY GridSquare.Empty

let fleetToGrid fleet =
    let grid = emptyGrid 10 10

    fleet
    |> List.collect toPoints
    |> List.iter (fun (x, y) -> grid[x, y] <- GridSquare.Ship)

    grid

let getFleet gameState player =
    let fleet1, fleet2 = gameState.fleets

    match player with
    | Player.P1 -> fleet1
    | Player.P2 -> fleet2

let getGrid gameState player =
    let fleet = getFleet gameState player
    fleetToGrid fleet

let markKilled (grid: Grid) fleet =
    fleet
    |> List.map toPoints
    |> List.iter (fun points ->
        let allHit = points |> List.forall (fun (x, y) -> grid[x, y] = GridSquare.Hit)

        if allHit then
            points |> List.iter (fun (x, y) -> grid[x, y] <- GridSquare.Kill))

    grid

let reduceState gameState =
    let moves = getAllMoves gameState.events
    let grid1 = getGrid gameState Player.P1
    let grid2 = getGrid gameState Player.P2

    moves
    |> List.iter (fun { Point = (x, y); Player = player } ->
        let grid = if player = Player.P1 then grid2 else grid1

        match grid[x, y] with
        | GridSquare.Empty -> grid[x, y] <- GridSquare.Miss
        | GridSquare.Ship -> grid[x, y] <- GridSquare.Hit
        | _ -> ())

    let fleet1, fleet2 = gameState.fleets
    let grid1, grid2 = markKilled grid1 fleet1, markKilled grid2 fleet2

    grid1, grid2
