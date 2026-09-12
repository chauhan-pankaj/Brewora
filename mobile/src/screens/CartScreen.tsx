import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { ConfirmDialog, EmptyState } from "../components/Feedback";
import { useCart } from "../context/CartContext";
import { inr } from "../utils/format";

export function CartScreen() {
  const nav = useNavigate();
  const cart = useCart();
  const [confirm, setConfirm] = useState(false);

  return (
    <Screen>
      <div className="pad" style={{ paddingBottom: 90 }}>
        <Header
          title="Your Cart"
          right={
            cart.items.length ? (
              <button className="muted-link" onClick={() => setConfirm(true)}>
                Clear All
              </button>
            ) : null
          }
        />
        {cart.items.length === 0 ? (
          <EmptyState
            title="Your tray is empty"
            hint="Add a latte, a plate, or a little sweetness."
            action={
              <button className="primary" style={{ marginTop: 16 }} onClick={() => nav("/menu")}>
                Browse Menu
              </button>
            }
          />
        ) : (
          <>
            {cart.items.map((l) => (
              <div className="cartitem" key={l.key}>
                <img src={l.imageUrl} alt={l.name} />
                <div className="info">
                  <b>{l.name}</b>
                  <small>
                    {l.size} · {inr(l.unitPrice)}
                  </small>
                  <div className="counter">
                    <button className="mini" onClick={() => cart.setQty(l.key, l.quantity - 1)}>
                      −
                    </button>
                    <span>{l.quantity}</span>
                    <button className="mini" onClick={() => cart.setQty(l.key, l.quantity + 1)}>
                      +
                    </button>
                    <button className="muted-link" onClick={() => cart.remove(l.key)}>
                      Remove
                    </button>
                  </div>
                </div>
              </div>
            ))}
            <div className="totals">
              <div className="row">
                <span>Subtotal</span>
                <b>{inr(cart.subtotal)}</b>
              </div>
              <div className="row">
                <span>Delivery Charges</span>
                <b>{inr(cart.deliveryCharges)}</b>
              </div>
              {cart.discount ? (
                <div className="row">
                  <span>Discount</span>
                  <b>-{inr(cart.discount)}</b>
                </div>
              ) : null}
              {cart.tax ? (
                <div className="row">
                  <span>Taxes</span>
                  <b>{inr(cart.tax)}</b>
                </div>
              ) : null}
              <div className="row final">
                <span>Total</span>
                <b>{inr(cart.total)}</b>
              </div>
            </div>
          </>
        )}
      </div>
      {cart.items.length ? (
        <button className="checkout-bar" onClick={() => nav("/checkout")}>
          Proceed to Checkout
        </button>
      ) : null}
      <ConfirmDialog
        open={confirm}
        title="Clear cart?"
        message="Every item on this tray will be removed."
        confirmLabel="Clear All"
        onCancel={() => setConfirm(false)}
        onConfirm={() => {
          cart.clear();
          setConfirm(false);
        }}
      />
    </Screen>
  );
}
