using System.Security.Claims;
using Brewora.Application.Common;
using Brewora.Application.DTOs;
using Brewora.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Brewora.Application.Services;

public class MenuService
{
    private readonly IMenuRepository _repo;
    public MenuService(IMenuRepository repo) => _repo = repo;
    public Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken ct) => _repo.GetAllAsync(ct);
    public async Task<MenuItemDto> GetByIdAsync(int id, CancellationToken ct) =>
        await _repo.GetByIdAsync(id, ct) ?? throw new AppException("Menu item not found", 404);
    public Task<IEnumerable<MenuItemDto>> GetByCategoryAsync(int id, CancellationToken ct) => _repo.GetByCategoryAsync(id, ct);
    public Task<IEnumerable<MenuItemDto>> GetFeaturedAsync(CancellationToken ct) => _repo.GetFeaturedAsync(ct);
    public Task<int> CreateAsync(CreateMenuItemDto dto, CancellationToken ct) => _repo.CreateAsync(dto, ct);
    public Task UpdateAsync(int id, CreateMenuItemDto dto, CancellationToken ct) => _repo.UpdateAsync(id, dto, ct);
}

public class CategoryService
{
    private readonly ICategoryRepository _repo;
    public CategoryService(ICategoryRepository repo) => _repo = repo;
    public Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken ct) => _repo.GetAllAsync(ct);
}

public class OrderService
{
    private readonly IMenuRepository _menu;
    private readonly IOrderRepository _orders;
    private readonly IOrderItemRepository _items;
    private readonly ILogger<OrderService> _log;

    public OrderService(IMenuRepository menu, IOrderRepository orders, IOrderItemRepository items, ILogger<OrderService> log)
    {
        _menu = menu; _orders = orders; _items = items; _log = log;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto request, int? userId, CancellationToken ct)
    {
        if (request.Items is null || request.Items.Count == 0)
            throw new AppException("Cart is empty");

        decimal subtotal = 0;
        var lines = new List<(OrderItemRequest req, MenuItemDto item, decimal unit)>();
        foreach (var line in request.Items)
        {
            if (line.Quantity <= 0) throw new AppException("Invalid quantity");
            var item = await _menu.GetByIdAsync(line.MenuItemId, ct)
                ?? throw new AppException($"Menu item {line.MenuItemId} not found", 404);
            if (!item.IsAvailable) throw new AppException($"{item.Name} is unavailable");
            var unit = item.Price + SizeDelta(line.Size);
            subtotal += unit * line.Quantity;
            lines.Add((line, item, unit));
        }

        var delivery = 40m;
        var prior = userId.HasValue
            ? (await _orders.GetByUserAsync(userId.Value, ct)).Any(o => o.PaymentStatus == "Paid")
            : (await _orders.GetByEmailAsync(request.CustomerEmail, ct)).Any(o => o.PaymentStatus == "Paid");
        var discount = prior ? 0m : Math.Round(subtotal * 0.20m, 2);
        var tax = 0m;
        var total = subtotal + delivery - discount + tax;

        var number = $"BR{DateTime.UtcNow:yyMMddHHmmss}";
        var order = new OrderDto(0, number, request.CustomerName, request.CustomerEmail, request.CustomerPhone,
            request.DeliveryAddress, subtotal, delivery, discount, tax, total, "Pending", "Pending", null, DateTime.UtcNow, null);
        var id = await _orders.CreateAsync(order, userId, ct);
        foreach (var (req, _, unit) in lines)
            await _items.CreateAsync(id, req.MenuItemId, req.Quantity, unit, unit * req.Quantity, req.Size, ct);

        _log.LogInformation("Created order {OrderNumber} total {Total}", number, total);
        return (await _orders.GetByIdAsync(id, ct))!;
    }

    public async Task<OrderDto> GetByIdAsync(int id, CancellationToken ct) =>
        await _orders.GetByIdAsync(id, ct) ?? throw new AppException("Order not found", 404);

    public Task<IEnumerable<OrderDto>> MineAsync(int userId, CancellationToken ct) => _orders.GetByUserAsync(userId, ct);
    public Task<IEnumerable<OrderDto>> AllAsync(CancellationToken ct) => _orders.GetAllAsync(ct);
    public Task UpdateStatusAsync(int id, string status, CancellationToken ct) => _orders.UpdateStatusAsync(id, status, ct);

    private static decimal SizeDelta(string? size) => size switch
    {
        "Medium" => 20m,
        "Large" => 40m,
        _ => 0m
    };
}

public class PaymentService
{
    private readonly IOrderRepository _orders;
    private readonly IPaymentRepository _payments;
    private readonly IRazorpayGateway _razorpay;
    private readonly RazorpayOptions _options;
    private readonly ILogger<PaymentService> _log;

    public PaymentService(IOrderRepository orders, IPaymentRepository payments, IRazorpayGateway razorpay, Microsoft.Extensions.Options.IOptions<RazorpayOptions> options, ILogger<PaymentService> log)
    {
        _orders = orders; _payments = payments; _razorpay = razorpay; _options = options.Value; _log = log;
    }

