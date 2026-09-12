import { api } from "./apiClient";
import type { CafeEvent, GalleryItem } from "../types";

export const galleryService = {
  getAll: () => api.get<GalleryItem[]>("/gallery"),
};

export const eventService = {
  getAll: () => api.get<CafeEvent[]>("/events"),
};

export const contactService = {
  send: (payload: { name: string; email: string; phone: string; message: string }) =>
    api.post<null>("/contact", payload),
};
