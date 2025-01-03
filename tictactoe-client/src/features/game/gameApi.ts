export type SquareValue = "X" | "O" | undefined;

export type Player = {
    name: string,
    isX: boolean
}

export type Move = {
    gameId: string,
    square: number
}

export type Create = {
    gameId: string
}

export type Join = {
    gameId: string
}

export type GameState = {
    gameId: string,
    players: Player[],
    squares: SquareValue[],
    xIsNext: boolean
}