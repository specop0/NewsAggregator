import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import React from "react";
import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import { AuthProvider, AuthProviderProps } from "react-oidc-context";

const root = ReactDOM.createRoot(
  document.getElementById("root") as HTMLElement
);
const baseUrl = import.meta.env.BASE_URL;

const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      refetchOnWindowFocus: false,
      retry(failureCount, error) {
        return false;
      },
    },
  },
});

const oidcConfig: AuthProviderProps = {
  authority: window.configuration.openIdConnect.authority,
  client_id: window.configuration.openIdConnect.clientId,
  redirect_uri: window.configuration.openIdConnect.redirectUri,
  onSigninCallback: () => {
    window.history.replaceState(
      {},
      document.title,
      window.location.pathname,
    )
  }
};

root.render(
  <React.StrictMode>
    <AuthProvider {...oidcConfig}>
      <QueryClientProvider client={queryClient}>
        <BrowserRouter basename={baseUrl}>
          <App />
        </BrowserRouter>
      </QueryClientProvider>
    </AuthProvider>
  </React.StrictMode>
);
