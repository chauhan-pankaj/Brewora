using Brewora.Application.Common;
using Brewora.Application.DTOs;
using Brewora.Application.Interfaces;
using Brewora.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Brewora.API.Controllers;

[ApiController]
[Route("api/menu")]
public class MenuController : ControllerBase
{
    private readonly MenuService _svc;
    public MenuController(MenuService svc) => _svc = svc;

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<MenuItemDto>>.Ok(await _svc.GetAllAsync(ct)));

    [HttpGet("featured")]
    public async Task<IActionResult> Featured(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<MenuItemDto>>.Ok(await _svc.GetFeaturedAsync(ct)));

    [HttpGet("category/{categoryId:int}")]
    public async Task<IActionResult> ByCategory(int categoryId, CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<MenuItemDto>>.Ok(await _svc.GetByCategoryAsync(categoryId, ct)));

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct) =>
        Ok(ApiResponse<MenuItemDto>.Ok(await _svc.GetByIdAsync(id, ct)));
}

[ApiController]
[Route("api/categories")]
public class CategoriesController : ControllerBase
{
    private readonly CategoryService _svc;
    public CategoriesController(CategoryService svc) => _svc = svc;
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<CategoryDto>>.Ok(await _svc.GetAllAsync(ct)));
}

[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly OrderService _svc;
    public OrdersController(OrderService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderDto dto, CancellationToken ct)
    {
        var uid = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (int?)null;
        return Ok(ApiResponse<OrderDto>.Ok(await _svc.CreateAsync(dto, uid, ct), "Order created"));
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id, CancellationToken ct) =>
        Ok(ApiResponse<OrderDto>.Ok(await _svc.GetByIdAsync(id, ct)));

    [Authorize]
    [HttpGet("my-orders")]
    public async Task<IActionResult> Mine(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(await _svc.MineAsync(User.GetUserId(), ct)));
}

[ApiController]
[Route("api/payment")]
public class PaymentController : ControllerBase
{
    private readonly PaymentService _svc;
    public PaymentController(PaymentService svc) => _svc = svc;

    [HttpPost("create-order")]
    public async Task<IActionResult> Create([FromBody] CreatePaymentOrderDto dto, CancellationToken ct) =>
        Ok(ApiResponse<RazorpayOrderDto>.Ok(await _svc.CreateRazorpayOrderAsync(dto.BreworaOrderId, ct)));

    [HttpPost("verify")]
    public async Task<IActionResult> Verify([FromBody] VerifyPaymentDto dto, CancellationToken ct)
    {
        var (paid, number) = await _svc.VerifyAsync(dto, ct);
        return Ok(ApiResponse<object>.Ok(new { paid, orderNumber = number }, "Payment verified"));
    }
}

[ApiController]
[Route("api/reservations")]
public class ReservationsController : ControllerBase
{
    private readonly ReservationService _svc;
    public ReservationsController(ReservationService svc) => _svc = svc;

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateReservationDto dto, CancellationToken ct)
    {
        var uid = User.Identity?.IsAuthenticated == true ? User.GetUserId() : (int?)null;
        return Ok(ApiResponse<ReservationDto>.Ok(await _svc.CreateAsync(dto, uid, ct), "Reservation confirmed"));
    }

    [HttpGet("availability")]
    public async Task<IActionResult> Availability([FromQuery] DateTime date, [FromQuery] int guests = 2, CancellationToken ct = default) =>
        Ok(ApiResponse<IEnumerable<SlotDto>>.Ok(await _svc.AvailabilityAsync(date, guests, ct)));

    [Authorize]
    [HttpGet("my-reservations")]
    public async Task<IActionResult> Mine(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<ReservationDto>>.Ok(await _svc.MineAsync(User.GetUserId(), ct)));
}

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthService _svc;
    public AuthController(AuthService svc) => _svc = svc;
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct) =>
        Ok(ApiResponse<AuthResultDto>.Ok(await _svc.RegisterAsync(dto, ct)));
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct) =>
        Ok(ApiResponse<AuthResultDto>.Ok(await _svc.LoginAsync(dto, ct)));
}

[ApiController]
[Authorize]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly AuthService _svc;
    public UsersController(AuthService svc) => _svc = svc;
    [HttpGet("profile")]
    public async Task<IActionResult> Profile(CancellationToken ct) =>
        Ok(ApiResponse<UserProfileDto>.Ok(await _svc.ProfileAsync(User.GetUserId(), ct)));
    [HttpPut("profile")]
    public async Task<IActionResult> Update([FromBody] UpdateProfileDto dto, CancellationToken ct) =>
        Ok(ApiResponse<UserProfileDto>.Ok(await _svc.UpdateAsync(User.GetUserId(), dto, ct)));
}

