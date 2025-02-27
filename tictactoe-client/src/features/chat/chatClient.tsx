import { HubConnectionBuilder, LogLevel } from "@microsoft/signalr";
import { createClientSlice } from "src/features/signalr/signalrClientSlice";
import { RootState } from "src/app/store";
import { SignalrContextProvider, useSignalrClient } from "src/features/signalr/signalrContextProvider";
import { FC, useEffect } from "react";
import { useAppDispatch } from "src/app/hooks";

const config = {
  url: `${import.meta.env.VITE_API_BASE_URL}/chathub`,
  apiKey: import.meta.env.VITE_API_KEY,
  endpoints: {
    receive: "Receive",
    send: "Send",
  },
};

const client = new HubConnectionBuilder()
  .configureLogging(LogLevel.Debug)
  .withUrl(config.url, {
    headers: {
      "X-Api-Key": config.apiKey
    }
  })
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
    client.on(config.endpoints.receive, onMessage);
  }, [onMessage]);

  useEffect(() => {
    dispatch(startClient());
  }, [dispatch]);

  return <SignalrContextProvider client={client}>{children}</SignalrContextProvider>;
};

export const useChatClient = () => {
  const { sendMessage } = useSignalrClient();

  const send = (payload: string) => {
    sendMessage({ methodName: config.endpoints.send, payload });
  };

  return {
    startClient,
    send,
  };
};
