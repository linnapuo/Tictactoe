import styled from "@emotion/styled";
import { useNavigate } from "react-router-dom";
import { useErrorHandler } from "src/features/error/Error";
import { useGameClient } from "src/features/game/gameClient";
import { EnterCodeDialog } from "src/features/home/EnterCodeDialog";
import { Typography } from "@mui/material";

const HomeText = styled(Typography)`
  margin: 10px;
`;

function CreateGame() {
  const { create } = useGameClient();
  const navigate = useNavigate();
  const errorHandler = useErrorHandler();

  const createHandler = (code: string) =>
    create({ gameId: code })
      .then(() => navigate("/lobby"))
      .catch((e: unknown) => {
        errorHandler(e);
      });

  return <EnterCodeDialog title="Create a new game" buttonText="Create" onCodeEntered={createHandler} />;
}

function JoinGame() {
  const { join } = useGameClient();
  const navigate = useNavigate();
  const errorHandler = useErrorHandler();

  const joinHandler = (code: string) =>
    join({ gameId: code })
      .then(() => navigate("/lobby"))
      .catch((e: unknown) => {
        errorHandler(e);
      });

  return <EnterCodeDialog title="Join to an existing game" buttonText="Join" onCodeEntered={joinHandler} />;
}

function SpectateGame() {
  const { spectate } = useGameClient();
  const navigate = useNavigate();
  const errorHandler = useErrorHandler();

  const spectateHandler = (code: string) =>
    spectate({ gameId: code })
      .then(() => navigate("/lobby"))
      .catch((e: unknown) => {
        errorHandler(e);
      });

  return <EnterCodeDialog title="Join to an existing game" buttonText="Spectate" onCodeEntered={spectateHandler} />;
}

export function Home() {
  return (
    <div className="home">
      <HomeText variant="h3">Create or join a game</HomeText>
      <CreateGame />
      <JoinGame />
      <SpectateGame />
    </div>
  );
}
