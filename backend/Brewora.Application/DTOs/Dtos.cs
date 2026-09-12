namespace Brewora.Application.DTOs;

public record CategoryDto(int CategoryId, string Name, string Slug, string? Icon);

public record MenuItemDto(
    int MenuItemId,
    int CategoryId,
    string? CategoryName,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    bool IsFeatured,
    bool IsAvailable);

public record CreateMenuItemDto(
    int CategoryId,
    string Name,
    string Description,
    decimal Price,
    string ImageUrl,
    bool IsFeatured,
    bool IsAvailable);

public record OrderItemRequest(int MenuItemId, int Quantity, string Size);

public record CreateOrderDto(
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string DeliveryAddress,
    string PaymentMethod,
    List<OrderItemRequest> Items);

public record OrderItemDto(
    int OrderItemId,
    int MenuItemId,
    string? Name,
    string? ImageUrl,
    int Quantity,
    decimal UnitPrice,
    decimal TotalPrice,
    string? Size);

public record OrderDto(
    int OrderId,
    string OrderNumber,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string DeliveryAddress,
    decimal Subtotal,
    decimal DeliveryCharges,
    decimal Discount,
    decimal Tax,
    decimal TotalAmount,
    string OrderStatus,
    string PaymentStatus,
    string? RazorpayOrderId,
    DateTime CreatedDate,
    List<OrderItemDto>? Items);

public record CreatePaymentOrderDto(int BreworaOrderId);

public record RazorpayOrderDto(
    string RazorpayOrderId,
    int Amount,
    string Currency,
    string KeyId,
    int BreworaOrderId,
    string OrderNumber);

public record VerifyPaymentDto(
    int BreworaOrderId,
    string RazorpayOrderId,
    string RazorpayPaymentId,
    string RazorpaySignature);

public record CreateReservationDto(
    string CustomerName,
    string Email,
    string Phone,
    DateTime ReservationDate,
    string ReservationTime,
    int GuestCount,
    string? SpecialRequest);

public record ReservationDto(
    int ReservationId,
    string CustomerName,
    string Email,
    string Phone,
    DateTime ReservationDate,
    string ReservationTime,
    int GuestCount,
    string? SpecialRequest,
    string Status);

public record SlotDto(string Time, bool Available, int Remaining);

public record RegisterDto(string FullName, string Email, string Phone, string Password);
public record LoginDto(string Email, string Password);
public record AuthResultDto(string Token, UserProfileDto User);
public record UserProfileDto(int UserId, string FullName, string Email, string Phone, string Role, string? AvatarUrl, string? Address);
public record UpdateProfileDto(string FullName, string Phone, string? Address, string? AvatarUrl);
public record FavouriteRequest(int MenuItemId);
public record GalleryDto(int GalleryId, string Title, string ImageUrl, string? Caption);
public record EventDto(int EventId, string Title, string Description, DateTime EventDate, string ImageUrl);
public record ContactDto(string Name, string Email, string Phone, string Message);
public record UpdateOrderStatusDto(string OrderStatus);
