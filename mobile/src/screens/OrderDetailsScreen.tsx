import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { orderService } from "../services";
import type { Order } from "../types";
import { inr } from "../utils/format";

export function OrderDetailsScreen() {
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

  return (
    <Screen>
      <div className="pad">
        <Header title="Order Details" />
        {error ? <div className="error-banner">{error}</div> : null}
        {order ? (
          <>
            <div className="box">
              <h3>{order.orderNumber}</h3>
              <p className="address">
                {order.customerName}
                <br />
                {order.deliveryAddress}
                <br />
                {order.orderStatus} · {order.paymentStatus}
              </p>
            </div>
            <div className="box">
              {order.items?.map((it) => (
                <div className="row" key={it.orderItemId}>
                  <span>
                    {it.name || `Item ${it.menuItemId}`} × {it.quantity}
                  </span>
                  <b>{inr(it.totalPrice)}</b>
                </div>
              ))}
              <div className="row">
                <span>Subtotal</span>
                <b>{inr(order.subtotal)}</b>
              </div>
              <div className="row">
                <span>Delivery</span>
                <b>{inr(order.deliveryCharges)}</b>
              </div>
              <div className="row">
                <span>Discount</span>
                <b>-{inr(order.discount)}</b>
              </div>
              <div className="row final">
                <span>Total</span>
                <b>{inr(order.totalAmount)}</b>
              </div>
            </div>
            <button className="track" onClick={() => nav(`/orders/${order.orderId}/track`)}>
              Track Order
            </button>
          </>
        ) : null}
      </div>
    </Screen>
  );
}
