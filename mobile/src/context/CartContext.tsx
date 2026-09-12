import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from "react";
import { STORAGE_KEYS } from "../constants/theme";
import type { CartLine, MenuItem } from "../types";
import { lineKey, sizeDelta } from "../utils/format";

const DELIVERY = 40;

type CartValue = {
  items: CartLine[];
  add: (item: MenuItem, size: CartLine["size"], qty?: number) => void;
  remove: (key: string) => void;
  setQty: (key: string, qty: number) => void;
  clear: () => void;
  subtotal: number;
  deliveryCharges: number;
  discount: number;
  tax: number;
  total: number;
  count: number;
};

const Ctx = createContext<CartValue | null>(null);

function load(): CartLine[] {
  try {
    const raw = localStorage.getItem(STORAGE_KEYS.cart);
    return raw ? (JSON.parse(raw) as CartLine[]) : [];
  } catch {
    return [];
  }
}

export function CartProvider({ children }: { children: ReactNode }) {
  const [items, setItems] = useState<CartLine[]>(load);

  const persist = (next: CartLine[]) => {
    setItems(next);
    localStorage.setItem(STORAGE_KEYS.cart, JSON.stringify(next));
  };

  const add = useCallback((item: MenuItem, size: CartLine["size"], qty = 1) => {
    persist(
      (() => {
        const current = load();
        const key = lineKey(item.menuItemId, size);
        const unitPrice = item.price + sizeDelta(size);
        const found = current.find((l) => l.key === key);
        if (found) {
          return current.map((l) =>
            l.key === key ? { ...l, quantity: l.quantity + qty } : l,
          );
        }
        return [
          ...current,
          {
            key,
            menuItemId: item.menuItemId,
            name: item.name,
            imageUrl: item.imageUrl,
            size,
            quantity: qty,
            unitPrice,
          },
        ];
      })(),
    );
  }, []);

  const remove = useCallback((key: string) => {
    persist(load().filter((l) => l.key !== key));
  }, []);

  const setQty = useCallback((key: string, qty: number) => {
    if (qty <= 0) persist(load().filter((l) => l.key !== key));
    else persist(load().map((l) => (l.key === key ? { ...l, quantity: qty } : l)));
  }, []);

  const clear = useCallback(() => persist([]), []);

  const subtotal = items.reduce((s, l) => s + l.unitPrice * l.quantity, 0);
  const deliveryCharges = items.length ? DELIVERY : 0;
  const discount = 0;
  const tax = 0;
  const total = subtotal + deliveryCharges - discount + tax;
  const count = items.reduce((s, l) => s + l.quantity, 0);

  const value = useMemo(
    () => ({
      items,
      add,
      remove,
      setQty,
      clear,
      subtotal,
      deliveryCharges,
      discount,
      tax,
      total,
      count,
    }),
    [items, add, remove, setQty, clear, subtotal, deliveryCharges, discount, tax, total, count],
  );

  return <Ctx.Provider value={value}>{children}</Ctx.Provider>;
}

export function useCart() {
  const ctx = useContext(Ctx);
  if (!ctx) throw new Error("CartProvider missing");
  return ctx;
}