    public async Task<RazorpayOrderDto> CreateRazorpayOrderAsync(int breworaOrderId, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(breworaOrderId, ct) ?? throw new AppException("Order not found", 404);
        var (rzpId, paise) = await _razorpay.CreateOrderAsync(order.TotalAmount, order.OrderNumber, ct);
        await _orders.UpdatePaymentAsync(order.OrderId, "Pending", "Pending", rzpId, ct);
        await _payments.CreateAsync(new PaymentWrite(0, order.OrderId, rzpId, null, null, order.TotalAmount, "INR", "Created", "Razorpay"), ct);
        return new RazorpayOrderDto(rzpId, paise, "INR", _options.KeyId, order.OrderId, order.OrderNumber);
    }

    public async Task<(bool Paid, string OrderNumber)> VerifyAsync(VerifyPaymentDto dto, CancellationToken ct)
    {
        var order = await _orders.GetByIdAsync(dto.BreworaOrderId, ct) ?? throw new AppException("Order not found", 404);
        if (!RazorpaySignature.IsValid(_options.KeySecret, dto.RazorpayOrderId, dto.RazorpayPaymentId, dto.RazorpaySignature))
            throw new AppException("Payment signature is invalid", 400);

        await _payments.CreateAsync(new PaymentWrite(0, order.OrderId, dto.RazorpayOrderId, dto.RazorpayPaymentId, dto.RazorpaySignature, order.TotalAmount, "INR", "Paid", "Razorpay"), ct);
        await _orders.UpdatePaymentAsync(order.OrderId, "Paid", "Confirmed", dto.RazorpayOrderId, ct);
        _log.LogInformation("Verified payment for {Order}", order.OrderNumber);
        return (true, order.OrderNumber);
    }
}

public class RazorpayOptions
{
    public string KeyId { get; set; } = string.Empty;
    public string KeySecret { get; set; } = string.Empty;
}

public static class RazorpaySignature
{
    public static bool IsValid(string secret, string orderId, string paymentId, string signature)
    {
        var payload = $"{orderId}|{paymentId}";
        using var hmac = new System.Security.Cryptography.HMACSHA256(System.Text.Encoding.UTF8.GetBytes(secret));
        var hash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payload));
        var computed = Convert.ToHexString(hash).ToLowerInvariant();
        return string.Equals(computed, signature, StringComparison.OrdinalIgnoreCase);
    }
}

public class ReservationService
{
    private readonly IReservationRepository _repo;
    public ReservationService(IReservationRepository repo) => _repo = repo;

    public async Task<ReservationDto> CreateAsync(CreateReservationDto request, int? userId, CancellationToken ct)
    {
        if (request.GuestCount <= 0) throw new AppException("Guest count is required");
        var slots = await _repo.GetAvailabilityAsync(request.ReservationDate, ct);
        var slot = slots.FirstOrDefault(s => s.Time == request.ReservationTime)
            ?? throw new AppException("That time is not offered");
        if (!slot.Available || slot.Remaining < request.GuestCount)
            throw new AppException("This slot is unavailable");
        var id = await _repo.CreateAsync(request, userId, ct);
        return await _repo.GetByIdAsync(id, ct) ?? throw new AppException("Reservation failed");
    }

    public async Task<IEnumerable<SlotDto>> AvailabilityAsync(DateTime date, int guests, CancellationToken ct)
    {
        var slots = await _repo.GetAvailabilityAsync(date, ct);
        return slots.Select(s => s with { Available = s.Remaining >= Math.Max(1, guests) });
    }

    public Task<IEnumerable<ReservationDto>> MineAsync(int userId, CancellationToken ct) => _repo.GetByUserAsync(userId, ct);
    public Task<IEnumerable<ReservationDto>> AllAsync(CancellationToken ct) => _repo.GetAllAsync(ct);
}

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IJwtTokenService _jwt;
    public AuthService(IUserRepository users, IJwtTokenService jwt) { _users = users; _jwt = jwt; }

    public async Task<AuthResultDto> RegisterAsync(RegisterDto dto, CancellationToken ct)
    {
        if (await _users.GetByEmailAsync(dto.Email, ct) is not null)
            throw new AppException("An account already uses this email");
        var hash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
        var id = await _users.CreateAsync(2, dto.FullName, dto.Email, dto.Phone, hash, null, ct);
        var user = await _users.GetByIdAsync(id, ct) ?? throw new AppException("Unable to create account");
        return new AuthResultDto(_jwt.CreateToken(user), Map(user));
    }

    public async Task<AuthResultDto> LoginAsync(LoginDto dto, CancellationToken ct)
    {
        var user = await _users.GetByEmailAsync(dto.Email, ct) ?? throw new AppException("Invalid credentials", 401);
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new AppException("Invalid credentials", 401);
        return new AuthResultDto(_jwt.CreateToken(user), Map(user));
    }

    public async Task<UserProfileDto> ProfileAsync(int userId, CancellationToken ct) =>
        Map(await _users.GetByIdAsync(userId, ct) ?? throw new AppException("User not found", 404));

    public async Task<UserProfileDto> UpdateAsync(int userId, UpdateProfileDto dto, CancellationToken ct)
    {
        await _users.UpdateProfileAsync(userId, dto, ct);
        return await ProfileAsync(userId, ct);
    }

    private static UserProfileDto Map(UserRecord u) =>
        new(u.UserId, u.FullName, u.Email, u.Phone ?? "", u.RoleName, u.AvatarUrl, u.Address);
}

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        var id = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? user.FindFirst("sub")?.Value;
        return int.TryParse(id, out var n) ? n : 0;
    }
}
