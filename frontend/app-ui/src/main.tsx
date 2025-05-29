import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { DataStoreContextProvider } from "./context/DataStoreContext.tsx";
import { UserContextProvider } from "./context/UserContext.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <UserContextProvider>
      <DataStoreContextProvider>
        <App />
      </DataStoreContextProvider>
    </UserContextProvider>
  </StrictMode>
);
