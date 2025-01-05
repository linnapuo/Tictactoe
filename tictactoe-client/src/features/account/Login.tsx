import { IconButton } from "@mui/material";
import AccountCircleIcon from "@mui/icons-material/AccountCircle";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import { FC, FormEvent, useState } from "react";
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
  return <div>Login</div>;
}

async function handleSubmit(e: FormEvent<HTMLFormElement>, username: string, password: string) {
  e.preventDefault();
  try {
    const response = await axios.post("https://localhost:7100/connect/authorize", {
      username,
      password,
    });

    console.log(response);
  } catch (e) {
    console.log(e);
  }
}

function LoginForm() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");

  return (
    <form
      id="login"
      onSubmit={(e) => {
        handleSubmit(e, username, password);
      }}
      data-testid="Login"
    >
      <div className="space-y-3">
        <div>
          <input
            name="username"
            id="username"
            type="text"
            required
            placeholder="Username or email"
            onChange={(event) => {
              setUsername(event.target.value);
            }}
          />
        </div>
        <div>
          <input
            name="password"
            id="password"
            type="password"
            required
            placeholder="Password"
            onChange={(event) => {
              setPassword(event.target.value);
            }}
          />
        </div>
        <button className="bg-green-500 text-white hover:bg-green-400" type="submit">
          Sign In
        </button>
      </div>
    </form>
  );
}

// eslint-disable-next-line @typescript-eslint/no-unused-vars
const TestLogin: FC = () => {
  return (
    <div>
      <LoginForm />
    </div>
  );
};

// eslint-disable-next-line @typescript-eslint/no-unused-vars
function AuthButton() {
  const auth = useAuth();

  const signIn = async () => {
    await auth.signinRedirect();
  };

  return (
    <button className="bg-green-500 text-white hover:bg-green-400" type="button" onClick={signIn}>
      Sign In
    </button>
  );
}
