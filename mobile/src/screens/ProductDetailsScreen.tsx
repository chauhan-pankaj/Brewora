import { useEffect, useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { StatusBar } from "../components/StatusBar";
import { useCart } from "../context/CartContext";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";
import { favouriteService, menuService } from "../services";
import type { CartLine, MenuItem } from "../types";
import { inr, sizeDelta } from "../utils/format";

export function ProductDetailsScreen() {
  const { id } = useParams();
  const nav = useNavigate();
  const { add } = useCart();
  const { token } = useAuth();
  const { push } = useToast();
  const [item, setItem] = useState<MenuItem | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [size, setSize] = useState<CartLine["size"]>("Small");
  const [qty, setQty] = useState(1);
  const [fav, setFav] = useState(false);

  useEffect(() => {
    if (!id) return;
    menuService
      .getById(Number(id))
      .then(setItem)
      .catch((e: Error) => setError(e.message));
  }, [id]);

  if (error) {
    return (
      <div className="screen pad">
        <StatusBar />
        <div className="error-banner">{error}</div>
      </div>
    );
  }
  if (!item) {
    return (
      <div className="screen pad">
        <StatusBar />
        <div className="skel" style={{ height: 330, borderRadius: 0 }} />
      </div>
    );
  }

  const price = item.price + sizeDelta(size);

  return (
    <div className="screen" style={{ overflow: "auto" }}>
      <StatusBar />
      <div className="productimg" style={{ backgroundImage: `url(${item.imageUrl})` }}>
        <div className="abs-actions">
          <button onClick={() => nav(-1)}>‹</button>
          <button
            onClick={async () => {
              if (!token) {
                nav("/login");
                return;
              }
              try {
                if (fav) await favouriteService.remove(item.menuItemId);
                else await favouriteService.add(item.menuItemId);
                setFav(!fav);
                push(fav ? "Removed from favourites" : "Saved to favourites");
              } catch (e) {
                push((e as Error).message);
              }
            }}
          >
            {fav ? "♥" : "♡"}
          </button>
        </div>
      </div>
      <div className="productbody">
        <div className="ptitle">
          <h2>{item.name}</h2>
          <b>{inr(price)}</b>
        </div>
        <p>{item.description}</p>
        <div className="label">Size</div>
        <div className="sizes">
          {(["Small", "Medium", "Large"] as const).map((s) => (
            <button key={s} className={size === s ? "active" : ""} onClick={() => setSize(s)}>
              {s}
            </button>
          ))}
        </div>
        <div className="qty">
          <button className="qbtn" onClick={() => setQty(Math.max(1, qty - 1))}>
            −
          </button>
          <b>{qty}</b>
          <button className="qbtn" onClick={() => setQty(qty + 1)}>
            +
          </button>
        </div>
        <button
          className="add"
          onClick={() => {
            add(item, size, qty);
            push("Added to cart");
            nav("/cart");
          }}
        >
          🛒 &nbsp; Add to Cart
        </button>
      </div>
    </div>
  );
}
