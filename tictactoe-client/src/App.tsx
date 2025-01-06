import "src/App.css";
import { useAppSelector } from "src/app/hooks";
import { Outlet } from "react-router-dom";
import { RenderError } from "src/features/error/Error";
import { AppBar, CircularProgress, CssBaseline, Toolbar, Typography } from "@mui/material";
import { ToggleColorModeButton } from "src/app/theme";
import { GameClientProvider, selectGameClient } from "src/features/game/gameClient";
import { Chat } from "src/features/chat/Chat";
import { useState } from "react";
import { AccountButton } from "src/features/account/Login";

const debugMode = false;

const MyAppBar = () => (
  <AppBar>
    <Toolbar>
      <ToggleColorModeButton />
      <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
        Tic-Tac-Toe
      </Typography>
      <AccountButton />
    </Toolbar>
  </AppBar>
);

export default function App() {
  const [isDebug] = useState(debugMode);
  const { connected, error } = useAppSelector(selectGameClient);

  return (
    <div className="App">
      <CssBaseline />
      <header className="App-header">
        <RenderError />
        <MyAppBar />
      </header>
      <main>
        {isDebug ? (
          <Outlet />
        ) : (
          <GameClientProvider>
            {connected ? (
              <Outlet />
            ) : !error ? (
              <CircularProgress />
            ) : (
              <Typography variant="h3">Failed to connect</Typography>
            )}
          </GameClientProvider>
        )}
      </main>
      <footer style={{ marginTop: "2vmin" }}>
        <Chat />
      </footer>
    </div>
  );
}
