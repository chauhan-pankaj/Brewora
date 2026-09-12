USE BreworaDB;
GO

SET IDENTITY_INSERT dbo.Roles ON;
INSERT INTO dbo.Roles (RoleId, Name) VALUES (1, N'Admin'), (2, N'Customer');
SET IDENTITY_INSERT dbo.Roles OFF;

-- Passwords are hashed in the API seeder (never stored in plain text).
-- Catalog photography uses the same Unsplash URLs as the mobile UI reference.

SET IDENTITY_INSERT dbo.Categories ON;
INSERT INTO dbo.Categories (CategoryId, Name, Slug, Icon) VALUES
 (1, N'Coffee', N'coffee', N'☕'),
 (2, N'Food', N'food', N'🥐'),
 (3, N'Desserts', N'desserts', N'🍰'),
 (4, N'Beverages', N'beverages', N'🥤');
SET IDENTITY_INSERT dbo.Categories OFF;

INSERT INTO dbo.MenuItems (CategoryId, Name, Description, Price, ImageUrl, IsFeatured, IsAvailable) VALUES
(1, N'Classic Latte', N'Smooth espresso with steamed milk.', 180,
 N'https://images.unsplash.com/photo-1541167760496-1628856ab772?auto=format&fit=crop&w=900&q=90', 1, 1),
(1, N'Cappuccino', N'Rich, bold and foamy.', 190,
 N'https://images.unsplash.com/photo-1509042239860-f550ce710b93?auto=format&fit=crop&w=800&q=85', 1, 1),
(1, N'Iced Cold Brew', N'Refreshing and perfect anytime.', 190,
 N'https://images.unsplash.com/photo-1517701604599-bb29b565090c?auto=format&fit=crop&w=800&q=85', 1, 1),
(1, N'Hazelnut Mocha', N'Chocolate and hazelnut blend.', 210,
 N'https://images.unsplash.com/photo-1578374173705-14e4a6f7d9b3?auto=format&fit=crop&w=800&q=85', 0, 1),
(2, N'Creamy Alfredo Pasta', N'Silky parmesan sauce over ribbons of pasta.', 320,
 N'https://images.unsplash.com/photo-1473093295043-cdd812d0e601?auto=format&fit=crop&w=800&q=85', 0, 1),
(2, N'Grilled Club Sandwich', N'Toasted sourdough, greens, and house sauce.', 220,
 N'https://images.unsplash.com/photo-1528735602780-2552fd46c7af?auto=format&fit=crop&w=800&q=85', 0, 1),
(3, N'Chocolate Brownie', N'Warm cocoa crumb with a molten centre.', 250,
 N'https://images.unsplash.com/photo-1606313564200-e75d5e30476c?auto=format&fit=crop&w=800&q=85', 1, 1),
(3, N'Berry Cheesecake', N'Baked cheesecake with berry conserve.', 280,
 N'https://images.unsplash.com/photo-1533134242443-d4fd215305ad?auto=format&fit=crop&w=800&q=85', 0, 1),
(4, N'Fresh Lime Soda', N'Bright, cold, and lightly sweet.', 120,
 N'https://images.unsplash.com/photo-1556679343-c7306c197fb4?auto=format&fit=crop&w=800&q=85', 0, 1),
(4, N'Sparkling Iced Tea', N'House brew over ice with citrus.', 140,
 N'https://images.unsplash.com/photo-1556679343-c7306c197fb4?auto=format&fit=crop&w=800&q=85', 0, 1);

INSERT INTO dbo.ReservationSlots (SlotTime, Capacity) VALUES
(N'11:00', 12),(N'12:30', 12),(N'13:00', 12),(N'17:00', 10),(N'19:00', 10),(N'20:30', 8),(N'21:30', 8);

INSERT INTO dbo.Gallery (Title, ImageUrl, Caption, SortOrder) VALUES
(N'Warm interiors', N'https://images.unsplash.com/photo-1554118811-1e0d58224f24?auto=format&fit=crop&w=800&q=90', N'More than just coffee', 1),
(N'Latte art', N'https://images.unsplash.com/photo-1541167760496-1628856ab772?auto=format&fit=crop&w=800&q=90', N'Everyday ritual', 2),
(N'Beans', N'https://images.unsplash.com/photo-1447933601403-0c6688de566e?auto=format&fit=crop&w=800&q=85', N'Sourced with care', 3),
(N'Table for two', N'https://images.unsplash.com/photo-1517248135467-4c7edcad34c4?auto=format&fit=crop&w=800&q=90', N'Good company', 4);

INSERT INTO dbo.Events (Title, Description, EventDate, ImageUrl) VALUES
(N'Sunday Slow Brunch', N'Bottomless filter coffee and kitchen plates from 11.', DATEADD(DAY, 10, CAST(GETDATE() AS DATE)),
 N'https://images.unsplash.com/photo-1414235077428-338989a2e8c0?auto=format&fit=crop&w=800&q=90'),
(N'Live Acoustic Evening', N'Soft sets, warm lights, reserved tables recommended.', DATEADD(DAY, 18, CAST(GETDATE() AS DATE)),
 N'https://images.unsplash.com/photo-1511081692775-05d0f180a065?auto=format&fit=crop&w=800&q=85');
GO
