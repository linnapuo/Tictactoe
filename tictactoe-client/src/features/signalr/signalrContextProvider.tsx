import { HubConnection, HubConnectionBuilder } from "@microsoft/signalr";
import { createContext, FC, useContext } from "react";

const SignalrContext = createContext({
  client: new HubConnectionBuilder().withUrl("placeholder").build(),
});

interface Props {
  client: HubConnection;
  children: React.ReactNode;
}

export const SignalrContextProvider: FC<Props> = ({ client, children }) => {
  return <SignalrContext.Provider value={{ client }}>{children}</SignalrContext.Provider>;
};

interface Message {
  methodName: string;
  payload: unknown;
}

export const useSignalrClient = () => {
  const { client } = useContext(SignalrContext);

  return {
    sendMessage: ({ methodName, payload }: Message) => client.invoke(methodName, payload),
  };
};
