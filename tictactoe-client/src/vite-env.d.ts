/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_API_BASE_URL: string;
  readonly VITE_API_KEY: string;
  readonly VITE_OIDC_AUTHORITY: string;
  readonly VITE_OIDC_REDIRECT_URL: string;
  readonly VITE_OIDC_CLIENT_ID: string;
}

interface ImportMeta {
  readonly env: ImportMetaEnv;
}
