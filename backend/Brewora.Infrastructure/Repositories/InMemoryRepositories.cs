using Brewora.Application.DTOs;
using Brewora.Application.Interfaces;
using Brewora.Infrastructure.Persistence;

namespace Brewora.Infrastructure.Repositories;

public class InMemoryMenuRepository : IMenuRepository
{
    private readonly InMemoryStore _db;
    public InMemoryMenuRepository(InMemoryStore db) => _db = db;
    public Task<IEnumerable<MenuItemDto>> GetAllAsync(CancellationToken _) => Task.FromResult(_db.Menu.AsEnumerable());
    public Task<MenuItemDto?> GetByIdAsync(int id, CancellationToken _) => Task.FromResult(_db.Menu.FirstOrDefault(m => m.MenuItemId == id));
    public Task<IEnumerable<MenuItemDto>> GetByCategoryAsync(int categoryId, CancellationToken _) =>
        Task.FromResult(_db.Menu.Where(m => m.CategoryId == categoryId).AsEnumerable());
    public Task<IEnumerable<MenuItemDto>> GetFeaturedAsync(CancellationToken _) =>
        Task.FromResult(_db.Menu.Where(m => m.IsFeatured).AsEnumerable());
    public Task<int> CreateAsync(CreateMenuItemDto request, CancellationToken _)
    {
        var cat = _db.Categories.FirstOrDefault(c => c.CategoryId == request.CategoryId);
        var id = _db.NextMenu++;
        _db.Menu.Add(new MenuItemDto(id, request.CategoryId, cat?.Name, request.Name, request.Description, request.Price, request.ImageUrl, request.IsFeatured, request.IsAvailable));
        return Task.FromResult(id);
    }
    public Task UpdateAsync(int id, CreateMenuItemDto request, CancellationToken _)
    {
        var i = _db.Menu.FindIndex(m => m.MenuItemId == id);
        if (i >= 0)
        {
            var cat = _db.Categories.FirstOrDefault(c => c.CategoryId == request.CategoryId);
            _db.Menu[i] = new MenuItemDto(id, request.CategoryId, cat?.Name, request.Name, request.Description, request.Price, request.ImageUrl, request.IsFeatured, request.IsAvailable);
        }
        return Task.CompletedTask;
    }
}

public class InMemoryCategoryRepository : ICategoryRepository
{
    private readonly InMemoryStore _db;
    public InMemoryCategoryRepository(InMemoryStore db) => _db = db;
    public Task<IEnumerable<CategoryDto>> GetAllAsync(CancellationToken _) => Task.FromResult(_db.Categories.AsEnumerable());
    public Task<int> CreateAsync(string name, string slug, string icon, CancellationToken _)
    {
        var id = _db.NextCat++;
        _db.Categories.Add(new CategoryDto(id, name, slug, icon));
        return Task.FromResult(id);
    }
}

public class InMemoryOrderRepository : IOrderRepository
{
    private readonly InMemoryStore _db;
    public InMemoryOrderRepository(InMemoryStore db) => _db = db;

    public Task<int> CreateAsync(OrderDto order, int? userId, CancellationToken _)
    {
        var id = _db.NextOrder++;
        _db.Orders.Add(order with { OrderId = id, Items = new List<OrderItemDto>() });
        if (userId.HasValue) UserMap[id] = userId.Value;
        return Task.FromResult(id);
    }

    public Task<OrderDto?> GetByIdAsync(int id, CancellationToken _)
    {
        var o = _db.Orders.FirstOrDefault(x => x.OrderId == id);
        if (o is null) return Task.FromResult<OrderDto?>(null);
        var items = _db.OrderItems.Where(i => _itemOrders.TryGetValue(i.OrderItemId, out var oid) && oid == id).ToList();
        return Task.FromResult<OrderDto?>(o with { Items = items });
    }

    internal static readonly Dictionary<int, int> _itemOrders = new();

    public Task<IEnumerable<OrderDto>> GetByUserAsync(int userId, CancellationToken _)
    {
        // UserId not on OrderDto - keep email-based in service; store map
        var ids = UserMap.Where(kv => kv.Value == userId).Select(kv => kv.Key).ToHashSet();
        return Task.FromResult(_db.Orders.Where(o => ids.Contains(o.OrderId)).AsEnumerable());
    }

    internal static readonly Dictionary<int, int> UserMap = new();

    public Task<IEnumerable<OrderDto>> GetByEmailAsync(string email, CancellationToken _) =>
        Task.FromResult(_db.Orders.Where(o => o.CustomerEmail.Equals(email, StringComparison.OrdinalIgnoreCase)).AsEnumerable());

