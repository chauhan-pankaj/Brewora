import { Navigate, Route, Routes, useNavigate } from "react-router-dom";
import { useEffect } from "react";
import { App as CapApp } from "@capacitor/app";
import { SplashScreen } from "../screens/SplashScreen";
import { OnboardingScreen } from "../screens/OnboardingScreen";
import { HomeScreen } from "../screens/HomeScreen";
import { MenuScreen } from "../screens/MenuScreen";
import { ProductDetailsScreen } from "../screens/ProductDetailsScreen";
import { CartScreen } from "../screens/CartScreen";
import { CheckoutScreen } from "../screens/CheckoutScreen";
import { PaymentScreen } from "../screens/PaymentScreen";
import { OrderSuccessScreen } from "../screens/OrderSuccessScreen";
import { OrderTrackingScreen } from "../screens/OrderTrackingScreen";
import { ReservationsScreen } from "../screens/ReservationsScreen";
import { ReservationConfirmationScreen } from "../screens/ReservationConfirmationScreen";
import { ProfileScreen } from "../screens/ProfileScreen";
import { MyOrdersScreen } from "../screens/MyOrdersScreen";
import { OrderDetailsScreen } from "../screens/OrderDetailsScreen";
import { FavouritesScreen } from "../screens/FavouritesScreen";
import { OffersScreen } from "../screens/OffersScreen";
import { GalleryScreen } from "../screens/GalleryScreen";
import { AboutUsScreen } from "../screens/AboutUsScreen";
import { ContactUsScreen } from "../screens/ContactUsScreen";
import { LoginScreen } from "../screens/LoginScreen";
import { DrawerScreen } from "../screens/DrawerScreen";

export function AppRouter() {
  const nav = useNavigate();
  useEffect(() => {
    const sub = CapApp.addListener("backButton", ({ canGoBack }) => {
      if (canGoBack) nav(-1);
      else CapApp.exitApp();
    });
    return () => {
      sub.then((s) => s.remove());
    };
  }, [nav]);

  return (
    <Routes>
      <Route path="/" element={<SplashScreen />} />
      <Route path="/onboarding" element={<OnboardingScreen />} />
      <Route path="/home" element={<HomeScreen />} />
      <Route path="/menu" element={<MenuScreen />} />
      <Route path="/menu/:id" element={<ProductDetailsScreen />} />
      <Route path="/cart" element={<CartScreen />} />
      <Route path="/checkout" element={<CheckoutScreen />} />
      <Route path="/payment" element={<PaymentScreen />} />
      <Route path="/order-success" element={<OrderSuccessScreen />} />
      <Route path="/orders" element={<MyOrdersScreen />} />
      <Route path="/orders/:id" element={<OrderDetailsScreen />} />
      <Route path="/orders/:id/track" element={<OrderTrackingScreen />} />
      <Route path="/reservations" element={<ReservationsScreen />} />
      <Route path="/reservations/confirm" element={<ReservationConfirmationScreen />} />
      <Route path="/profile" element={<ProfileScreen />} />
      <Route path="/favourites" element={<FavouritesScreen />} />
      <Route path="/offers" element={<OffersScreen />} />
      <Route path="/gallery" element={<GalleryScreen />} />
      <Route path="/about" element={<AboutUsScreen />} />
      <Route path="/contact" element={<ContactUsScreen />} />
      <Route path="/login" element={<LoginScreen />} />
      <Route path="/drawer" element={<DrawerScreen />} />
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}
