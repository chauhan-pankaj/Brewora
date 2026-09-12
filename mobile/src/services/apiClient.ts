import { STORAGE_KEYS } from "../constants/theme";
import type { ApiResult } from "../types";

const BASE = import.meta.env.VITE_API_BASE_URL || "/api";

export class ApiError extends Error {
  errors?: string[];
  constructor(message: string, errors?: string[]) {
    super(message);
    this.errors = errors;
  }
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

  let res: Response;
  try {
    res = await fetch(`${BASE}${path}`, { ...options, headers });
  } catch {
    throw new ApiError("Unable to reach Brewora kitchen. Check your connection.");
  }

  const raw = await res.text();
  let json: ApiResult<T> | null = null;
  try {
    json = JSON.parse(raw) as ApiResult<T>;
  } catch {
    throw new ApiError(
      `Brewora API JSON nahi de rahi (${res.status}). Brewora.API chalu hai? Swagger: http://localhost:17421/swagger`,
    );
  }

  if (!res.ok || !json.success) {
    const fail = json as { message?: string; errors?: string[] };
    throw new ApiError(
      fail.message || "Unable to process request",
      fail.errors,
    );
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
