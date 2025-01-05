import eslint from "@eslint/js";
import tseslint from "typescript-eslint";
import pluginRefresh from "eslint-plugin-react-refresh";
import pluginReactHooks from "eslint-plugin-react-hooks";
import eslintConfigPrettier from "eslint-config-prettier";

export default tseslint.config(
  {
    ignores: ["dist/**/*.ts", "dist/**", "**/*.mjs", "eslint.config.js", "**/*.js", "vite.config.ts"],
  },
  {
    files: ["**/*.{js,mjs,cjs,ts,jsx,tsx}"],
  },
  eslint.configs.recommended,
  //pluginRefresh.configs.vite,
  ...tseslint.configs.recommended,
  {
    plugins: {
      "react-hooks": pluginReactHooks,
    },
    rules: {
      "react/react-in-jsx-scope": "off",
      ...pluginReactHooks.configs.recommended.rules,
    },
    ignores: ["*.test.tsx"],
  },
  {
    languageOptions: {
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
  },
  eslintConfigPrettier,
);
