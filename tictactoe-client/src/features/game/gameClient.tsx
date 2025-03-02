import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { RootState } from "src/app/store";
import React, { useEffect } from "react";
import { useAppDispatch } from "src/app/hooks";
import { Create, GameState, Join, Move, Spectate } from "./gameApi";
import { gamestate } from "./gameSlice";
import { SignalrContextProvider, useSignalrClient } from "src/features/signalr/signalrContextProvider";
import { createClientSlice } from "src/features/signalr/signalrClientSlice";

const config = {
  url: `${import.meta.env.VITE_API_BASE_URL}/gamehub`,
  endpoints: {
    gameState: "GameState",
    create: "Create",
    join: "Join",
    move: "Move",
    spectate: "Spectate",
  },
};

const client = new HubConnectionBuilder()
  .configureLogging(LogLevel.Debug)
  .withUrl(config.url)
  .build();

const { slice, startClient } = createClientSlice({
  name: "gameClient",
  client,
});

export const gameClientReducer = slice.reducer;
export const selectGameClient = (state: RootState) => state.gameClient;
export const { connected } = slice.actions;

interface Props {
  children: React.ReactNode;
}

export function GameClientProvider({ children }: Props) {
  const dispatch = useAppDispatch();

  useEffect(() => {
    client.on(config.endpoints.gameState, (state: GameState) => {
      dispatch(gamestate(state));
    });

    dispatch(startClient());
  }, [dispatch]);

  return <SignalrContextProvider client={client}>{children}</SignalrContextProvider>;
}

export const useGameClient = () => {
  const { sendMessage } = useSignalrClient();

  return {
    startClient,
    create: (payload: Create) => sendMessage({ methodName: config.endpoints.create, payload }),
    join: (payload: Join) => sendMessage({ methodName: config.endpoints.join, payload }),
    move: (payload: Move) => sendMessage({ methodName: config.endpoints.move, payload }),
    spectate: (payload: Spectate) => sendMessage({ methodName: config.endpoints.spectate, payload }),
  };
};
