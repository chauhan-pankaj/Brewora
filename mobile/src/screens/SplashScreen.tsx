import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { IMAGES } from "../constants/images";
import { STORAGE_KEYS } from "../constants/theme";
import { StatusBar } from "../components/StatusBar";

export function SplashScreen() {
  const nav = useNavigate();
  useEffect(() => {
    const t = window.setTimeout(() => {
      const done = localStorage.getItem(STORAGE_KEYS.onboarding);
      nav(done ? "/home" : "/onboarding", { replace: true });
    }, 1800);
    return () => window.clearTimeout(t);
  }, [nav]);

  return (
    <div className="screen">
      <StatusBar light />
      <div
        className="splash"
        style={{ ["--splash" as string]: `url('${IMAGES.splash}')`, height: "calc(100% - 30px)" }}
      >
        <div>
          <div className="mark">◈</div>
          <div className="brand">Brewora</div>
          <div className="tag">CAFÉ & KITCHEN</div>
        </div>
        <div className="script">
          Good
          <br />
          Brews
          <br />
          Brighter
          <br />
          People ♡
        </div>
        <div className="bottom-mark">SIP · SAVOR · BELONG</div>
      </div>
    </div>
  );
}
