import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { StatusBar, Style } from "@capacitor/status-bar";
import App from "./App";
import "./index.css";

createRoot(document.getElementById("root")!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);

void StatusBar.setStyle({ style: Style.Dark }).catch(() => undefined);
