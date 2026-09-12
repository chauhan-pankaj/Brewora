export function inr(n: number) {
  return `₹${Math.round(n)}`;
}

export function greeting(date = new Date()) {
  const h = date.getHours();
  if (h < 12) return "Good Morning";
  if (h < 17) return "Good Afternoon";
  return "Good Evening";
}

export function lineKey(menuItemId: number, size: string) {
  return `${menuItemId}:${size}`;
}

export function sizeDelta(size: "Small" | "Medium" | "Large") {
  if (size === "Medium") return 20;
  if (size === "Large") return 40;
  return 0;
}
