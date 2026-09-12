import http from "node:http";
import https from "node:https";
import type { IncomingMessage, ServerResponse } from "node:http";
import type { Plugin } from "vite";
import react from "@vitejs/plugin-react";
import { defineConfig } from "vite";

const API_CANDIDATES = [
  process.env.BREWORA_API_URL,
  "http://127.0.0.1:17421",
  "http://localhost:17421",
  "https://127.0.0.1:44356",
  "https://localhost:44356",
  "http://127.0.0.1:5003",
  "http://localhost:5003",
].filter((v, i, all): v is string => Boolean(v) && all.indexOf(v) === i);

function probe(base: string): Promise<boolean> {
  return new Promise((resolve) => {
    const url = new URL("/health", base);
    const lib = url.protocol === "https:" ? https : http;
    const req = lib.request(
      {
        hostname: url.hostname,
        port: url.port,
        path: url.pathname,
        method: "GET",
        timeout: 900,
        rejectUnauthorized: false,
      },
      (res) => {
        res.resume();
        resolve((res.statusCode ?? 500) < 500);
      },
    );
    req.on("error", () => resolve(false));
    req.on("timeout", () => {
      req.destroy();
      resolve(false);
    });
    req.end();
  });
}

async function pickApiBase(): Promise<string> {
  for (const base of API_CANDIDATES) {
    if (await probe(base)) {
      console.log(`[brewora] API proxy → ${base}`);
      return base;
    }
  }
  console.warn("[brewora] No API answered /health. Proxying to http://127.0.0.1:17421");
  return "http://127.0.0.1:17421";
}

function breworaApiProxy(): Plugin {
  let target = "http://127.0.0.1:17421";
  return {
    name: "brewora-api-proxy",
    async configureServer(server) {
      target = await pickApiBase();
      server.middlewares.use((req, res, next) => {
        if (!req.url?.startsWith("/api") && req.url !== "/health") return next();
        forward(req, res, target, () => {
          if (!res.headersSent) {
            res.statusCode = 502;
            res.setHeader("Content-Type", "application/json");
            res.end(
              JSON.stringify({
                success: false,
                message:
                  "Brewora API is not reachable. Start Brewora.API (port 17421 or IIS Express 44356).",
              }),
            );
          }
        });
      });
    },
  };
}

function forward(
  req: IncomingMessage,
  res: ServerResponse,
  targetBase: string,
  onError: () => void,
) {
  const target = new URL(req.url || "/", targetBase);
  const lib = target.protocol === "https:" ? https : http;
  const headers = { ...req.headers, host: target.host };
  const pReq = lib.request(
    {
      hostname: target.hostname,
      port: target.port,
      path: `${target.pathname}${target.search}`,
      method: req.method,
      headers,
      rejectUnauthorized: false,
    },
    (pRes) => {
      res.writeHead(pRes.statusCode ?? 502, pRes.headers);
      pRes.pipe(res);
    },
  );
  pReq.on("error", onError);
  req.pipe(pReq);
}

export default defineConfig({
  plugins: [react(), breworaApiProxy()],
  server: {
    host: "0.0.0.0",
    port: 43123,
    open: "http://localhost:43123",
  },
  preview: {
    host: "0.0.0.0",
    port: 43123,
  },
});
