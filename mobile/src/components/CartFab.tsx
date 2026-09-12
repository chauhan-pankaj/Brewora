import { useNavigate } from "react-router-dom";
import { useCart } from "../context/CartContext";

export function CartFab() {
  const { count } = useCart();
  const nav = useNavigate();
  if (!count) return null;
  return (
    <button
      onClick={() => nav("/cart")}
      style={{
        position: "absolute",
        right: 16,
        bottom: 72,
        zIndex: 9,
        width: 46,
        height: 46,
        borderRadius: 16,
        border: 0,
        background: "#173620",
        color: "#fff",
        fontSize: 12,
        fontWeight: 700,
        boxShadow: "0 8px 18px #0004",
      }}
    >
      🛒{count}
    </button>
  );
}
