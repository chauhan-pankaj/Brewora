import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { EmptyState } from "../components/Feedback";
import { useAuth } from "../context/AuthContext";
import { orderService } from "../services";
import type { Order } from "../types";
import { inr } from "../utils/format";

export function MyOrdersScreen() {
  const nav = useNavigate();
  const { token } = useAuth();
  const [orders, setOrders] = useState<Order[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!token) {
      setLoading(false);
      return;
    }
    orderService
      .myOrders()
      .then(setOrders)
      .catch((e: Error) => setError(e.message))
      .finally(() => setLoading(false));
  }, [token]);

  return (
    <Screen nav>
      <div className="pad">
        <Header title="My Orders" onBack={() => nav("/home")} />
        {!token ? (
          <EmptyState
            title="Sign in to see orders"
            action={
              <button className="primary" style={{ marginTop: 12 }} onClick={() => nav("/login")}>
                Sign in
              </button>
            }
          />
        ) : null}
        {error ? <div className="error-banner">{error}</div> : null}
        {!loading && token && orders.length === 0 ? (
          <EmptyState title="No orders yet" hint="Your first brew is waiting on the menu." />
        ) : (
          orders.map((o) => (
            <button
              key={o.orderId}
              className="box"
              style={{ width: "100%", textAlign: "left" }}
              onClick={() => nav(`/orders/${o.orderId}`)}
            >
              <h3>{o.orderNumber}</h3>
              <div className="row">
                <span>{o.orderStatus}</span>
                <b>{inr(o.totalAmount)}</b>
              </div>
            </button>
          ))
        )}
      </div>
    </Screen>
  );
}
