import { Button, Table, TableRow } from "@mui/material";
import { useEffect, useState } from "react";
import DeleteIcon from '@mui/icons-material/Delete';

async function onClick(gameId: string, setData: (state: Lobby[]) => void) {

  const response = await fetch(
    `${import.meta.env.VITE_API_BASE_URL}/admin/lobbies/${gameId}`,
    {
      method: "DELETE"
    }
  );

  if (!response.ok) {
    return;
  }

  fetchData(setData);

}

function LobbyRow({ gameId, setData }: { gameId: string, setData: (state: Lobby[]) => void }) {
  return <TableRow>
    {gameId}
    <Button onClick={() => onClick(gameId, setData)}>
      <DeleteIcon />
    </Button>
  </TableRow>;
}

interface Lobby {
  gameId: string
}

async function fetchData(setData: (state: Lobby[]) => void) {
  const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/admin/lobbies`);

  if (!response.ok) {
    setData([]);
  }

  const state = await response.json() as Lobby[];
  setData(state);
}

export function Admin() {

  const [data, setData] = useState<Lobby[]>([]);

  useEffect(() => {
    fetchData(setData);
  }, []);

  if (!data.length) {
    return <div>Empty</div>
  }

  return <Table>{data.map(d => <LobbyRow gameId={d.gameId} setData={setData} />)}</Table>;
}