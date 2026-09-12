import { api } from "./apiClient";
import type { Category, MenuItem } from "../types";

export const menuService = {
  getAll: () => api.get<MenuItem[]>("/menu"),
  getById: (id: number) => api.get<MenuItem>(`/menu/${id}`),
  getByCategory: (categoryId: number) =>
    api.get<MenuItem[]>(`/menu/category/${categoryId}`),
  featured: () => api.get<MenuItem[]>("/menu/featured"),
};

export const categoryService = {
  getAll: () => api.get<Category[]>("/categories"),
};
