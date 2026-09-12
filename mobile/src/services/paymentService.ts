import { api } from "./apiClient";

export type RazorpayOrder = {
  razorpayOrderId: string;
  amount: number;
  currency: string;
  keyId: string;
  breworaOrderId: number;
  orderNumber: string;
};

export type VerifyPaymentPayload = {
  breworaOrderId: number;
  razorpayOrderId: string;
  razorpayPaymentId: string;
  razorpaySignature: string;
};

export const paymentService = {
  createOrder: (breworaOrderId: number) =>
    api.post<RazorpayOrder>("/payment/create-order", { breworaOrderId }),
  verify: (payload: VerifyPaymentPayload) =>
    api.post<{ paid: boolean; orderNumber: string }>("/payment/verify", payload),
};
