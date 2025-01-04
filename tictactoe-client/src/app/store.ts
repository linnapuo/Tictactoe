import { configureStore, ThunkAction, Action } from "@reduxjs/toolkit";
import { errorReducer } from "src/features/error/Error";
import { gameReducer } from "src/features/game/gameSlice";
import { chatClientReducer } from "src/features/chat/chatClient";
import { gameClientReducer } from "src/features/game/gameClient";

export const store = configureStore({
  reducer: {
    game: gameReducer,
    gameClient: gameClientReducer,
    chatClient: chatClientReducer,
    error: errorReducer,
  },
});

export type AppDispatch = typeof store.dispatch;
export type RootState = ReturnType<typeof store.getState>;
export type AppThunk<ReturnType = void> = ThunkAction<ReturnType, RootState, unknown, Action>;
