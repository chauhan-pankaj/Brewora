import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { Screen } from "../components/Screen";
import { Header } from "../components/Header";
import { IMAGES } from "../constants/images";
import { galleryService } from "../services";
import type { GalleryItem } from "../types";

export function GalleryScreen() {
  const nav = useNavigate();
  const [items, setItems] = useState<GalleryItem[]>([]);
  useEffect(() => {
    galleryService
      .getAll()
      .then(setItems)
      .catch(() =>
        setItems([
          { galleryId: 1, title: "Interior", imageUrl: IMAGES.gallery.interior },
          { galleryId: 2, title: "Latte", imageUrl: IMAGES.gallery.latteArt },
          { galleryId: 3, title: "Beans", imageUrl: IMAGES.gallery.beans },
          { galleryId: 4, title: "Table", imageUrl: IMAGES.gallery.table },
        ]),
      );
  }, []);
  return (
    <Screen>
      <div className="pad">
        <Header title="Gallery" onBack={() => nav("/home")} />
        <p className="address" style={{ margin: "8px 0 14px" }}>
          Coffee, people, stories.
        </p>
        <div className="gallery-grid">
          {items.map((g) => (
            <img key={g.galleryId} src={g.imageUrl} alt={g.title} />
          ))}
        </div>
      </div>
    </Screen>
  );
}
