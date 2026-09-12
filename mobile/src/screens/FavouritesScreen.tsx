import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { EmptyState } from "../components/Feedback";
import { useAuth } from "../context/AuthContext";
import { useCart } from "../context/CartContext";
import { useToast } from "../context/ToastContext";
import { favouriteService } from "../services";
import type { MenuItem } from "../types";
import { inr } from "../utils/format";

export function FavouritesScreen() {
  const nav = useNavigate();
  const { token } = useAuth();
  const { add } = useCart();
  const { push } = useToast();
  const [items, setItems] = useState<MenuItem[]>([]);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!token) return;
    favouriteService
      .list()
      .then(setItems)
      .catch((e: Error) => setError(e.message));
  }, [token]);

  return (
    <Screen nav>
      <div className="pad">
        <Header title="Favourites" onBack={() => nav("/home")} />
        {!token ? (
          <EmptyState
            title="Sign in to keep favourites"
            action={
              <button className="primary" style={{ marginTop: 12 }} onClick={() => nav("/login")}>
                Sign in
              </button>
            }
          />
        ) : null}
        {error ? <div className="error-banner">{error}</div> : null}
        {token && items.length === 0 ? (
          <EmptyState title="No favourites yet" hint="Tap the heart on a brew you love." />
        ) : (
          items.map((item) => (
            <div key={item.menuItemId} className="mi">
              <img src={item.imageUrl} alt={item.name} onClick={() => nav(`/menu/${item.menuItemId}`)} />
              <div className="miinfo" onClick={() => nav(`/menu/${item.menuItemId}`)}>
                <b>{item.name}</b>
                <p>{item.description}</p>
                <strong className="price">{inr(item.price)}</strong>
              </div>
              <button
                className="plus"
                onClick={() => {
                  add(item, "Small");
                  push("Added to cart");
                }}
              >
                +
              </button>
            </div>
          ))
        )}
      </div>
    </Screen>
  );
}
