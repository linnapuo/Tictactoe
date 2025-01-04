import { HubConnection } from "@microsoft/signalr";
import { createAction, createAsyncThunk, createSlice, SerializedError } from "@reduxjs/toolkit";

interface ClientState {
    connected: boolean,
    connectionId: string,
    error?: SerializedError
};

const clientstate: ClientState = {
    connected: false,
    connectionId: "",
    error: undefined
};

export const createClientSlice = ({name, client}: {name: string, client: HubConnection}) => {

    const connectedAction = createAction<ClientState>(name + "/connected");

    const startClient = createAsyncThunk(name + "/start", async (_, {dispatch}) => {
        
        const connected = (state: ClientState) => connectedAction(state);

        await client.start();

        dispatch(connected({
            connected: true,
            connectionId: client.connectionId ?? "",
            error: undefined
        }));
    
        client.onreconnected((connectionId) => {
            dispatch(connected({
                connected: true,
                connectionId: connectionId ?? "",
                error: undefined
            }));
        });
    
        client.onreconnecting((error) => {
            dispatch(connected({
                connected: false,
                connectionId: "",
                error: error
            }));
        });
    });

    const slice = createSlice({
        name,
        initialState: clientstate,
        reducers: {
            connected: (_state, action) => {
                // eslint-disable-next-line @typescript-eslint/no-unsafe-return
                return action.payload;
            }
        },
        extraReducers: (builder) => {
            builder
            // eslint-disable-next-line @typescript-eslint/no-empty-function
            .addCase(startClient.fulfilled, () => {})
            .addCase(startClient.rejected, (state, action) => {
                state.error = action.error;
            });
        }
    });

    return {slice, startClient};
}