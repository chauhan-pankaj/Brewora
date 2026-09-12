import { api } from "./apiClient";
import type { UserProfile } from "../types";

export const userService = {
  login: (email: string, password: string) =>
    api.post<{ token: string; user: UserProfile }>("/auth/login", { email, password }),
  register: (payload: {
    fullName: string;
    email: string;
    phone: string;
    password: string;
  }) => api.post<{ token: string; user: UserProfile }>("/auth/register", payload),
  profile: () => api.get<UserProfile>("/users/profile"),
  updateProfile: (payload: Partial<UserProfile>) =>
    api.put<UserProfile>("/users/profile", payload),
};
