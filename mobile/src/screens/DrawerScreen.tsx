import { useNavigate } from "react-router-dom";
import { IMAGES } from "../constants/images";
import { StatusBar } from "../components/StatusBar";

const LINKS = [
  { icon: "⌂", label: "Home", to: "/home" },
  { icon: "♨", label: "Menu", to: "/menu" },
  { icon: "▣", label: "Reservations", to: "/reservations" },
  { icon: "♡", label: "Offers", to: "/offers" },
  { icon: "▧", label: "Gallery", to: "/gallery" },
  { icon: "◎", label: "About Us", to: "/about" },
  { icon: "☎", label: "Contact Us", to: "/contact" },
];

export function DrawerScreen() {
  const nav = useNavigate();
  return (
    <div className="screen">
      <StatusBar light />
      <div
        className="drawer"
        style={{ ["--drawer-img" as string]: `url('${IMAGES.drawer}')`, position: "relative" }}
      >
        <button className="icon-btn" style={{ color: "#fff", marginLeft: "auto" }} onClick={() => nav(-1)}>
          ×
        </button>
        <div className="drawerbrand">
          <div className="mark">◈</div>
          <div>
            <div className="brand">Brewora</div>
            <div className="tag">CAFÉ & KITCHEN</div>
          </div>
        </div>
        <div className="links">
          {LINKS.map((l) => (
            <button key={l.to} onClick={() => nav(l.to)}>
              {l.icon} &nbsp; {l.label}
            </button>
          ))}
        </div>
        <div className="drawscript">
          Good Coffee
          <br />
          Brighter People ♡
        </div>
      </div>
    </div>
  );
}
