import { Box, Grid, IconButton, Input, Stack, Typography } from "@mui/material";
import React, { FC, useState } from "react";
import { ChatClientProvider, selectChatClient, useChatClient } from "./chatClient"
import SendIcon from '@mui/icons-material/Send';
import { useAppSelector } from "src/app/hooks";

type Message = {
    timestamp: Date,
    name: string,
    message: string
};

export const Chat = () => {
    const [messages, setMessages] = useState<Message[]>([]);

    const addMessage = (message: Message) => {
        setMessages([...messages, message]);
    };

    return (
        <ChatClientProvider onMessage={(message) => addMessage({...message, timestamp: new Date()})}>
            <ChatBox messages={messages} addMessage={addMessage}/>
        </ChatClientProvider>
    );
}

const ChatBox: FC<{messages: Message[], addMessage: (message: Message) => void}> = ({messages, addMessage}) => {
    const [message, setMessage] = useState("");
    const { send } = useChatClient();

    const state = useAppSelector(selectChatClient);

    const handleClick = () => {
        addMessage({message, timestamp: new Date(), name: state.connectionId});
        setMessage("");
        send(message);
    };

    const handleEnter = (event: React.KeyboardEvent<HTMLDivElement>) => {
        if (event.key === "Enter") {
            handleClick();
        }
    }

    return (
        <Box marginTop="2vmin">
            <Grid 
                container
                direction="column"
                justifyContent="flex-start"
                alignItems="flex-start"
            >
                {messages.map((value, index) => (
                    <Typography key={index}>[{value.timestamp.toLocaleTimeString()}] {value.name}: {value.message}</Typography>
                ))}
            </Grid>
            <Stack direction="row">
                <Input 
                    placeholder="Type to chat" 
                    onChange={({ target: { value } }) => setMessage(value)} 
                    value={message}
                    onKeyPress={handleEnter}
                />
                <IconButton onClick={handleClick}>
                    <SendIcon />
                </IconButton>
            </Stack>
        </Box>
    );
}