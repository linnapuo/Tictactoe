import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "src/index.css";
import { Provider } from "react-redux";
import { AppThemeProvider } from "src/app/theme";
import { AuthProvider, AuthProviderProps } from "react-oidc-context";
import { BrowserRouter } from "react-router-dom";
import { AppRoutes } from "src/app/routes";
import { CssBaseline } from "@mui/material";
import { store } from "src/app/store";

const oidcConfig: AuthProviderProps = {
  authority: import.meta.env.VITE_OIDC_AUTHORITY,
  redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URL,
  client_id: import.meta.env.VITE_OIDC_CLIENT_ID,
  post_logout_redirect_uri: import.meta.env.VITE_OIDC_REDIRECT_URL,
  onSigninCallback: () => history.replaceState({}, document.title, window.location.pathname),
};

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <Provider store={store}>
      <AppThemeProvider>
        <CssBaseline />
        <AuthProvider {...oidcConfig}>
          <BrowserRouter>
            <AppRoutes />
          </BrowserRouter>
        </AuthProvider>
      </AppThemeProvider>
    </Provider>
  </StrictMode>,
);
