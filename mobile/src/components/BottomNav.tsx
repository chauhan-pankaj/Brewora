import { useLocation, useNavigate } from "react-router-dom";

const TABS = [
  { to: "/home", label: "Home", icon: "⌂" },
  { to: "/menu", label: "Menu", icon: "♨" },
  { to: "/orders", label: "Orders", icon: "▣" },
  { to: "/favourites", label: "Favourites", icon: "♡" },
  { to: "/profile", label: "Profile", icon: "♙" },
];

export function BottomNav() {
  const loc = useLocation();
  const nav = useNavigate();
  return (
    <nav className="bottomnav">
      {TABS.map((t) => (
        <button
          key={t.to}
          className={loc.pathname.startsWith(t.to) ? "active" : ""}
          onClick={() => nav(t.to)}
        >
          <i>{t.icon}</i>
          {t.label}
        </button>
      ))}
    </nav>
  );
}
