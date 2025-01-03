import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { createContext, FC, useContext } from 'react';

const SignalrContext = createContext({
    client: new HubConnectionBuilder().withUrl("http://localhost:3000").build()
});

export const SignalrContextProvider: FC<{client: HubConnection}> = ({client, children}) => {
    return (
        <SignalrContext.Provider value={{client}}>
            {children}
        </SignalrContext.Provider>
    );
};

type Message = {
    methodName: string,
    payload: unknown
}

export const useSignalrClient = () => {
    const {client} = useContext(SignalrContext);

    return {
        sendMessage: ({methodName, payload}: Message) => client.invoke(methodName, payload)
    };
};