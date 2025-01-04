import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { createClientSlice } from "src/features/signalr/signalrClientSlice";
import { RootState } from "src/app/store";
import { SignalrContextProvider, useSignalrClient } from "src/features/signalr/signalrContextProvider";
import { FC, useEffect } from "react";
import { useAppDispatch } from "src/app/hooks";

const client = new HubConnectionBuilder()
  .configureLogging(LogLevel.Debug)
  .withUrl("https://localhost:7138/chathub")
  .build();

export const { slice, startClient } = createClientSlice({
  name: "chatClient",
  client,
});

export const chatClientReducer = slice.reducer;
export const selectChatClient = (state: RootState) => state.chatClient;
export const { connected } = slice.actions;

interface ChatMessage {
  name: string;
  message: string;
}

interface Props {
  onMessage: (message: ChatMessage) => void;
  children: React.ReactNode;
}

export const ChatClientProvider: FC<Props> = ({ onMessage, children }) => {
  const dispatch = useAppDispatch();

  useEffect(() => {
    client.on("Receive", onMessage);
  }, [onMessage]);

  useEffect(() => {
    // eslint-disable-next-line @typescript-eslint/no-floating-promises
    dispatch(startClient());
  }, [dispatch]);

  return <SignalrContextProvider client={client}>{children}</SignalrContextProvider>;
};

export const useChatClient = () => {
  const { sendMessage } = useSignalrClient();

  const send = (payload: string) => {
    // eslint-disable-next-line @typescript-eslint/no-floating-promises
    sendMessage({ methodName: "Send", payload });
  };

  return {
    startClient,
    send,
  };
};
