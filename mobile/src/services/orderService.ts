import { api } from "./apiClient";
import type { Order } from "../types";

export type CreateOrderPayload = {
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  deliveryAddress: string;
  paymentMethod: string;
  items: { menuItemId: number; quantity: number; size: string }[];
};

export const orderService = {
  create: (payload: CreateOrderPayload) => api.post<Order>("/orders", payload),
  getById: (id: number) => api.get<Order>(`/orders/${id}`),
  myOrders: () => api.get<Order[]>("/orders/my-orders"),
};
