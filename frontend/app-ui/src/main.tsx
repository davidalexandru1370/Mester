import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./index.css";
import App from "./App.tsx";
import { DataStoreContextProvider } from "./context/DataStoreContext.tsx";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <DataStoreContextProvider>
      <App />
    </DataStoreContextProvider>
  </StrictMode>
);
