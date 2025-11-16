import styled from "@emotion/styled";
import { Button, Dialog, DialogTitle, DialogContent } from "@mui/material";
import { useState } from "react";
import { useErrorHandler } from "src/features/error/Error";

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

interface CodeDialogProps {
  onCodeEntered: (code: string) => Promise<void>;
  title: string;
  buttonText: string;
}

interface EnterCodeProps {
  onCodeEntered: (code: string) => Promise<void>;
}

function EnterCode({ onCodeEntered }: EnterCodeProps) {
  const [code, setCode] = useState("");
  const [loading, setLoading] = useState(false);
  const errorHandler = useErrorHandler();

  const clickHandler = () => {
    if (!code) {
      errorHandler("Code should not be empty");
      return;
    }

    setLoading(true);

    onCodeEntered(code).finally(() => {
      setLoading(false);
    });
  };

  return (
    <>
      <Input
        placeholder="Code"
        onChange={({ target: { value } }) => {
          setCode(value);
        }}
      />
      <Button onClick={clickHandler} loading={loading}>
        Enter
      </Button>
    </>
  );
}

export function EnterCodeDialog({ onCodeEntered, title, buttonText }: CodeDialogProps) {
  const [open, setOpen] = useState(false);

  const handleOpen = () => {
    setOpen(true);
  };
  const handleClose = () => {
    setOpen(false);
  };

  return (
    <>
      <Dialog open={open} onClose={handleClose}>
        <DialogTitle>{title}</DialogTitle>
        <DialogContent>
          <EnterCode onCodeEntered={onCodeEntered} />
        </DialogContent>
      </Dialog>
      <StyledButton onClick={handleOpen}>{buttonText}</StyledButton>
    </>
  );
}
