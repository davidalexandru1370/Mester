import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { DataStoreContextProvider } from "./context/DataStoreContext.tsx";
import { UserContextProvider } from "./context/UserContext.tsx";
import { AuthProvider } from "react-auth-kit";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <UserContextProvider>
      <DataStoreContextProvider>
        <AuthProvider
          authType={"cookie"}
          authName={"_auth"}
          cookieSecure={true}
          cookieDomain="localhost"
        >
          <App />
        </AuthProvider>
      </DataStoreContextProvider>
    </UserContextProvider>
  </StrictMode>
);
