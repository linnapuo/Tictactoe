import { Typography } from "@mui/material";
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useAppSelector } from "src/app/hooks";
import { selectGame } from "src/features/game/gameSlice";

export function Lobby() {
    const game = useAppSelector(selectGame);
    const navigate = useNavigate();

    useEffect(() => {
        if (game.players.length === 2) {
            navigate("/game");
        }
    }, [game, navigate]);

    return (
        <div className="lobby">
            <Typography variant='h4' margin='2vmin'>Waiting for: {game.gameId}</Typography>
        </div>
    );
}