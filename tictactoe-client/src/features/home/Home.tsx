import styled from '@emotion/styled';
import { LoadingButton } from '@mui/lab';
import { Dialog, DialogContent, DialogTitle, Typography } from '@mui/material';
import { FC, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useErrorHandler } from 'src/features/error/Error';
import { useGameClient } from 'src/features/game/gameClient';

const Input = styled.input`
    font-size: 18px;
    padding: 10px;
    margin: 10px;
    border: none;
    border-radius: 3px;
`;

const StyledButton = styled.button`
    min-width: 100%;
    font-size: 18px;
    padding: 10px;
    margin: 5px;
    border-radius: 7px;
    :hover {
        color: red;
    }
`;

const HomeText = styled(Typography)`
    margin: 10px;
`;

const EnterCodeDialog : FC<{
    onCodeEntered: (code: string) => Promise<void>, 
    title: string, 
    buttonText: string
}> = ({onCodeEntered, title, buttonText}) => {
    
    const [open, setOpen] = useState(false);

    const handleOpen = () => setOpen(true);
    const handleClose = () => setOpen(false);

    return (
        <>
            <Dialog open={open} onClose={handleClose}>
                <DialogTitle>{title}</DialogTitle>
                <DialogContent>
                    <EnterCode onCodeEntered={onCodeEntered}/>
                </DialogContent>
            </Dialog>
            <StyledButton onClick={handleOpen}>{buttonText}</StyledButton>
        </>
    );
}

const EnterCode : FC<{onCodeEntered: (code: string) => Promise<void>}> = ({onCodeEntered}) => {
    const [code, setCode] = useState("");
    const [loading, setLoading] = useState(false);

    const clickHandler = () => {
        setLoading(true);
        onCodeEntered(code).finally(() => setLoading(false));
    }

    return (
        <>
            <Input placeholder="Code" onChange={({target: {value}}) => setCode(value)}/>
            <LoadingButton onClick={clickHandler} loading={loading}>Enter</LoadingButton>
        </>
    );
}

const CreateGame = () => {
    const {create} = useGameClient();
    const navigate = useNavigate();
    const errorHandler = useErrorHandler();

    const createHandler = (code: string) => 
        create({ gameId: code })
            .then(() => navigate("/lobby"))
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            .catch((e: any) => errorHandler(e));

    return (
        <EnterCodeDialog 
                title="Create a new game"
                buttonText="Create"
                onCodeEntered={createHandler}
        />
    );
}

const JoinGame = () => {
    const {join} = useGameClient();
    const navigate = useNavigate();
    const errorHandler = useErrorHandler();

    const joinHandler = (code: string) => 
        join({ gameId: code })
            .then(() => navigate("/lobby"))
            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            .catch((e: any) => errorHandler(e));


    return (
        <EnterCodeDialog 
            title="Join to an existing game"
            buttonText="Join"
            onCodeEntered={joinHandler}
        />
    );
}

export function Home() {
    return (
        <div className="home">
            <HomeText variant='h3'>
                Create or join a game
            </HomeText>
            <CreateGame/>
            <JoinGame/>
        </div>
    );
}