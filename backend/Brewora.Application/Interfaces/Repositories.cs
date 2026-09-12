using Brewora.Application.DTOs;

namespace Brewora.Application.Interfaces;

public interface IMenuRepository
{
    Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<MenuItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<MenuItemDto>> GetByCategoryAsync(int categoryId, CancellationToken cancellationToken);
    Task<IEnumerable<MenuItemDto>> GetFeaturedAsync(CancellationToken cancellationToken);
    Task<int> CreateAsync(CreateMenuItemDto request, CancellationToken cancellationToken);
    Task UpdateAsync(int id, CreateMenuItemDto request, CancellationToken cancellationToken);
}

public interface ICategoryRepository
{
    Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CreateAsync(string name, string slug, string icon, CancellationToken cancellationToken);
}

public interface IOrderRepository
{
    Task<int> CreateAsync(OrderDto order, int? userId, CancellationToken cancellationToken);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<OrderDto>> GetByUserAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<OrderDto>> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdatePaymentAsync(int orderId, string paymentStatus, string orderStatus, string? razorpayOrderId, CancellationToken cancellationToken);
    Task UpdateStatusAsync(int orderId, string orderStatus, CancellationToken cancellationToken);
}

public interface IOrderItemRepository
{
    Task CreateAsync(int orderId, int menuItemId, int quantity, decimal unitPrice, decimal totalPrice, string size, CancellationToken cancellationToken);
}

public interface IPaymentRepository
{
    Task<int> CreateAsync(PaymentWrite write, CancellationToken cancellationToken);
    Task<IEnumerable<PaymentWrite>> GetByOrderIdAsync(int orderId, CancellationToken cancellationToken);
    Task UpdateStatusAsync(int paymentId, string status, string? paymentIdRzp, string? signature, CancellationToken cancellationToken);
}

public record PaymentWrite(
    int PaymentId,
    int OrderId,
    string? RazorpayOrderId,
    string? RazorpayPaymentId,
    string? RazorpaySignature,
    decimal Amount,
    string Currency,
    string PaymentStatus,
    string? PaymentMethod);

public interface IReservationRepository
{
    Task<int> CreateAsync(CreateReservationDto request, int? userId, CancellationToken cancellationToken);
    Task<IEnumerable<SlotDto>> GetAvailabilityAsync(DateTime date, CancellationToken cancellationToken);
    Task<IEnumerable<ReservationDto>> GetByUserAsync(int userId, CancellationToken cancellationToken);
    Task<IEnumerable<ReservationDto>> GetAllAsync(CancellationToken cancellationToken);
    Task UpdateStatusAsync(int id, string status, CancellationToken cancellationToken);
    Task<ReservationDto?> GetByIdAsync(int id, CancellationToken cancellationToken);
}

public interface IUserRepository
{
    Task<UserRecord?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<UserRecord?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> CreateAsync(int roleId, string fullName, string email, string phone, string passwordHash, string? avatar, CancellationToken cancellationToken);
    Task UpdateProfileAsync(int userId, UpdateProfileDto request, CancellationToken cancellationToken);
}

public record UserRecord(int UserId, int RoleId, string RoleName, string FullName, string Email, string? Phone, string PasswordHash, string? AvatarUrl, string? Address);

public interface IFavouriteRepository
{
    Task AddAsync(int userId, int menuItemId, CancellationToken cancellationToken);
    Task RemoveAsync(int userId, int menuItemId, CancellationToken cancellationToken);
    Task<IEnumerable<MenuItemDto>> GetByUserAsync(int userId, CancellationToken cancellationToken);
}

public interface IGalleryRepository
{
    Task<IEnumerable<GalleryDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CreateAsync(string title, string imageUrl, string? caption, int sort, CancellationToken cancellationToken);
}

public interface IEventRepository
{
    Task<IEnumerable<EventDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<int> CreateAsync(string title, string description, DateTime eventDate, string imageUrl, CancellationToken cancellationToken);
}

public interface IContactRepository
{
    Task CreateAsync(ContactDto request, CancellationToken cancellationToken);
}

public interface IRazorpayGateway
{
    Task<(string OrderId, int AmountPaise)> CreateOrderAsync(decimal amountInr, string receipt, CancellationToken cancellationToken);
}

public interface IJwtTokenService
{
    string CreateToken(UserRecord user);
}
