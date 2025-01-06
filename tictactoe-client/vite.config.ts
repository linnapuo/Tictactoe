import { defineConfig, loadEnv } from "vite";
import react from "@vitejs/plugin-react";
import tsconfigPaths from "vite-tsconfig-paths";

// https://vite.dev/config/
export default defineConfig(({ mode }) => {
  const env = loadEnv(mode, process.cwd(), "");

  return {
    plugins: [
      react({
        babel: {
          plugins: [["babel-plugin-react-compiler", {}]],
        },
      }),
      tsconfigPaths(),
    ],
    server: {
      open: true,
      port: parseInt(env.VITE_PORT),
    },
  };
});
