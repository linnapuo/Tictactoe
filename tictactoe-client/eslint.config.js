import eslint from '@eslint/js';
import tseslint from 'typescript-eslint';

export default tseslint.config(
  {
    ignores: [
      'dist/**/*.ts',
      'dist/**',
      "**/*.mjs",
      "eslint.config.mjs",
      "**/*.js"
    ],
  },
  {
    files: [
      "src/**/*.ts", "src/**/*.tsx"
    ]
  },
  eslint.configs.recommended,
  tseslint.configs.strictTypeChecked,
  tseslint.configs.stylisticTypeChecked,
  {
    languageOptions: {
      parserOptions: {
        projectService: true,
        tsconfigRootDir: import.meta.dirname,
      },
    },
  }
);
