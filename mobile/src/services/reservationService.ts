import { api } from "./apiClient";
import type { Reservation, SlotAvailability } from "../types";

export type CreateReservationPayload = {
  customerName: string;
  email: string;
  phone: string;
  reservationDate: string;
  reservationTime: string;
  guestCount: number;
  specialRequest?: string;
};

export const reservationService = {
  create: (payload: CreateReservationPayload) =>
    api.post<Reservation>("/reservations", payload),
  availability: (date: string, guests: number) =>
    api.get<SlotAvailability[]>(
      `/reservations/availability?date=${encodeURIComponent(date)}&guests=${guests}`,
    ),
  mine: () => api.get<Reservation[]>("/reservations/my-reservations"),
};
