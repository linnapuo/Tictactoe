import { Route, Routes } from "react-router-dom";
import App from "src/App";
import Login from "src/features/account/Login";
import { Game } from "src/features/game/Game";
import { Home } from "src/features/home/Home";
import { Lobby } from "src/features/lobby/Lobby";

export function AppRoutes() {
  return (
    <Routes>
      <Route path="*" element={<div>404 Not Found</div>} />
      <Route path="/" element={<App />}>
        <Route index element={<Home />} />
        <Route path="game" element={<Game />} />
        <Route path="lobby" element={<Lobby />} />
        <Route path="login" element={<Login />} />
      </Route>
    </Routes>
  );
}
