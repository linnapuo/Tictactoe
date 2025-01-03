import 'src/App.css';
import { useAppSelector } from 'src/app/hooks';
import { Outlet } from 'react-router-dom';
import { RenderError } from 'src/features/error/Error';
import { AppBar, CircularProgress, CssBaseline, Toolbar, Typography } from '@mui/material';
import { ToggleColorModeButton } from 'src/app/theme';
import { GameClientProvider, selectGameClient } from 'src/features/game/gameClient';
import { Chat } from 'src/features/chat/Chat';

const MyAppBar = () => (
  <AppBar>
    <Toolbar>
      <ToggleColorModeButton/>
      <Typography variant="h6" component="div" sx={{ flexGrow: 1 }}>
        Tic-Tac-Toe
      </Typography>
    </Toolbar>
  </AppBar>
);

function App() {
  const {connected, error} = useAppSelector(selectGameClient);

  return (
    <div className="App">
      <header className="App-header">
        <CssBaseline/>
        <RenderError/>
        <MyAppBar/>
        <GameClientProvider>
          {connected ? <Outlet /> : (!error ? <CircularProgress/> : <Typography variant='h3'>Failed to connect</Typography>)}
        </GameClientProvider>
        <Chat/>
      </header>
    </div>
  );
}

export default App;
