import { createSlice, PayloadAction } from "@reduxjs/toolkit";
import { RootState } from "src/app/store";
import { GameState } from "./gameApi";

const initialState: GameState = {
  xIsNext: true,
  gameId: "",
  players: [],
  squares: [],
};

export const gameSlice = createSlice({
  name: "game",
  initialState,
  reducers: {
    gamestate: (_state, action: PayloadAction<GameState>) => {
      return action.payload;
    },
  },
});

export const gameReducer = gameSlice.reducer;
export const selectGame = (state: RootState) => state.game;
export const { gamestate } = gameSlice.actions;
