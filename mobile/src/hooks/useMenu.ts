import { useEffect, useState } from "react";
import { menuService } from "../services";
import type { MenuItem } from "../types";

export function useMenu() {
  const [items, setItems] = useState<MenuItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  useEffect(() => {
    menuService
      .getAll()
      .then(setItems)
      .catch((e: Error) => setError(e.message))
      .finally(() => setLoading(false));
  }, []);
  return { items, loading, error };
}
