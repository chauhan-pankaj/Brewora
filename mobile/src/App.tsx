import { BrowserRouter } from "react-router-dom";
import { AuthProvider } from "./context/AuthContext";
import { CartProvider } from "./context/CartContext";
import { ToastProvider } from "./context/ToastContext";
import { AppRouter } from "./navigation/AppRouter";

export default function App() {
  return (
    <div className="app-root">
      <div className="device">
        <BrowserRouter>
          <AuthProvider>
            <CartProvider>
              <ToastProvider>
                <AppRouter />
              </ToastProvider>
            </CartProvider>
          </AuthProvider>
        </BrowserRouter>
      </div>
    </div>
  );
}
