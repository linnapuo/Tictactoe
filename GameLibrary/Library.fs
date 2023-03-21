namespace GameLibrary

type Game = {
    mutable xIsNext: bool
    squares: string option[]
}

type Player = {
    name: string
    isX: bool
}

type Lobby = {
    name: string
    players: Player list
    game: Game
}

type Move = {
    gameId: string
    playerId: string
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

    let private GameOver (squares: string option[]) =
        checkWinner squares |> Option.isSome

    let CreateGame (create: Create, connectionId) = 
        {
        name = create.gameId
        players = [{name = connectionId; isX = true}]
        game = {xIsNext = true; squares = Array.zeroCreate 9}
        }

    let JoinGame (lobby: Lobby, connectionId) = 
        match lobby.players.Length = 2 with
        | true -> Error "Game has already started"
        | false -> Ok ()
       |> Result.map(fun _ -> { lobby with players = lobby.players @ [{name = connectionId; isX = false}] })

    let private validatePlayersTurn (player, lobby) = 
        match player.isX && not lobby.game.xIsNext || not player.isX && lobby.game.xIsNext with
        | true -> Error "Not your turn"
        | false -> Ok (player, lobby)

    let MakeMove (move, lobby, connectionId) = 
        match GameOver lobby.game.squares with
        | true -> Error "Game over"
        | false -> 
            lobby.players
         |> List.find(fun p -> p.name = connectionId)
         |> fun player -> validatePlayersTurn (player, lobby)
         |> Result.map(fun (player, lobby) -> 
                lobby.game.squares[move.square] <- if player.isX then Some "X" else Some "O"
                lobby.game.xIsNext <- not lobby.game.xIsNext
                lobby
            )
