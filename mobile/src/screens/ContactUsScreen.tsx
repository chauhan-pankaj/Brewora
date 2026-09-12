import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { useToast } from "../context/ToastContext";
import { contactService } from "../services";

export function ContactUsScreen() {
  const nav = useNavigate();
  const { push } = useToast();
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [message, setMessage] = useState("");
  const [busy, setBusy] = useState(false);

  async function send() {
    setBusy(true);
    try {
      await contactService.send({ name, email, phone, message });
      push("Message received. We'll write back soon.");
      setMessage("");
    } catch (e) {
      push((e as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <Screen>
      <div className="pad">
        <Header title="Contact Us" onBack={() => nav("/home")} />
        <p className="address">Indiranagar, Bangalore · sip@brewora.cafe · +91 80 4000 1200</p>
        <div className="group">
          <label>NAME</label>
          <input className="input" value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div className="group">
          <label>EMAIL</label>
          <input className="input" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="group">
          <label>MOBILE</label>
          <input className="input" value={phone} onChange={(e) => setPhone(e.target.value)} />
        </div>
        <div className="group">
          <label>MESSAGE</label>
          <textarea className="input" rows={4} value={message} onChange={(e) => setMessage(e.target.value)} />
        </div>
        <button className="book" disabled={busy} onClick={send}>
          Send Message
        </button>
      </div>
    </Screen>
  );
}
