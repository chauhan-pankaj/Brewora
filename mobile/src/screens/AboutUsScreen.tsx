import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { IMAGES } from "../constants/images";
import { eventService } from "../services";
import type { CafeEvent } from "../types";

export function AboutUsScreen() {
  const nav = useNavigate();
  const [events, setEvents] = useState<CafeEvent[]>([]);
  useEffect(() => {
    eventService.getAll().then(setEvents).catch(() => setEvents([]));
  }, []);
  return (
    <Screen>
      <div className="pad">
        <Header title="About Us" onBack={() => nav("/home")} />
        <div
          className="onimg"
          style={{ height: 180, backgroundImage: `url(${IMAGES.onboarding})` }}
        />
        <h2 style={{ marginTop: 16, fontSize: 28 }}>More than just coffee</h2>
        <p className="address" style={{ fontSize: 11, marginTop: 10 }}>
          Brewora Café & Kitchen is a warm corner in Indiranagar — slow coffee, kitchen plates, and
          a table for the people you like spending time with.
        </p>
        <div className="sect">
          <h3>Evenings ahead</h3>
        </div>
        {events.map((e) => (
          <div className="box" key={e.eventId}>
            <h3>{e.title}</h3>
            <p className="address">{e.description}</p>
          </div>
        ))}
      </div>
    </Screen>
  );
}
