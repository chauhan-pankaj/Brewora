import { useLocation, useNavigate } from "react-router-dom";
import { StatusBar } from "../components/StatusBar";
import type { Reservation } from "../types";

export function ReservationConfirmationScreen() {
  const nav = useNavigate();
  const reservation = (useLocation().state as { reservation?: Reservation } | null)?.reservation;
  return (
    <div className="screen">
      <StatusBar />
      <div className="success">
        <div className="check">✓</div>
        <h2>
          Table Reserved
          <br />
          for Special Moments
        </h2>
        <p>We saved a quiet corner for you at Brewora.</p>
        <div className="orderbox">
          <strong>{reservation?.customerName || "Guest"}</strong>
          {reservation?.reservationDate} · {reservation?.reservationTime}
          <br />
          {reservation?.guestCount} guests
        </div>
        <button className="track" style={{ marginTop: 16 }} onClick={() => nav("/home")}>
          Back Home
        </button>
        <button className="continue" onClick={() => nav("/reservations")}>
          Book Another
        </button>
      </div>
    </div>
  );
}
