import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { IMAGES } from "../constants/images";
import { useAuth } from "../context/AuthContext";

export function ProfileScreen() {
  const nav = useNavigate();
  const { user, logout } = useAuth();
  const rows = [
    { label: "My Orders", to: "/orders" },
    { label: "My Reservations", to: "/reservations" },
    { label: "Favourites", to: "/favourites" },
    { label: "Addresses", to: "/profile" },
    { label: "Help & Support", to: "/contact" },
    { label: "Settings", to: "/profile" },
    { label: "Offers & More", to: "/offers" },
    { label: "Gallery", to: "/gallery" },
    { label: "About Us", to: "/about" },
  ];
  return (
    <Screen nav className="profile">
      <div className="pad">
        <Header title="Profile" right={<span>⋮</span>} onBack={() => nav("/home")} />
        <div className="profileuser">
          <img src={user?.avatarUrl || IMAGES.avatar} alt="" />
          <h2>{user?.fullName || "Customer Name"}</h2>
          <p>{user?.email || "customer@email.com"}</p>
        </div>
        <div className="profilemenu">
          {rows.map((r) => (
            <button key={r.label} onClick={() => nav(r.to)}>
              {r.label} <span>›</span>
            </button>
          ))}
          {user ? (
            <button onClick={() => { logout(); nav("/home"); }}>
              Sign out <span>›</span>
            </button>
          ) : (
            <button onClick={() => nav("/login")}>
              Sign in <span>›</span>
            </button>
          )}
        </div>
      </div>
    </Screen>
  );
}
