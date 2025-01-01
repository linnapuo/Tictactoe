module Tictactoe.Engine.UnitTest.Tests

open Tictactoe.Engine
open Xunit

[<Fact>]
let ``CreateGame should return ok`` () =

    let create: Create = { gameId = "1" }
    
    let lobby = Funcs.CreateGame(create, "connection-1")

    let expected: Lobby = {
        name = "1"
        players = [{ name = "connection-1"; isX = true }]
        game = { xIsNext = true; squares = Array.zeroCreate 9 }
    }

    Assert.Equal(expected, lobby)

[<Fact>]
let ``JoinGame should return ok`` () =

    let lobby: Lobby = {
        name = "1"
        players = [{ name = "connection-1"; isX = true }]
        game = { xIsNext = true; squares = Array.zeroCreate 9 }
    }

    let result = Funcs.JoinGame(lobby, "connection-2")

    let expected: Result<Lobby, string> = Ok {
        name = "1"
        players = [{ name = "connection-1"; isX = true }; { name = "connection-2"; isX = false }]
        game = { xIsNext = true; squares = Array.zeroCreate 9 }
    }

    Assert.Equal(expected, result)

[<Fact>]
let ``MakeMove should return ok`` () =

    let move: Move = {
        gameId = "1"
        square = 0
    }
    
    let lobby: Lobby = {
        name = "1"
        players = [{ name = "connection-1"; isX = true }; { name = "connection-2"; isX = false }]
        game = { xIsNext = true; squares = Array.zeroCreate 9 }
    }

    let result = Funcs.MakeMove(move, lobby, "connection-1")

    let expected: Result<Lobby, string> = Ok {
        name = "1"
        players = [{ name = "connection-1"; isX = true }; { name = "connection-2"; isX = false }]
        game = { xIsNext = false; squares = [|Some "X"; None; None; None; None; None; None; None; None;|] }
    }

    Assert.Equal(expected, result)


[<Fact>]
let ``MakeMove should return not your turn`` () =

    let move: Move = {
        gameId = "1"
        square = 1
    }
    
    let lobby: Lobby = {
        name = "1"
        players = [{ name = "connection-1"; isX = true }; { name = "connection-2"; isX = false }]
        game = { xIsNext = false; squares = [|Some "X"; None; None; None; None; None; None; None; None;|] }
    }

    let result = Funcs.MakeMove(move, lobby, "connection-1")

    let expected: Result<Lobby, string> = Error "Not your turn"

    Assert.Equal(expected, result)