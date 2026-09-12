export async function openRazorpay(options: {
  key: string;
  amount: number;
  currency: string;
  name: string;
  description: string;
  order_id: string;
  prefill?: { name?: string; email?: string; contact?: string };
}): Promise<{ razorpay_payment_id: string; razorpay_order_id: string; razorpay_signature: string }> {
  await loadScript();
  const Razorpay = (window as unknown as { Razorpay: new (o: unknown) => { open: () => void } }).Razorpay;
  return new Promise((resolve, reject) => {
    const rzp = new Razorpay({
      ...options,
      theme: { color: "#173620" },
      handler: resolve,
      modal: { ondismiss: () => reject(new Error("Payment cancelled")) },
    });
    rzp.open();
  });
}

function loadScript() {
  if (document.querySelector('script[src*="checkout.razorpay.com"]')) {
    return Promise.resolve();
  }
  return new Promise<void>((resolve, reject) => {
    const s = document.createElement("script");
    s.src = "https://checkout.razorpay.com/v1/checkout.js";
    s.onload = () => resolve();
    s.onerror = () => reject(new Error("Unable to load Razorpay"));
    document.body.appendChild(s);
  });
}