    public Task<IEnumerable<OrderDto>> GetAllAsync(CancellationToken _) => Task.FromResult(_db.Orders.AsEnumerable());

    public Task UpdatePaymentAsync(int orderId, string paymentStatus, string orderStatus, string? razorpayOrderId, CancellationToken _)
    {
        var i = _db.Orders.FindIndex(o => o.OrderId == orderId);
        if (i >= 0)
        {
            var o = _db.Orders[i];
            _db.Orders[i] = o with { PaymentStatus = paymentStatus, OrderStatus = orderStatus, RazorpayOrderId = razorpayOrderId ?? o.RazorpayOrderId };
        }
        return Task.CompletedTask;
    }

    public Task UpdateStatusAsync(int orderId, string orderStatus, CancellationToken _)
    {
        var i = _db.Orders.FindIndex(o => o.OrderId == orderId);
        if (i >= 0)
        {
            var o = _db.Orders[i];
            _db.Orders[i] = o with { OrderStatus = orderStatus };
        }
        return Task.CompletedTask;
    }
}

public class InMemoryOrderItemRepository : IOrderItemRepository
{
    private readonly InMemoryStore _db;
    private readonly IMenuRepository _menu;
    public InMemoryOrderItemRepository(InMemoryStore db, IMenuRepository menu) { _db = db; _menu = menu; }

    public async Task CreateAsync(int orderId, int menuItemId, int quantity, decimal unitPrice, decimal totalPrice, string size, CancellationToken ct)
    {
        var id = _db.NextItem++;
        var item = await _menu.GetByIdAsync(menuItemId, ct);
        _db.OrderItems.Add(new OrderItemDto(id, menuItemId, item?.Name, item?.ImageUrl, quantity, unitPrice, totalPrice, size));
        InMemoryOrderRepository._itemOrders[id] = orderId;
    }
}

public class InMemoryPaymentRepository : IPaymentRepository
{
    private readonly InMemoryStore _db;
    public InMemoryPaymentRepository(InMemoryStore db) => _db = db;
    public Task<int> CreateAsync(PaymentWrite write, CancellationToken _)
    {
        var id = _db.NextPay++;
        _db.Payments.Add(write with { PaymentId = id });
        return Task.FromResult(id);
    }
    public Task<IEnumerable<PaymentWrite>> GetByOrderIdAsync(int orderId, CancellationToken _) =>
        Task.FromResult(_db.Payments.Where(p => p.OrderId == orderId).AsEnumerable());
    public Task UpdateStatusAsync(int paymentId, string status, string? paymentIdRzp, string? signature, CancellationToken _)
    {
        var i = _db.Payments.FindIndex(p => p.PaymentId == paymentId);
        if (i >= 0)
        {
            var p = _db.Payments[i];
            _db.Payments[i] = p with { PaymentStatus = status, RazorpayPaymentId = paymentIdRzp ?? p.RazorpayPaymentId, RazorpaySignature = signature ?? p.RazorpaySignature };
        }
        return Task.CompletedTask;
    }
}

public class InMemoryReservationRepository : IReservationRepository
{
    private readonly InMemoryStore _db;
    internal static readonly Dictionary<int, int?> UserMap = new();
    public InMemoryReservationRepository(InMemoryStore db) => _db = db;

    public Task<int> CreateAsync(CreateReservationDto request, int? userId, CancellationToken _)
    {
        var id = _db.NextRes++;
        _db.Reservations.Add(new ReservationDto(id, request.CustomerName, request.Email, request.Phone, request.ReservationDate, request.ReservationTime, request.GuestCount, request.SpecialRequest, "Confirmed"));
        UserMap[id] = userId;
        return Task.FromResult(id);
    }

    public Task<IEnumerable<SlotDto>> GetAvailabilityAsync(DateTime date, CancellationToken _)
    {
        var day = date.Date;
        var list = _db.Slots.Select(s =>
        {
            var used = _db.Reservations.Where(r => r.ReservationDate.Date == day && r.ReservationTime == s.Time && r.Status != "Cancelled").Sum(r => r.GuestCount);
            var remaining = Math.Max(0, s.Capacity - used);
            return new SlotDto(s.Time, remaining > 0, remaining);
        });
        return Task.FromResult(list);
    }

    public Task<IEnumerable<ReservationDto>> GetByUserAsync(int userId, CancellationToken _)
    {
        var ids = UserMap.Where(kv => kv.Value == userId).Select(kv => kv.Key).ToHashSet();
        return Task.FromResult(_db.Reservations.Where(r => ids.Contains(r.ReservationId)).AsEnumerable());
    }

