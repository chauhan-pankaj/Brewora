import { useEffect, useMemo, useState } from "react";
import { useNavigate, useSearchParams } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { EmptyState, SkeletonList } from "../components/Feedback";
import { useCart } from "../context/CartContext";
import { useToast } from "../context/ToastContext";
import { categoryService, menuService } from "../services";
import type { Category, MenuItem } from "../types";
import { inr } from "../utils/format";

export function MenuScreen() {
  const nav = useNavigate();
  const [params] = useSearchParams();
  const { add } = useCart();
  const { push } = useToast();
  const [cats, setCats] = useState<Category[]>([]);
  const [items, setItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [cat, setCat] = useState<number | "all">(
    params.get("category") ? Number(params.get("category")) : "all",
  );
  const q = (params.get("q") || "").toLowerCase();

  useEffect(() => {
    Promise.all([menuService.getAll(), categoryService.getAll()])
      .then(([m, c]) => {
        setItems(m);
        setCats(c);
      })
      .catch((e: Error) => setError(e.message))
      .finally(() => setLoading(false));
  }, []);

  const visible = useMemo(
    () =>
      items.filter((i) => {
        const okCat = cat === "all" || i.categoryId === cat;
        const okQ = !q || i.name.toLowerCase().includes(q) || i.description.toLowerCase().includes(q);
        return okCat && okQ;
      }),
    [items, cat, q],
  );

  return (
    <Screen nav>
      <div className="pad">
        <Header title="Our Menu" onBack={() => nav("/home")} right={<span>⌕</span>} />
        <div className="filters">
          <button className={cat === "all" ? "selected" : ""} onClick={() => setCat("all")}>
            All
          </button>
          {(cats.length
            ? cats
            : [
                { categoryId: 1, name: "Coffee", slug: "coffee", icon: "☕" },
                { categoryId: 2, name: "Food", slug: "food", icon: "🥐" },
                { categoryId: 3, name: "Desserts", slug: "desserts", icon: "🍰" },
                { categoryId: 4, name: "Beverages", slug: "beverages", icon: "🥤" },
              ]
          ).map((c) => (
            <button
              key={c.categoryId}
              className={cat === c.categoryId ? "selected" : ""}
              onClick={() => setCat(c.categoryId)}
            >
              {c.name}
            </button>
          ))}
        </div>
        {loading ? <SkeletonList /> : null}
        {error ? <div className="error-banner">{error}</div> : null}
        {!loading && visible.length === 0 ? (
          <EmptyState title="Nothing on this tray" hint="Try another category." />
        ) : (
          visible.map((item) => (
            <div key={item.menuItemId} className="mi" role="button" onClick={() => nav(`/menu/${item.menuItemId}`)}>
              <img src={item.imageUrl} alt={item.name} />
              <div className="miinfo">
                <b>{item.name}</b>
                <p>{item.description}</p>
                <strong className="price">{inr(item.price)}</strong>
              </div>
              <button
                className="plus"
                onClick={(e) => {
                  e.stopPropagation();
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
