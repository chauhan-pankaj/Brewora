import { STORAGE_KEYS } from "../constants/theme";
import type { ApiResult } from "../types";

export class ApiError extends Error {
  errors?: string[];
  constructor(message: string, errors?: string[]) {
    super(message);
    this.errors = errors;
  }
}

const CACHE_KEY = "brewora.apiBase";

function candidateBases(): string[] {
  const env = import.meta.env.VITE_API_BASE_URL;
  const host = window.location.hostname;
  const local = host === "localhost" || host === "127.0.0.1";
  const list = [
    env,
    local ? "http://localhost:17421/api" : `http://${host}:17421/api`,
    local ? "http://127.0.0.1:17421/api" : null,
    local ? "https://localhost:44356/api" : `https://${host}:44356/api`,
    "/api",
  ].filter((v, i, all): v is string => Boolean(v) && all.indexOf(v) === i);
  return list;
}

async function looksLikeApi(base: string): Promise<boolean> {
  try {
    const res = await fetch(`${base}/menu`, { method: "GET" });
    const text = await res.text();
    if (!text.startsWith("{") && !text.startsWith("[")) return false;
    const json = JSON.parse(text) as { success?: boolean };
    return json.success === true;
  } catch {
    return false;
  }
}

let resolvedBase: string | null = sessionStorage.getItem(CACHE_KEY);

async function getBase(): Promise<string> {
  if (resolvedBase) return resolvedBase;
  const cached = sessionStorage.getItem(CACHE_KEY);
  if (cached && (await looksLikeApi(cached))) {
    resolvedBase = cached;
    return cached;
  }
  for (const base of candidateBases()) {
    if (await looksLikeApi(base)) {
      resolvedBase = base;
      sessionStorage.setItem(CACHE_KEY, base);
      console.info("[brewora] API →", base);
      return base;
    }
  }
  throw new ApiError(
    "Brewora API se connect nahi ho paaya. Visual Studio mein Brewora.API run karo (http://localhost:17421/swagger).",
  );
}

function getToken() {
  return localStorage.getItem(STORAGE_KEYS.token);
}

export async function apiClient<T>(
  path: string,
  options: RequestInit = {},
): Promise<T> {
  const headers = new Headers(options.headers);
  if (!headers.has("Content-Type") && options.body) {
    headers.set("Content-Type", "application/json");
  }
  const token = getToken();
  if (token) headers.set("Authorization", `Bearer ${token}`);

  const base = await getBase();
  let res: Response;
  try {
    res = await fetch(`${base}${path}`, { ...options, headers });
  } catch {
    sessionStorage.removeItem(CACHE_KEY);
    resolvedBase = null;
    throw new ApiError("Unable to reach Brewora kitchen. Check your connection.");
  }

  const raw = await res.text();
  let json: ApiResult<T> | null = null;
  try {
    json = JSON.parse(raw) as ApiResult<T>;
  } catch {
    sessionStorage.removeItem(CACHE_KEY);
    resolvedBase = null;
    throw new ApiError(
      `API JSON nahi mili (${res.status}). Brewora.API chalu hai? ${base}${path}`,
    );
  }

  if (!res.ok || !json.success) {
    const fail = json as { message?: string; errors?: string[] };
    throw new ApiError(fail.message || "Unable to process request", fail.errors);
  }

  return json.data;
}

export const api = {
  get: <T>(path: string) => apiClient<T>(path),
  post: <T>(path: string, body?: unknown) =>
    apiClient<T>(path, { method: "POST", body: body ? JSON.stringify(body) : undefined }),
  put: <T>(path: string, body?: unknown) =>
    apiClient<T>(path, { method: "PUT", body: body ? JSON.stringify(body) : undefined }),
  del: <T>(path: string) => apiClient<T>(path, { method: "DELETE" }),
};
