import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { useAuth } from "../context/AuthContext";
import { useToast } from "../context/ToastContext";

export function LoginScreen() {
  const nav = useNavigate();
  const { login, register } = useAuth();
  const { push } = useToast();
  const [mode, setMode] = useState<"in" | "up">("in");
  const [email, setEmail] = useState("customer@email.com");
  const [password, setPassword] = useState("Customer@123");
  const [fullName, setFullName] = useState("");
  const [phone, setPhone] = useState("");
  const [busy, setBusy] = useState(false);

  async function submit() {
    setBusy(true);
    try {
      if (mode === "in") await login(email, password);
      else await register({ fullName, email, phone, password });
      nav("/home");
    } catch (e) {
      push((e as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <Screen>
      <div className="pad">
        <Header title={mode === "in" ? "Sign in" : "Create account"} />
        {mode === "up" ? (
          <>
            <div className="group">
              <label>NAME</label>
              <input className="input" value={fullName} onChange={(e) => setFullName(e.target.value)} />
            </div>
            <div className="group">
              <label>MOBILE</label>
              <input className="input" value={phone} onChange={(e) => setPhone(e.target.value)} />
            </div>
          </>
        ) : null}
        <div className="group">
          <label>EMAIL</label>
          <input className="input" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="group">
          <label>PASSWORD</label>
          <input className="input" type="password" value={password} onChange={(e) => setPassword(e.target.value)} />
        </div>
        <button className="book" disabled={busy} onClick={submit}>
          {busy ? "Please wait…" : mode === "in" ? "Sign in" : "Create account"}
        </button>
        <button className="continue" onClick={() => setMode(mode === "in" ? "up" : "in")}>
          {mode === "in" ? "Need an account?" : "Already have an account?"}
        </button>
      </div>
    </Screen>
  );
}
