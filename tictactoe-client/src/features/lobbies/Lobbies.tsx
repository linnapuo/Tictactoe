import { Button, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { useEffect, useState } from "react";
import RefreshIcon from "@mui/icons-material/Refresh";
import { useGameClient } from "src/features/game/gameClient";
import { useNavigate } from "react-router-dom";
import { useErrorHandler } from "src/features/error/Error";

interface Lobby {
  gameId: string;
}

interface JoinGameProps {
  gameId: string;
}

async function fetchData(setData: (state: Lobby[]) => void) {
  const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/lobbies`);

  if (!response.ok) {
    setData([]);
  }

  const state = (await response.json()) as Lobby[];
  setData(state);
}

function JoinGame(props: JoinGameProps) {
  const { join } = useGameClient();
  const navigate = useNavigate();
  const errorHandler = useErrorHandler();

  const joinHandler = () =>
    join({ gameId: props.gameId })
      .then(() => navigate("/lobby"))
      .catch((e: unknown) => {
        errorHandler(e);
      });

  return <Button onClick={joinHandler}>Join</Button>;
}

export function Lobbies() {
  const [data, setData] = useState<Lobby[]>([]);

  useEffect(() => {
    fetchData(setData);
  }, []);

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>
              Games
              <Button onClick={() => fetchData(setData)}>
                <RefreshIcon />
              </Button>
            </TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {data.map((row) => (
            <TableRow key={row.gameId}>
              <TableCell>
                {row.gameId}
                <JoinGame gameId={row.gameId} />
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
