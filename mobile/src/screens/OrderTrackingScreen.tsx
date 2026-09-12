import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { orderService } from "../services";
import type { Order, OrderStatus } from "../types";

const STEPS: { key: OrderStatus; label: string; hint: string }[] = [
  { key: "Pending", label: "Order Placed", hint: "We've received your order" },
  { key: "Confirmed", label: "Confirmed", hint: "The kitchen has your ticket" },
  { key: "Preparing", label: "Preparing", hint: "Brewing and plating" },
  { key: "Ready", label: "Ready", hint: "Packed with care" },
  { key: "Delivered", label: "Delivered", hint: "Enjoy every sip" },
];

const ORDER_INDEX: Record<string, number> = {
  Pending: 0,
  Confirmed: 1,
  Preparing: 2,
  Ready: 3,
  OutForDelivery: 3,
  Delivered: 4,
  Cancelled: -1,
};

export function OrderTrackingScreen() {
  const { id } = useParams();
  const nav = useNavigate();
  const [order, setOrder] = useState<Order | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    orderService
      .getById(Number(id))
      .then(setOrder)
      .catch((e: Error) => setError(e.message));
  }, [id]);

  const idx = ORDER_INDEX[order?.orderStatus || "Pending"] ?? 0;

  return (
    <Screen>
      <div className="pad">
        <Header title="Track Order" onBack={() => nav("/orders")} />
        {error ? <div className="error-banner">{error}</div> : null}
        <div className="box">
          <h3>{order?.orderNumber || "…"}</h3>
          <p className="address">Status: {order?.orderStatus || "Loading"}</p>
        </div>
        {order?.orderStatus === "Cancelled" ? (
          <div className="error-banner">This order was cancelled.</div>
        ) : (
          <div className="timeline">
            {STEPS.map((s, i) => (
              <div className="t-item" key={s.key}>
                <div className="t-rail">
                  <div className={`t-dot ${i <= idx ? "done" : ""}`} />
                  {i < STEPS.length - 1 ? <div className="t-line" /> : null}
                </div>
                <div>
                  <b>{s.label}</b>
                  <span>{s.hint}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </Screen>
  );
}
