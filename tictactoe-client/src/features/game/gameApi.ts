export type SquareValue = "X" | "O" | undefined;

export interface Player {
  name: string;
  isX: boolean;
}

export interface Move {
  gameId: string;
  square: number;
}

export interface Create {
  gameId: string;
}

export interface Join {
  gameId: string;
}

export interface Spectate {
  gameId: string;
}

export interface GameState {
  gameId: string;
  players: Player[];
  squares: SquareValue[];
  xIsNext: boolean;
}
