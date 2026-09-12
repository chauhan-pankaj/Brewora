import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { useAuth } from "../context/AuthContext";
import { useCart } from "../context/CartContext";
import { useToast } from "../context/ToastContext";
import { orderService } from "../services";
import { inr } from "../utils/format";
import { BRAND } from "../constants/theme";

export function CheckoutScreen() {
  const nav = useNavigate();
  const cart = useCart();
  const { user } = useAuth();
  const { push } = useToast();
  const [name, setName] = useState(user?.fullName || "");
  const [phone, setPhone] = useState(user?.phone || "");
  const [email, setEmail] = useState(user?.email || "");
  const [address, setAddress] = useState(user?.address || BRAND.location);
  const [method, setMethod] = useState("Razorpay");
  const [busy, setBusy] = useState(false);

  const methods = [
    { id: "Razorpay", label: "UPI / Card / NetBanking" },
    { id: "UPI", label: "UPI" },
    { id: "Card", label: "Credit / Debit Card" },
    { id: "COD", label: "Cash on Delivery" },
  ];

  async function pay() {
    if (!cart.items.length) return;
    if (!name || !phone || !email || !address) {
      push("Please complete your delivery details.");
      return;
    }
    setBusy(true);
    try {
      const order = await orderService.create({
        customerName: name,
        customerEmail: email,
        customerPhone: phone,
        deliveryAddress: address,
        paymentMethod: method,
        items: cart.items.map((l) => ({
          menuItemId: l.menuItemId,
          quantity: l.quantity,
          size: l.size,
        })),
      });
      nav("/payment", { state: { order, method } });
    } catch (e) {
      push((e as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <Screen>
      <div className="pad">
        <Header title="Checkout" />
        <div className="box">
          <h3>Delivery Details</h3>
          <div className="field">
            <label>NAME</label>
            <input value={name} onChange={(e) => setName(e.target.value)} />
          </div>
          <div className="field">
            <label>MOBILE</label>
            <input value={phone} onChange={(e) => setPhone(e.target.value)} />
          </div>
          <div className="field">
            <label>EMAIL</label>
            <input value={email} onChange={(e) => setEmail(e.target.value)} />
          </div>
          <div className="field">
            <label>DELIVERY ADDRESS</label>
            <textarea rows={3} value={address} onChange={(e) => setAddress(e.target.value)} />
          </div>
        </div>
        <div className="box">
          <h3>Payment Method</h3>
          {methods.map((m) => (
            <button key={m.id} className="payrow" onClick={() => setMethod(m.id)}>
              {m.label} <b>{method === m.id ? "●" : "○"}</b>
            </button>
          ))}
        </div>
        <div className="box">
          <h3>Order Summary</h3>
          {cart.items.map((l) => (
            <div className="row" key={l.key}>
              <span>
                {l.name} × {l.quantity}
              </span>
              <b>{inr(l.unitPrice * l.quantity)}</b>
            </div>
          ))}
        </div>
        <div className="row final">
          <span>Total Amount</span>
          <b>{inr(cart.total)}</b>
        </div>
        <p className="address" style={{ marginTop: 8 }}>
          Final amount is calculated securely by Brewora using menu prices.
        </p>
        <button className="pay" disabled={busy} onClick={pay}>
          🔒 &nbsp; {busy ? "Placing order…" : "Pay Now Securely"}
        </button>
      </div>
    </Screen>
  );
}
