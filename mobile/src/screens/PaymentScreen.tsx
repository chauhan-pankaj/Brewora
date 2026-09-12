import { useState } from "react";
import { useLocation, useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { useCart } from "../context/CartContext";
import { useToast } from "../context/ToastContext";
import { paymentService } from "../services";
import type { Order } from "../types";
import { inr } from "../utils/format";
import { openRazorpay } from "../utils/razorpay";

export function PaymentScreen() {
  const loc = useLocation();
  const nav = useNavigate();
  const cart = useCart();
  const { push } = useToast();
  const order = (loc.state as { order?: Order; method?: string } | null)?.order;
  const method = (loc.state as { method?: string } | null)?.method || "Razorpay";
  const [busy, setBusy] = useState(false);

  if (!order) {
    return (
      <Screen>
        <div className="pad">
          <Header title="Payment" />
          <div className="error-banner">No order found. Return to checkout.</div>
        </div>
      </Screen>
    );
  }

  const current = order;

  async function completeCod() {
    cart.clear();
    nav("/order-success", { state: { order: current } });
  }

  async function payRazorpay() {
    setBusy(true);
    try {
      const created = await paymentService.createOrder(current.orderId);
      const result = await openRazorpay({
        key: created.keyId || import.meta.env.VITE_RAZORPAY_KEY_ID,
        amount: created.amount,
        currency: created.currency,
        name: "Brewora Café & Kitchen",
        description: created.orderNumber,
        order_id: created.razorpayOrderId,
        prefill: {
          name: current.customerName,
          email: current.customerEmail,
          contact: current.customerPhone,
        },
      });
      await paymentService.verify({
        breworaOrderId: current.orderId,
        razorpayOrderId: result.razorpay_order_id,
        razorpayPaymentId: result.razorpay_payment_id,
        razorpaySignature: result.razorpay_signature,
      });
      cart.clear();
      nav("/order-success", { state: { order: { ...current, paymentStatus: "Paid" } } });
    } catch (e) {
      push((e as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <Screen>
      <div className="pad">
        <Header title="Payment" />
        <div className="box">
          <h3>Pay for {order.orderNumber}</h3>
          <div className="row final">
            <span>Amount</span>
            <b>{inr(order.totalAmount)}</b>
          </div>
          <p className="address">
            Method: {method}. Card and UPI are processed in Razorpay test mode. Secrets never leave
            the Brewora kitchen.
          </p>
        </div>
        {method === "COD" ? (
          <button className="pay" onClick={completeCod}>
            Place Cash on Delivery Order
          </button>
        ) : (
          <button className="pay" disabled={busy} onClick={payRazorpay}>
            {busy ? "Opening Razorpay…" : "Pay with Razorpay"}
          </button>
        )}
      </div>
    </Screen>
  );
}