    public Task<IEnumerable<ReservationDto>> GetAllAsync(CancellationToken _) => Task.FromResult(_db.Reservations.AsEnumerable());

    public Task UpdateStatusAsync(int id, string status, CancellationToken _)
    {
        var i = _db.Reservations.FindIndex(r => r.ReservationId == id);
        if (i >= 0)
        {
            var r = _db.Reservations[i];
            _db.Reservations[i] = r with { Status = status };
        }
        return Task.CompletedTask;
    }

    public Task<ReservationDto?> GetByIdAsync(int id, CancellationToken _) =>
        Task.FromResult(_db.Reservations.FirstOrDefault(r => r.ReservationId == id));
}

public class InMemoryUserRepository : IUserRepository
{
    private readonly InMemoryStore _db;
    public InMemoryUserRepository(InMemoryStore db) => _db = db;
    public Task<UserRecord?> GetByEmailAsync(string email, CancellationToken _) =>
        Task.FromResult(_db.Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)));
    public Task<UserRecord?> GetByIdAsync(int id, CancellationToken _) =>
        Task.FromResult(_db.Users.FirstOrDefault(u => u.UserId == id));
    public Task<int> CreateAsync(int roleId, string fullName, string email, string phone, string passwordHash, string? avatar, CancellationToken _)
    {
        var id = _db.NextUser++;
        var role = roleId == 1 ? "Admin" : "Customer";
        _db.Users.Add(new UserRecord(id, roleId, role, fullName, email, phone, passwordHash, avatar, null));
        return Task.FromResult(id);
    }
    public Task UpdateProfileAsync(int userId, UpdateProfileDto request, CancellationToken _)
    {
        var i = _db.Users.FindIndex(u => u.UserId == userId);
        if (i >= 0)
        {
            var u = _db.Users[i];
            _db.Users[i] = u with { FullName = request.FullName, Phone = request.Phone, Address = request.Address, AvatarUrl = request.AvatarUrl };
        }
        return Task.CompletedTask;
    }
}

public class InMemoryFavouriteRepository : IFavouriteRepository
{
    private readonly InMemoryStore _db;
    public InMemoryFavouriteRepository(InMemoryStore db) => _db = db;
    public Task AddAsync(int userId, int menuItemId, CancellationToken _)
    {
        if (!_db.Favourites.Any(f => f.UserId == userId && f.MenuItemId == menuItemId))
            _db.Favourites.Add((userId, menuItemId));
        return Task.CompletedTask;
    }
    public Task RemoveAsync(int userId, int menuItemId, CancellationToken _)
    {
        _db.Favourites.RemoveAll(f => f.UserId == userId && f.MenuItemId == menuItemId);
        return Task.CompletedTask;
    }
    public Task<IEnumerable<MenuItemDto>> GetByUserAsync(int userId, CancellationToken _)
    {
        var ids = _db.Favourites.Where(f => f.UserId == userId).Select(f => f.MenuItemId).ToHashSet();
        return Task.FromResult(_db.Menu.Where(m => ids.Contains(m.MenuItemId)).AsEnumerable());
    }
}

public class InMemoryGalleryRepository : IGalleryRepository
{
    private readonly InMemoryStore _db;
    public InMemoryGalleryRepository(InMemoryStore db) => _db = db;
    public Task<IEnumerable<GalleryDto>> GetAllAsync(CancellationToken _) => Task.FromResult(_db.Gallery.AsEnumerable());
    public Task<int> CreateAsync(string title, string imageUrl, string? caption, int sort, CancellationToken _)
    {
        var id = _db.NextGal++;
        _db.Gallery.Add(new GalleryDto(id, title, imageUrl, caption));
        return Task.FromResult(id);
    }
}

public class InMemoryEventRepository : IEventRepository
{
    private readonly InMemoryStore _db;
    public InMemoryEventRepository(InMemoryStore db) => _db = db;
    public Task<IEnumerable<EventDto>> GetAllAsync(CancellationToken _) => Task.FromResult(_db.Events.AsEnumerable());
    public Task<int> CreateAsync(string title, string description, DateTime eventDate, string imageUrl, CancellationToken _)
    {
        var id = _db.NextEvt++;
        _db.Events.Add(new EventDto(id, title, description, eventDate, imageUrl));
        return Task.FromResult(id);
    }
}

public class InMemoryContactRepository : IContactRepository
{
    private readonly InMemoryStore _db;
    public InMemoryContactRepository(InMemoryStore db) => _db = db;
    public Task CreateAsync(ContactDto request, CancellationToken _)
    {
        _db.Contacts.Add(request);
        return Task.CompletedTask;
    }
}
