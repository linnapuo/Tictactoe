import { useAppSelector } from "src/app/hooks";
import { Grid2, Typography } from "@mui/material";
import styled from "@emotion/styled";
import { SquareValue } from "src/features/game/gameApi";
import { useErrorHandler } from "src/features/error/Error";
import { selectGame } from "./gameSlice";
import { selectGameClient, useGameClient } from "./gameClient";
import { LoadingButton } from "@mui/lab";
import { useState } from "react";

interface SquareProps {
  value: SquareValue;
  onClick: () => Promise<void>;
}

interface BoardProps {
  squares: SquareValue[];
  onClick: (square: number) => Promise<void>;
}

function Square(props: SquareProps) {
  const [loading, setLoading] = useState(false);

  const LoadingSquare = styled(LoadingButton)`
    font-size: 80px;
    margin: 2px;
    min-width: 96px;
    min-height: 96px;
    max-width: 96px;
    max-height: 96px;

    &:hover {
      border-color: red;
    }
  `;

  const handleClick = () => {
    setLoading(true);

    props.onClick().finally(() => {
      setLoading(false);
    });
  };

  return (
    <LoadingSquare variant="outlined" className="square" onClick={handleClick} loading={loading}>
      {props.value}
    </LoadingSquare>
  );
}

function Board(props: BoardProps) {
  function renderSquare(i: number) {
    return <Square value={props.squares[i]} onClick={() => props.onClick(i)} />;
  }

  return (
    <Grid2 container className="board">
      {[0, 1, 2].map((y) => {
        return (
          <Grid2 container width={"100%"} key={y} justifyContent={"center"} className="board-row">
            {[0, 1, 2].map((x) => {
              return (
                <Grid2 key={x} textAlign={"center"}>
                  {renderSquare(x + 3 * y)}
                </Grid2>
              );
            })}
          </Grid2>
        );
      })}
    </Grid2>
  );
}

export function Game() {
  const game = useAppSelector(selectGame);
  const client = useAppSelector(selectGameClient);
  const errorHandler = useErrorHandler();
  const { move } = useGameClient();

  const status = game.players
    .filter((player) => player.name === client.connectionId)
    .map((player) => {
      const winner = calculateWinner(game.squares);

      const status = winner
        ? (winner === "X" && player.isX) || (winner === "O" && !player.isX)
          ? "You win"
          : "You lose"
        : (player.isX && game.xIsNext) || (!player.isX && !game.xIsNext)
          ? "Your turn"
          : "Wait for your turn";

      return status;
    });

  const moveHandler = (square: number) =>
    move({
      gameId: game.gameId,
      square,
    }).catch((e: unknown) => {
      errorHandler(e);
    });

  return (
    <Grid2 container justifyContent={"center"} className="game">
      <Board squares={game.squares} onClick={moveHandler} />
      <div className="game-info">
        <Typography variant="h4" marginTop="2vmin">
          {status}
        </Typography>
      </div>
    </Grid2>
  );
}

export function calculateWinner(squares: SquareValue[]) {
  const lines = [
    [0, 1, 2],
    [3, 4, 5],
    [6, 7, 8],
    [0, 3, 6],
    [1, 4, 7],
    [2, 5, 8],
    [0, 4, 8],
    [2, 4, 6],
  ];

  for (const [a, b, c] of lines) {
    if (squares[a] && squares[a] === squares[b] && squares[a] === squares[c]) {
      return squares[a];
    }
  }

  return undefined;
}
