import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import { store } from "./app/store";
import { Provider } from "react-redux";
import { AppRoutes } from "./app/routes";
import { AppThemeProvider } from "./app/theme";
import { BrowserRouter } from "react-router-dom";

// eslint-disable-next-line @typescript-eslint/no-non-null-assertion
const root = document.getElementById("root")!;

createRoot(root).render(
  <StrictMode>
    <Provider store={store}>
      <AppThemeProvider>
        <BrowserRouter children={<AppRoutes />} />
      </AppThemeProvider>
    </Provider>
  </StrictMode>,
);
