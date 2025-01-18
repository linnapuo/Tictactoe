import { Button, IconButton } from "@mui/material";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import { useNavigate } from "react-router-dom";
import { useAuth } from "react-oidc-context";

export function AccountButton() {
  const navigate = useNavigate();

  return (
    <IconButton onClick={() => navigate("/login")}>
      <AccountCircleIcon />
    </IconButton>
  );
}

export default function Login() {
  const auth = useAuth();

  if (auth.isAuthenticated) {
    return <Button onClick={() => auth.signoutRedirect()}>Logout</Button>;
  }

  return <Button onClick={() => auth.signinRedirect()}>Login</Button>;
}
