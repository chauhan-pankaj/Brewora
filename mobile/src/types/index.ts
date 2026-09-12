export type Category = {
  categoryId: number;
  name: string;
  slug: string;
  icon: string;
};

export type MenuItem = {
  menuItemId: number;
  categoryId: number;
  categoryName?: string;
  name: string;
  description: string;
  price: number;
  imageUrl: string;
  isFeatured: boolean;
  isAvailable: boolean;
};

export type CartLine = {
  key: string;
  menuItemId: number;
  name: string;
  imageUrl: string;
  size: "Small" | "Medium" | "Large";
  quantity: number;
  unitPrice: number;
};

export type UserProfile = {
  userId: number;
  fullName: string;
  email: string;
  phone: string;
  role: "Customer" | "Admin";
  avatarUrl?: string;
  address?: string;
};

export type OrderStatus =
  | "Pending"
  | "Confirmed"
  | "Preparing"
  | "Ready"
  | "OutForDelivery"
  | "Delivered"
  | "Cancelled";

export type PaymentStatus = "Pending" | "Paid" | "Failed" | "Refunded";

export type OrderItem = {
  orderItemId: number;
  menuItemId: number;
  name?: string;
  imageUrl?: string;
  quantity: number;
  unitPrice: number;
  totalPrice: number;
  size?: string;
};

export type Order = {
  orderId: number;
  orderNumber: string;
  customerName: string;
  customerEmail: string;
  customerPhone: string;
  deliveryAddress: string;
  subtotal: number;
  deliveryCharges: number;
  discount: number;
  tax: number;
  totalAmount: number;
  orderStatus: OrderStatus;
  paymentStatus: PaymentStatus;
  razorpayOrderId?: string;
  createdDate: string;
  items?: OrderItem[];
  estimatedMinutes?: number;
};

export type Reservation = {
  reservationId: number;
  customerName: string;
  email: string;
  phone: string;
  reservationDate: string;
  reservationTime: string;
  guestCount: number;
  specialRequest?: string;
  status: string;
};

export type SlotAvailability = {
  time: string;
  available: boolean;
  remaining: number;
};

export type GalleryItem = {
  galleryId: number;
  title: string;
  imageUrl: string;
  caption?: string;
};

export type CafeEvent = {
  eventId: number;
  title: string;
  description: string;
  eventDate: string;
  imageUrl: string;
};

export type Offer = {
  title: string;
  subtitle: string;
  imageUrl: string;
  tone: "green" | "brown";
};

export type ApiSuccess<T> = {
  success: true;
  message: string;
  data: T;
};

export type ApiFailure = {
  success: false;
  message: string;
  errors?: string[];
};

export type ApiResult<T> = ApiSuccess<T> | ApiFailure;
