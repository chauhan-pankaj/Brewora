import { useLocation, useNavigate } from "react-router-dom";
import { StatusBar } from "../components/StatusBar";
import type { Order } from "../types";

export function OrderSuccessScreen() {
  const nav = useNavigate();
  const order = (useLocation().state as { order?: Order } | null)?.order;
  return (
    <div className="screen">
      <StatusBar />
      <div className="success">
        <div className="check">✓</div>
        <h2>
          Order Placed
          <br />
          Successfully!
        </h2>
        <p>
          Your order is on its way to make
          <br />
          your day better!
        </p>
        <div className="orderbox">
          <strong>Order #{order?.orderNumber || "BREW"}</strong>
          Estimated Delivery
          <br />
          <b>25 - 30 mins</b>
        </div>
        <button
          className="track"
          style={{ marginTop: 16 }}
          onClick={() => nav(order ? `/orders/${order.orderId}/track` : "/orders")}
        >
          Track Order
        </button>
        <button className="continue" onClick={() => nav("/menu")}>
          Continue Shopping
        </button>
      </div>
    </div>
  );
}
