import { AuthProviderProps } from "react-oidc-context";

export const oidcConfig: AuthProviderProps = {
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URL,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
  post_logout_redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URL,
  onSigninCallback: () => history.replaceState({}, document.title, window.location.pathname),
};

export const authUrl = `${import.meta.env.VITE_API_BASE_URL}/.auth/login/aad?post_login_redirect_uri=${import.meta.env.VITE_OIDC_REDIRECT_URL}`;
