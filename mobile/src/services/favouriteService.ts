import { api } from "./apiClient";
import type { MenuItem } from "../types";

export const favouriteService = {
  list: () => api.get<MenuItem[]>("/favourites"),
  add: (menuItemId: number) => api.post<null>("/favourites", { menuItemId }),
  remove: (menuItemId: number) => api.del<null>(`/favourites/${menuItemId}`),
};
