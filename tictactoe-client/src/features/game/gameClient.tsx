import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { RootState } from "src/app/store";
import React, { FC, useEffect } from "react";
import { useAppDispatch } from "src/app/hooks";
import { Create, GameState, Join, Move } from "./gameApi";
import { gamestate } from "./gameSlice";
import { SignalrContextProvider, useSignalrClient } from "src/features/signalr/signalrContextProvider";
import { createClientSlice } from "src/features/signalr/signalrClientSlice";

const client = new HubConnectionBuilder()
    .configureLogging(LogLevel.Debug)
    .withUrl("https://localhost:7138/gamehub")
.build();

const { slice, startClient } = createClientSlice({
    name: "gameClient",
    client
});

export const gameClientReducer = slice.reducer;
export const selectGameClient = (state: RootState) => state.gameClient;
export const { connected } = slice.actions;

interface Props {
    children: React.ReactNode
}

export const GameClientProvider: FC<Props> = ({children}) => {
    const dispatch = useAppDispatch();

    useEffect(() => {
        client.on("Gamestate", (state: GameState) => {
            dispatch(gamestate(state));
        });

        void dispatch(startClient());
    }, [dispatch]);

    return (
        <SignalrContextProvider client={client}>
            {children}
        </SignalrContextProvider>
    );
};

export const useGameClient = () => {
    const {sendMessage} = useSignalrClient();

    return {
        startClient,
        create: (payload: Create) => sendMessage({methodName: "Create", payload}),
        join: (payload: Join) => sendMessage({methodName: "Join", payload}),
        move: (payload: Move) => sendMessage({methodName: "Move", payload})
    };
};