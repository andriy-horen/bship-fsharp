module Bship.Models

type Point = int * int

type Ship = Point * Point

type Fleet = Ship list

type Player =
    | P1
    | P2

type PlayerMove = { Player: Player; Point: Point }

type GameResult = { Winner: Player }

type GameEvent =
    | GameStart
    | PlayerMove of PlayerMove
    | GameOver of GameResult

type GameState =
    { fleets: Fleet * Fleet
      events: GameEvent list }

type GridSquare =
    | Empty // .
    | Ship // #
    | Hit // +
    | Miss // -
    | Kill // x

type Grid = GridSquare[,]


