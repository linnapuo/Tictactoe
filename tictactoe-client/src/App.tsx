import "src/App.css";
import { Outlet } from "react-router-dom";
import { RenderError } from "src/features/error/Error";
import { AppBar, Toolbar, Typography } from "@mui/material";
import { ToggleColorModeButton } from "src/app/theme";
import { Chat } from "src/features/chat/Chat";
import { AccountButton } from "src/features/account/Login";
import { GameClientProvider } from "src/features/game/gameClient";

function TopNavBar() {
  return (
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
}

function Header() {
  return (
    <header className="App-header">
      <RenderError />
      <TopNavBar />
    </header>
  );
}

function Footer() {
  return (
    <footer style={{ marginTop: "2vmin" }}>
      <Chat />
    </footer>
  );
}

export default function App() {
  return (
    <div className="App">
      <Header />
      <main>
        <GameClientProvider>
          <Outlet />
        </GameClientProvider>
      </main>
      <Footer />
    </div>
  );
}
