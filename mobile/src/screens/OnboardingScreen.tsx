import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { IMAGES } from "../constants/images";
import { STORAGE_KEYS } from "../constants/theme";
import { StatusBar } from "../components/StatusBar";

const SLIDES = [
  {
    img: IMAGES.onboarding,
    title: "A Cozy Place\nfor Great Moments",
    copy: "Delicious coffee, handcrafted food,\nand a warm ambience — all in one place.",
  },
  {
    img: IMAGES.onboardingFood,
    title: "Good Food.\nBrighter People.",
    copy: "From slow-poured coffee to kitchen classics,\nevery plate is made to share.",
  },
  {
    img: IMAGES.onboardingReserve,
    title: "Reserve Your Table\nfor Special Moments",
    copy: "Walk in for a quiet brew, or book a table\nwhen you want the evening to linger.",
  },
];

export function OnboardingScreen() {
  const [i, setI] = useState(0);
  const nav = useNavigate();
  const finish = () => {
    localStorage.setItem(STORAGE_KEYS.onboarding, "1");
    nav("/home", { replace: true });
  };
  const slide = SLIDES[i];
  return (
    <div className="screen">
      <StatusBar />
      <div className="onboard">
        <div className="top">
          <button onClick={() => (i === 0 ? finish() : setI(i - 1))}>‹</button>
          <button onClick={finish}>Skip</button>
        </div>
        <div className="onimg" style={{ backgroundImage: `url(${slide.img})` }} />
        <h2 style={{ whiteSpace: "pre-line" }}>{slide.title}</h2>
        <p style={{ whiteSpace: "pre-line" }}>{slide.copy}</p>
        <div className="dots">
          {SLIDES.map((_, idx) => (
            <i key={idx} className={idx === i ? "sel" : ""} />
          ))}
        </div>
        <button
          className="arrow"
          onClick={() => (i === SLIDES.length - 1 ? finish() : setI(i + 1))}
        >
          →
        </button>
      </div>
    </div>
  );
}
