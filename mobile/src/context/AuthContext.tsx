import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { STORAGE_KEYS } from "../constants/theme";
import type { UserProfile } from "../types";
import { userService } from "../services";

type AuthValue = {
  token: string | null;
  user: UserProfile | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (p: {
    fullName: string;
    email: string;
    phone: string;
    password: string;
  }) => Promise<void>;
  logout: () => void;
  refreshProfile: () => Promise<void>;
};

const Ctx = createContext<AuthValue | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setToken] = useState<string | null>(() =>
    localStorage.getItem(STORAGE_KEYS.token),
  );
  const [user, setUser] = useState<UserProfile | null>(() => {
    const raw = localStorage.getItem(STORAGE_KEYS.user);
    return raw ? (JSON.parse(raw) as UserProfile) : null;
  });
  const [loading, setLoading] = useState(Boolean(token));

  const persist = (t: string, u: UserProfile) => {
    setToken(t);
    setUser(u);
    localStorage.setItem(STORAGE_KEYS.token, t);
    localStorage.setItem(STORAGE_KEYS.user, JSON.stringify(u));
  };

  const logout = useCallback(() => {
    setToken(null);
    setUser(null);
    localStorage.removeItem(STORAGE_KEYS.token);
    localStorage.removeItem(STORAGE_KEYS.user);
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const data = await userService.login(email, password);
    persist(data.token, data.user);
  }, []);

  const register = useCallback(
    async (p: { fullName: string; email: string; phone: string; password: string }) => {
      const data = await userService.register(p);
      persist(data.token, data.user);
    },
    [],
  );

  const refreshProfile = useCallback(async () => {
    if (!token) return;
    const profile = await userService.profile();
    setUser(profile);
    localStorage.setItem(STORAGE_KEYS.user, JSON.stringify(profile));
  }, [token]);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }
    refreshProfile()
      .catch(() => logout())
      .finally(() => setLoading(false));
  }, [token, refreshProfile, logout]);

  const value = useMemo(
    () => ({ token, user, loading, login, register, logout, refreshProfile }),
    [token, user, loading, login, register, logout, refreshProfile],
  );

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useAuth() {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error("AuthProvider missing");
  return ctx;
}
