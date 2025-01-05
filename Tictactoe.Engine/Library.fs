namespace Tictactoe.Engine

type Game = {
    mutable xIsNext: bool
    squares: string option[]
}

type Player = {
    name: string
    isX: bool
}

type Lobby = {
    gameId: string
    players: Player list
    game: Game
}

type Move = {
    gameId: string
    square: int
}

type Create = {
    gameId: string
}

type Join = {
    gameId: string
}

type Gamestate = {
    gameId: string
    squares: string option[]
    players: Player list
    xIsNext: bool
}

module Funcs =

    let private lines = [
        (0,1,2)
        (3,4,5)
        (6,7,8)
        (0,3,6)
        (1,4,7)
        (2,5,8)
        (0,4,8)
        (2,4,6)
    ]

    let private checkWinner (squares: string option[]) = 
        lines
        |> Seq.tryFind(fun (a,b,c) -> squares[a].IsSome && squares[a] = squares[b] && squares[a] = squares[c]) 
        |> Option.map(fun (a,_,_) -> squares[a])

    let CreateGame (create: Create) connectionId = 
        {
        gameId = create.gameId
        players = [{name = connectionId; isX = true}]
        game = {xIsNext = true; squares = Array.zeroCreate 9}
        }

    let JoinGame (lobby: Lobby) connectionId = 
        match lobby.players.Length = 2 with
        | true -> Error "Game has already started"
        | false -> Ok ()
        |> Result.map(fun _ -> { lobby with players = lobby.players @ [{name = connectionId; isX = false}] })

    let private validatePlayersTurn lobby player = 
        match player.isX && not lobby.game.xIsNext || not player.isX && lobby.game.xIsNext with
        | true -> Error "Not your turn"
        | false -> Ok lobby

    let private validateMove lobby move = 
        match move.square < 0 || move.square > 8 || lobby.game.squares[move.square].IsSome with
        | true -> Error "Invalid move"
        | false -> Ok lobby

    let private gameOver lobby =
        let matcher some = if not some then Ok lobby else Error "Game over"
        checkWinner lobby.game.squares |> Option.isSome |> matcher

    let MakeMove move (lobby: Lobby) connectionId = 
    
        let player = lobby.players |> List.find(fun p -> p.name = connectionId)

        let lobbyPlayer lobby = lobby, player
        let lobbyMove lobby = lobby, move
        
        let myMove (lobby, player) = validatePlayersTurn lobby player
        let legalMove (lobby, move) = validateMove lobby move

        let doMove (lobby, player) =
            lobby.game.squares[move.square] <- if player.isX then Some "X" else Some "O"
            lobby.game.xIsNext <- not lobby.game.xIsNext
            lobby

        gameOver lobby
        |> Result.map lobbyPlayer
        |> Result.bind myMove
        |> Result.map lobbyMove
        |> Result.bind legalMove
        |> Result.map (lobbyPlayer >> doMove)
