import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { EmptyState, SkeletonList } from "../components/Feedback";
import { IMAGES } from "../constants/images";
import { BRAND } from "../constants/theme";
import { useAuth } from "../context/AuthContext";
import { menuService } from "../services";
import type { MenuItem } from "../types";
import { greeting, inr } from "../utils/format";

const CATS = [
  { id: 1, name: "Coffee", icon: "☕" },
  { id: 2, name: "Food", icon: "🥐" },
  { id: 3, name: "Desserts", icon: "🍰" },
  { id: 4, name: "Beverages", icon: "🥤" },
];

export function HomeScreen() {
  const nav = useNavigate();
  const { user } = useAuth();
  const [featured, setFeatured] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [q, setQ] = useState("");

  useEffect(() => {
    menuService
      .featured()
      .then(setFeatured)
      .catch((e: Error) => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  return (
    <Screen nav className="home">
      <div className="pad">
        <div className="homehead">
          <button className="location" style={{ background: "none", border: 0, textAlign: "left" }} onClick={() => nav("/drawer")}>
            Location
            <b>{BRAND.location}⌄</b>
          </button>
          <div style={{ display: "flex", gap: 10, alignItems: "center" }}>
            <button className="icon-btn" onClick={() => nav("/offers")} aria-label="Notifications">
              ⌕
            </button>
            <img
              className="avatar"
              src={user?.avatarUrl || IMAGES.avatar}
              alt=""
              onClick={() => nav("/profile")}
            />
          </div>
        </div>
        <div className="greet">{greeting()}</div>
        <h2 className="hero">
          Let's Brew
          <br />
          Happiness ☕
        </h2>
        <form
          className="search"
          onSubmit={(e) => {
            e.preventDefault();
            nav(`/menu?q=${encodeURIComponent(q)}`);
          }}
        >
          ⌕
          <input
            placeholder="Search for coffee, food, desserts..."
            value={q}
            onChange={(e) => setQ(e.target.value)}
          />
        </form>
        <div className="cats">
          {CATS.map((c) => (
            <button key={c.id} className="cat" onClick={() => nav(`/menu?category=${c.id}`)}>
              <div className="ci">{c.icon}</div>
              {c.name}
            </button>
          ))}
        </div>
        <div className="promo" style={{ ["--promo-img" as string]: `url('${IMAGES.promo}')` }}>
          <b>FLAT 20% OFF</b>
          <p>On Your First Order</p>
          <button onClick={() => nav("/menu")}>Order Now</button>
        </div>
        <div className="sect">
          <h3>Popular Today</h3>
          <button onClick={() => nav("/menu")}>See All</button>
        </div>
        {loading ? <SkeletonList /> : null}
        {error ? (
          <div className="error-banner">
            {error}
            <div>
              <button className="muted-link" onClick={() => window.location.reload()}>
                Try again
              </button>
            </div>
          </div>
        ) : null}
        {!loading && !error && featured.length === 0 ? (
          <EmptyState title="The bar is still warming up" hint="Popular brews will appear here." />
        ) : (
          <div className="popular">
            {featured.map((p) => (
              <button key={p.menuItemId} className="food" onClick={() => nav(`/menu/${p.menuItemId}`)}>
                <img src={p.imageUrl} alt={p.name} />
                <b>{p.name}</b>
                <small>{inr(p.price)}</small>
              </button>
            ))}
          </div>
        )}
      </div>
    </Screen>
  );
}
