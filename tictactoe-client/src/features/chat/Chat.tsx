import { Box, IconButton, Input, Stack, Typography } from "@mui/material";
import React, { FC, useState } from "react";
import { ChatClientProvider, selectChatClient, useChatClient } from "./chatClient";
import SendIcon from "@mui/icons-material/Send";
import ClearIcon from "@mui/icons-material/Clear";
import { useAppSelector } from "src/app/hooks";

interface Message {
  timestamp: Date;
  name: string;
  message: string;
}

interface ChatBoxProps {
  messages: Message[];
  addMessage: (message: Message) => void;
  clearMessages: () => void;
}

export const Chat = () => {
  const [messages, setMessages] = useState<Message[]>([]);

  const addMessage = (message: Message) => {
    setMessages([...messages, message]);
  };

  const clearMessages = () => {
    setMessages([]);
  };

  return (
    <ChatClientProvider
      onMessage={(message) => {
        addMessage({ ...message, timestamp: new Date() });
      }}
    >
      <ChatBox messages={messages} addMessage={addMessage} clearMessages={clearMessages} />
    </ChatClientProvider>
  );
};

const ChatBox: FC<ChatBoxProps> = ({ messages, addMessage, clearMessages }) => {
  const [message, setMessage] = useState("");
  const { send } = useChatClient();

  const state = useAppSelector(selectChatClient);

  const handleSend = () => {
    const trimmed = message.trim();

    if (trimmed === "") {
      return;
    }

    addMessage({
      message: trimmed,
      timestamp: new Date(),
      name: state.connectionId,
    });
    setMessage("");
    send(trimmed);
  };

  const handleKeyDown = (event: React.KeyboardEvent) => {
    if (event.key === "Enter") {
      handleSend();
    }
  };

  const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = event.target.value;
    setMessage(value);
  };

  return (
    <Box>
      <Stack direction={"row"} justifyContent={"center"}>
        <Stack direction="column" overflow={"auto"} minHeight={"10rem"} maxHeight={"10rem"} width={"100%"}>
          {messages.map((value, index) => (
            <Typography alignSelf={"center"} textAlign={"start"} width={"100%"} key={index} noWrap>
              [{value.timestamp.toLocaleTimeString()}] {value.name}: {value.message}
            </Typography>
          ))}
        </Stack>
      </Stack>
      <Stack direction="row" justifyContent={"center"}>
        <Input fullWidth placeholder="Type to chat" onChange={handleChange} onKeyDown={handleKeyDown} value={message} />
        <IconButton onClick={handleSend}>
          <SendIcon />
        </IconButton>
        <IconButton>
          <ClearIcon onClick={clearMessages} />
        </IconButton>
      </Stack>
    </Box>
  );
};
