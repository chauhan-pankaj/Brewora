import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { useToast } from "../context/ToastContext";
import { useAuth } from "../context/AuthContext";
import { reservationService } from "../services";
import type { SlotAvailability } from "../types";

function todayIso() {
  return new Date().toISOString().slice(0, 10);
}

export function ReservationsScreen() {
  const nav = useNavigate();
  const { user } = useAuth();
  const { push } = useToast();
  const [date, setDate] = useState(todayIso());
  const [time, setTime] = useState("");
  const [guests, setGuests] = useState(2);
  const [name, setName] = useState(user?.fullName || "");
  const [phone, setPhone] = useState(user?.phone || "");
  const [email, setEmail] = useState(user?.email || "");
  const [note, setNote] = useState("");
  const [slots, setSlots] = useState<SlotAvailability[]>([]);
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    reservationService
      .availability(date, guests)
      .then(setSlots)
      .catch(() => setSlots([]));
  }, [date, guests]);

  async function book() {
    if (!name || !phone || !email || !date || !time) {
      push("Please complete the reservation details.");
      return;
    }
    setBusy(true);
    try {
      const res = await reservationService.create({
        customerName: name,
        email,
        phone,
        reservationDate: date,
        reservationTime: time,
        guestCount: guests,
        specialRequest: note,
      });
      nav("/reservations/confirm", { state: { reservation: res } });
    } catch (e) {
      push((e as Error).message);
    } finally {
      setBusy(false);
    }
  }

  return (
    <Screen>
      <div className="pad">
        <Header title="Reserve a Table" />
        <div className="group">
          <label>DATE</label>
          <input className="input" type="date" value={date} onChange={(e) => setDate(e.target.value)} />
        </div>
        <div className="group">
          <label>TIME</label>
          <div className="slots">
            {slots.length
              ? slots.map((s) => (
                  <button
                    key={s.time}
                    className={`${time === s.time ? "on" : ""} ${s.available ? "" : "off"}`}
                    onClick={() => setTime(s.time)}
                  >
                    {s.time}
                  </button>
                ))
              : ["11:00", "13:00", "17:00", "19:00", "20:30", "21:30"].map((t) => (
                  <button key={t} className={time === t ? "on" : ""} onClick={() => setTime(t)}>
                    {t}
                  </button>
                ))}
          </div>
        </div>
        <div className="group">
          <label>NO. OF GUESTS</label>
          <select className="input" value={guests} onChange={(e) => setGuests(Number(e.target.value))}>
            {[1, 2, 3, 4, 5, 6, 8].map((n) => (
              <option key={n} value={n}>
                {n} People
              </option>
            ))}
          </select>
        </div>
        <div className="group">
          <label>NAME</label>
          <input className="input" placeholder="Your Name" value={name} onChange={(e) => setName(e.target.value)} />
        </div>
        <div className="group">
          <label>MOBILE NUMBER</label>
          <input className="input" placeholder="+91 XXXXX XXXXX" value={phone} onChange={(e) => setPhone(e.target.value)} />
        </div>
        <div className="group">
          <label>EMAIL</label>
          <input className="input" value={email} onChange={(e) => setEmail(e.target.value)} />
        </div>
        <div className="group">
          <label>SPECIAL REQUEST</label>
          <textarea className="input" rows={3} value={note} onChange={(e) => setNote(e.target.value)} />
        </div>
        <button className="book" disabled={busy} onClick={book}>
          {busy ? "Booking…" : "Book a Table"}
        </button>
      </div>
    </Screen>
  );
}
