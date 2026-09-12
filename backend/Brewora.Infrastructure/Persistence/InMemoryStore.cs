using Brewora.Application.DTOs;
using Brewora.Application.Interfaces;

namespace Brewora.Infrastructure.Persistence;

public class InMemoryStore
{
    public List<UserRecord> Users { get; } = new();
    public List<CategoryDto> Categories { get; } = new();
    public List<MenuItemDto> Menu { get; } = new();
    public List<OrderDto> Orders { get; } = new();
    public List<OrderItemDto> OrderItems { get; } = new();
    public List<PaymentWrite> Payments { get; } = new();
    public List<ReservationDto> Reservations { get; } = new();
    public List<(string Time, int Capacity)> Slots { get; } = new();
    public List<(int UserId, int MenuItemId)> Favourites { get; } = new();
    public List<GalleryDto> Gallery { get; } = new();
    public List<EventDto> Events { get; } = new();
    public List<ContactDto> Contacts { get; } = new();
    public int NextUser = 1;
    public int NextMenu = 1;
    public int NextOrder = 1;
    public int NextItem = 1;
    public int NextPay = 1;
    public int NextRes = 1;
    public int NextGal = 1;
    public int NextEvt = 1;
    public int NextCat = 1;

    public InMemoryStore()
    {
        Categories.AddRange(new[]
        {
            new CategoryDto(1, "Coffee", "coffee", "☕"),
            new CategoryDto(2, "Food", "food", "🥐"),
            new CategoryDto(3, "Desserts", "desserts", "🍰"),
            new CategoryDto(4, "Beverages", "beverages", "🥤"),
        });
        NextCat = 5;

        Menu.AddRange(new[]
        {
            Item(1, 1, "Coffee", "Classic Latte", "Smooth espresso with steamed milk.", 180,
                "https://images.unsplash.com/photo-1541167760496-1628856ab772?auto=format&fit=crop&w=900&q=90", true),
            Item(2, 1, "Coffee", "Cappuccino", "Rich, bold and foamy.", 190,
                "https://images.unsplash.com/photo-1509042239860-f550ce710b93?auto=format&fit=crop&w=800&q=85", true),
            Item(3, 1, "Coffee", "Iced Cold Brew", "Refreshing and perfect anytime.", 190,
                "https://images.unsplash.com/photo-1517701604599-bb29b565090c?auto=format&fit=crop&w=800&q=85", true),
            Item(4, 1, "Coffee", "Hazelnut Mocha", "Chocolate and hazelnut blend.", 210,
                "https://images.unsplash.com/photo-1578374173705-14e4a6f7d9b3?auto=format&fit=crop&w=800&q=85", false),
            Item(5, 2, "Food", "Creamy Alfredo Pasta", "Silky parmesan sauce over ribbons of pasta.", 320,
                "https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&w=800&q=85", false),
            Item(6, 2, "Food", "Grilled Club Sandwich", "Toasted sourdough, greens, and house sauce.", 220,
                "https://images.unsplash.com/photo-1528735602780-2552fd46c7af?auto=format&fit=crop&w=800&q=85", false),
            Item(7, 3, "Desserts", "Chocolate Brownie", "Warm cocoa crumb with a molten centre.", 250,
                "https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&w=800&q=85", true),
            Item(8, 3, "Desserts", "Berry Cheesecake", "Baked cheesecake with berry conserve.", 280,
                "https://images.unsplash.com/photo-1533134242443-d4fd215305ad?auto=format&fit=crop&w=800&q=85", false),
            Item(9, 4, "Beverages", "Fresh Lime Soda", "Bright, cold, and lightly sweet.", 120,
                "https://images.unsplash.com/photo-1556679343-c7306c197fb4?auto=format&fit=crop&w=800&q=85", false),
            Item(10, 4, "Beverages", "Sparkling Iced Tea", "House brew over ice with citrus.", 140,
                "https://images.unsplash.com/photo-1556679343-c7306c197fb4?auto=format&fit=crop&w=800&q=85", false),
        });
        NextMenu = 11;

        Users.Add(new UserRecord(1, 1, "Admin", "Brewora Host", "nina.v@example.com", "+918040001200",
            BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80",
            "Indiranagar, Bangalore"));
        Users.Add(new UserRecord(2, 2, "Customer", "Customer Name", "customer@email.com", "+91 XXXXX XXXXX",
            BCrypt.Net.BCrypt.HashPassword("Customer@123"),
            "https://images.unsplash.com/photo-1500648767791-00dcc994a43e?auto=format&fit=crop&w=200&q=80",
            "Indiranagar, Bangalore"));
        NextUser = 3;

        Slots.AddRange(new[]
        {
            ("11:00", 12), ("12:30", 12), ("13:00", 12), ("17:00", 10), ("19:00", 10), ("20:30", 8), ("21:30", 8)
        });

        Gallery.AddRange(new[]
        {
            new GalleryDto(1, "Warm interiors", "https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=800&q=90", "More than just coffee"),
            new GalleryDto(2, "Latte art", "https://images.unsplash.com/photo-1541167760496-1628856ab772?auto=format&fit=crop&w=800&q=90", "Everyday ritual"),
            new GalleryDto(3, "Beans", "https://images.unsplash.com/photo-1447933601403-0c6688de566e?auto=format&fit=crop&w=800&q=85", "Sourced with care"),
            new GalleryDto(4, "Table for two", "https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=800&q=90", "Good company"),
        });
        NextGal = 5;

        Events.Add(new EventDto(1, "Sunday Slow Brunch", "Bottomless filter coffee and kitchen plates from 11.", DateTime.UtcNow.Date.AddDays(10),
            "https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=800&q=90"));
        Events.Add(new EventDto(2, "Live Acoustic Evening", "Soft sets, warm lights, reserved tables recommended.", DateTime.UtcNow.Date.AddDays(18),
            "https://images.unsplash.com/photo-1511081692775-05d0f180a065?auto=format&fit=crop&w=800&q=85"));
        NextEvt = 3;
    }

    private static MenuItemDto Item(int id, int cat, string catName, string name, string desc, decimal price, string img, bool featured) =>
        new(id, cat, catName, name, desc, price, img, featured, true);
}