[ApiController]
[Authorize]
[Route("api/favourites")]
public class FavouritesController : ControllerBase
{
    private readonly IFavouriteRepository _repo;
    public FavouritesController(IFavouriteRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<MenuItemDto>>.Ok(await _repo.GetByUserAsync(User.GetUserId(), ct)));

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] FavouriteRequest dto, CancellationToken ct)
    {
        await _repo.AddAsync(User.GetUserId(), dto.MenuItemId, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Saved"));
    }

    [HttpDelete("{menuItemId:int}")]
    public async Task<IActionResult> Remove(int menuItemId, CancellationToken ct)
    {
        await _repo.RemoveAsync(User.GetUserId(), menuItemId, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Removed"));
    }
}

[ApiController]
[Route("api/gallery")]
public class GalleryController : ControllerBase
{
    private readonly IGalleryRepository _repo;
    public GalleryController(IGalleryRepository repo) => _repo = repo;
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<GalleryDto>>.Ok(await _repo.GetAllAsync(ct)));
}

[ApiController]
[Route("api/events")]
public class EventsController : ControllerBase
{
    private readonly IEventRepository _repo;
    public EventsController(IEventRepository repo) => _repo = repo;
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<EventDto>>.Ok(await _repo.GetAllAsync(ct)));
}

[ApiController]
[Route("api/contact")]
public class ContactController : ControllerBase
{
    private readonly IContactRepository _repo;
    public ContactController(IContactRepository repo) => _repo = repo;
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ContactDto dto, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Name) || string.IsNullOrWhiteSpace(dto.Message))
            throw new AppException("Name and message are required");
        await _repo.CreateAsync(dto, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Message received"));
    }
}

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin")]
public class AdminController : ControllerBase
{
    private readonly MenuService _menu;
    private readonly OrderService _orders;
    private readonly ReservationService _reservations;
    private readonly IGalleryRepository _gallery;
    private readonly IEventRepository _events;
    private readonly ICategoryRepository _categories;

    public AdminController(MenuService menu, OrderService orders, ReservationService reservations,
        IGalleryRepository gallery, IEventRepository events, ICategoryRepository categories)
    {
        _menu = menu; _orders = orders; _reservations = reservations;
        _gallery = gallery; _events = events; _categories = categories;
    }

    [HttpPost("menu")]
    public async Task<IActionResult> CreateMenu([FromBody] CreateMenuItemDto dto, CancellationToken ct) =>
        Ok(ApiResponse<int>.Ok(await _menu.CreateAsync(dto, ct), "Menu item created"));

    [HttpPut("menu/{id:int}")]
    public async Task<IActionResult> UpdateMenu(int id, [FromBody] CreateMenuItemDto dto, CancellationToken ct)
    {
        await _menu.UpdateAsync(id, dto, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Menu item updated"));
    }

    [HttpGet("orders")]
    public async Task<IActionResult> Orders(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<OrderDto>>.Ok(await _orders.AllAsync(ct)));

    [HttpPut("orders/{id:int}/status")]
    public async Task<IActionResult> Status(int id, [FromBody] UpdateOrderStatusDto dto, CancellationToken ct)
    {
        await _orders.UpdateStatusAsync(id, dto.OrderStatus, ct);
        return Ok(ApiResponse<object?>.Ok(null, "Status updated"));
    }

    [HttpGet("reservations")]
    public async Task<IActionResult> Reservations(CancellationToken ct) =>
        Ok(ApiResponse<IEnumerable<ReservationDto>>.Ok(await _reservations.AllAsync(ct)));

    [HttpPost("gallery")]
    public async Task<IActionResult> Gallery([FromBody] GalleryDto dto, CancellationToken ct) =>
        Ok(ApiResponse<int>.Ok(await _gallery.CreateAsync(dto.Title, dto.ImageUrl, dto.Caption, 0, ct)));

    [HttpPost("events")]
    public async Task<IActionResult> Events([FromBody] EventDto dto, CancellationToken ct) =>
        Ok(ApiResponse<int>.Ok(await _events.CreateAsync(dto.Title, dto.Description, dto.EventDate, dto.ImageUrl, ct)));

    [HttpPost("categories")]
    public async Task<IActionResult> Categories([FromBody] CategoryDto dto, CancellationToken ct) =>
        Ok(ApiResponse<int>.Ok(await _categories.CreateAsync(dto.Name, dto.Slug, dto.Icon ?? "", ct)));
}
