import { Button, Table, TableBody, TableCell, TableContainer, TableHead, TableRow } from "@mui/material";
import { useEffect, useState } from "react";
import DeleteIcon from "@mui/icons-material/Delete";
import { useAuth } from "react-oidc-context";

async function onClick(gameId: string, setData: (state: Lobby[]) => void, token: string | undefined) {
  const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/admin/lobbies/${gameId}`, {
    method: "DELETE",
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    return;
  }

  fetchData(setData, token);
}

interface Lobby {
  gameId: string;
}

async function fetchData(setData: (state: Lobby[]) => void, token: string | undefined) {
  const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/admin/lobbies`, {
    headers: {
      Authorization: `Bearer ${token}`,
    },
  });

  if (!response.ok) {
    setData([]);
  }

  const state = (await response.json()) as Lobby[];
  setData(state);
}

export function Admin() {
  const auth = useAuth();
  const [data, setData] = useState<Lobby[]>([]);
  const token = auth.user?.access_token;

  useEffect(() => {
    if (!token) {
      return;
    }
    fetchData(setData, token);
  }, [token]);

  return (
    <TableContainer>
      <Table>
        <TableHead>
          <TableRow>
            <TableCell>Games</TableCell>
          </TableRow>
        </TableHead>
        <TableBody>
          {data.map((row) => (
            <TableRow key={row.gameId}>
              <TableCell>
                {row.gameId}
                <Button onClick={() => onClick(row.gameId, setData, token)}>
                  <DeleteIcon />
                </Button>
              </TableCell>
            </TableRow>
          ))}
        </TableBody>
      </Table>
    </TableContainer>
  );
}
