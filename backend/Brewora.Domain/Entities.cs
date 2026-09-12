namespace Brewora.Domain.Entities;

public class User
{
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public string RoleName { get; set; } = "Customer";
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public string? Address { get; set; }
}

public class Category
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Icon { get; set; }
}

public class MenuItem
{
    public int MenuItemId { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
}

public class Order
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public int? UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerPhone { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DeliveryCharges { get; set; }
    public decimal Discount { get; set; }
    public decimal Tax { get; set; }
    public decimal TotalAmount { get; set; }
    public string OrderStatus { get; set; } = "Pending";
    public string PaymentStatus { get; set; } = "Pending";
    public string? RazorpayOrderId { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }
    public List<OrderItem> Items { get; set; } = new();
}

public class OrderItem
{
    public int OrderItemId { get; set; }
    public int OrderId { get; set; }
    public int MenuItemId { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Size { get; set; }
}

public class Payment
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string? RazorpayOrderId { get; set; }
    public string? RazorpayPaymentId { get; set; }
    public string? RazorpaySignature { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "INR";
    public string PaymentStatus { get; set; } = "Pending";
    public string? PaymentMethod { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public class Reservation
{
    public int ReservationId { get; set; }
    public int? UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime ReservationDate { get; set; }
    public string ReservationTime { get; set; } = string.Empty;
    public int GuestCount { get; set; }
    public string? SpecialRequest { get; set; }
    public string Status { get; set; } = "Confirmed";
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}

public class ReservationSlot
{
    public string Time { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public int Remaining { get; set; }
    public bool Available => Remaining > 0;
}

public class GalleryItem
{
    public int GalleryId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
}

public class CafeEvent
{
    public int EventId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime EventDate { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class ContactMessage
{
    public int ContactMessageId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string Message { get; set; } = string.Empty;
}
