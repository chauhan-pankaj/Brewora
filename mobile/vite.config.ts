import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

const apiTarget = process.env.BREWORA_API_URL || "http://127.0.0.1:17421";

export default defineConfig({
  plugins: [react()],
  server: {
    host: "0.0.0.0",
    port: 43123,
    open: "http://localhost:43123",
    proxy: {
      "/api": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
      "/health": {
        target: apiTarget,
        changeOrigin: true,
        secure: false,
      },
    },
  },
  preview: {
    host: "0.0.0.0",
    port: 43123,
  },
});
