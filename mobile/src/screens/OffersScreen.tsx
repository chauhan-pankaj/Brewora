import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { IMAGES } from "../constants/images";

export function OffersScreen() {
  const nav = useNavigate();
  return (
    <Screen>
      <div className="pad">
        <Header title="Offers & More" right={<button className="icon-btn" onClick={() => nav(-1)}>×</button>} />
        <button
          className="offer green"
          style={{ ["--offer-1" as string]: `url('${IMAGES.cake}')` }}
          onClick={() => nav("/menu")}
        >
          <b>Get 20% OFF</b>
          <p>On First Order</p>
        </button>
        <button
          className="offer brown"
          style={{ ["--offer-2" as string]: `url('${IMAGES.brownie}')` }}
          onClick={() => nav("/menu")}
        >
          <b>
            Sweet Deals
            <br />
            Happier Meals
          </b>
          <p>Up to 30% OFF</p>
        </button>
      </div>
    </Screen>
  );
}
