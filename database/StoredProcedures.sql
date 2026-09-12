USE BreworaDB;
GO

CREATE OR ALTER PROCEDURE dbo.sp_Menu_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.MenuItemId, m.CategoryId, c.Name AS CategoryName, m.Name, m.Description,
           m.Price, m.ImageUrl, m.IsFeatured, m.IsAvailable, m.CreatedDate, m.ModifiedDate
    FROM dbo.MenuItems m
    INNER JOIN dbo.Categories c ON c.CategoryId = m.CategoryId
    WHERE m.IsAvailable = 1
    ORDER BY c.CategoryId, m.Name;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Menu_GetById
    @MenuItemId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.MenuItemId, m.CategoryId, c.Name AS CategoryName, m.Name, m.Description,
           m.Price, m.ImageUrl, m.IsFeatured, m.IsAvailable, m.CreatedDate, m.ModifiedDate
    FROM dbo.MenuItems m
    INNER JOIN dbo.Categories c ON c.CategoryId = m.CategoryId
    WHERE m.MenuItemId = @MenuItemId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Menu_GetFeatured
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.MenuItemId, m.CategoryId, c.Name AS CategoryName, m.Name, m.Description,
           m.Price, m.ImageUrl, m.IsFeatured, m.IsAvailable, m.CreatedDate, m.ModifiedDate
    FROM dbo.MenuItems m
    INNER JOIN dbo.Categories c ON c.CategoryId = m.CategoryId
    WHERE m.IsFeatured = 1 AND m.IsAvailable = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Menu_GetByCategory
    @CategoryId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT m.MenuItemId, m.CategoryId, c.Name AS CategoryName, m.Name, m.Description,
           m.Price, m.ImageUrl, m.IsFeatured, m.IsAvailable, m.CreatedDate, m.ModifiedDate
    FROM dbo.MenuItems m
    INNER JOIN dbo.Categories c ON c.CategoryId = m.CategoryId
    WHERE m.CategoryId = @CategoryId AND m.IsAvailable = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Menu_Create
    @CategoryId INT,
    @Name NVARCHAR(160),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500),
    @IsFeatured BIT,
    @IsAvailable BIT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.MenuItems (CategoryId, Name, Description, Price, ImageUrl, IsFeatured, IsAvailable)
    VALUES (@CategoryId, @Name, @Description, @Price, @ImageUrl, @IsFeatured, @IsAvailable);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS MenuItemId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Menu_Update
    @MenuItemId INT,
    @CategoryId INT,
    @Name NVARCHAR(160),
    @Description NVARCHAR(500),
    @Price DECIMAL(10,2),
    @ImageUrl NVARCHAR(500),
    @IsFeatured BIT,
    @IsAvailable BIT
AS
BEGIN
    SET NOCOUNT ON;
    UPDATE dbo.MenuItems
    SET CategoryId=@CategoryId, Name=@Name, Description=@Description, Price=@Price,
        ImageUrl=@ImageUrl, IsFeatured=@IsFeatured, IsAvailable=@IsAvailable, ModifiedDate=SYSUTCDATETIME()
    WHERE MenuItemId=@MenuItemId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Category_GetAll
AS
BEGIN
    SET NOCOUNT ON;
    SELECT CategoryId, Name, Slug, Icon FROM dbo.Categories WHERE IsActive = 1;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Category_Create
    @Name NVARCHAR(80),
    @Slug NVARCHAR(80),
    @Icon NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.Categories (Name, Slug, Icon) VALUES (@Name, @Slug, @Icon);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS CategoryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_Create
    @OrderNumber NVARCHAR(30),
    @UserId INT = NULL,
    @CustomerName NVARCHAR(120),
    @CustomerEmail NVARCHAR(200),
    @CustomerPhone NVARCHAR(30),
    @DeliveryAddress NVARCHAR(400),
    @Subtotal DECIMAL(10,2),
    @DeliveryCharges DECIMAL(10,2),
    @Discount DECIMAL(10,2),
    @Tax DECIMAL(10,2),
    @TotalAmount DECIMAL(10,2),
    @OrderStatus NVARCHAR(40),
    @PaymentStatus NVARCHAR(40)
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO dbo.Orders (OrderNumber, UserId, CustomerName, CustomerEmail, CustomerPhone, DeliveryAddress,
        Subtotal, DeliveryCharges, Discount, Tax, TotalAmount, OrderStatus, PaymentStatus)
    VALUES (@OrderNumber, @UserId, @CustomerName, @CustomerEmail, @CustomerPhone, @DeliveryAddress,
        @Subtotal, @DeliveryCharges, @Discount, @Tax, @TotalAmount, @OrderStatus, @PaymentStatus);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS OrderId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_GetById
    @OrderId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Orders WHERE OrderId = @OrderId;
    SELECT oi.*, m.Name, m.ImageUrl
    FROM dbo.OrderItems oi
    INNER JOIN dbo.MenuItems m ON m.MenuItemId = oi.MenuItemId
    WHERE oi.OrderId = @OrderId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_GetByUser
    @UserId INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Orders WHERE UserId = @UserId ORDER BY CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_GetByEmail
    @Email NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.Orders WHERE CustomerEmail = @Email ORDER BY CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_UpdatePayment
    @OrderId INT,
    @PaymentStatus NVARCHAR(40),
    @OrderStatus NVARCHAR(40),
    @RazorpayOrderId NVARCHAR(80) = NULL
AS
BEGIN
    UPDATE dbo.Orders
    SET PaymentStatus=@PaymentStatus, OrderStatus=@OrderStatus, RazorpayOrderId=@RazorpayOrderId,
        ModifiedDate=SYSUTCDATETIME()
    WHERE OrderId=@OrderId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_UpdateStatus
    @OrderId INT,
    @OrderStatus NVARCHAR(40)
AS
BEGIN
    UPDATE dbo.Orders SET OrderStatus=@OrderStatus, ModifiedDate=SYSUTCDATETIME() WHERE OrderId=@OrderId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Order_GetAll
AS
BEGIN
    SELECT * FROM dbo.Orders ORDER BY CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_OrderItems_Create
    @OrderId INT,
    @MenuItemId INT,
    @Quantity INT,
    @UnitPrice DECIMAL(10,2),
    @TotalPrice DECIMAL(10,2),
    @Size NVARCHAR(20)
AS
BEGIN
    INSERT INTO dbo.OrderItems (OrderId, MenuItemId, Quantity, UnitPrice, TotalPrice, Size)
    VALUES (@OrderId, @MenuItemId, @Quantity, @UnitPrice, @TotalPrice, @Size);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Payment_Create
    @OrderId INT,
    @RazorpayOrderId NVARCHAR(80),
    @RazorpayPaymentId NVARCHAR(80),
    @RazorpaySignature NVARCHAR(200),
    @Amount DECIMAL(10,2),
    @Currency NVARCHAR(10),
    @PaymentStatus NVARCHAR(40),
    @PaymentMethod NVARCHAR(40)
AS
BEGIN
    INSERT INTO dbo.Payments (OrderId, RazorpayOrderId, RazorpayPaymentId, RazorpaySignature, Amount, Currency, PaymentStatus, PaymentMethod)
    VALUES (@OrderId, @RazorpayOrderId, @RazorpayPaymentId, @RazorpaySignature, @Amount, @Currency, @PaymentStatus, @PaymentMethod);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS PaymentId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Payment_GetByOrderId
    @OrderId INT
AS
BEGIN
    SELECT * FROM dbo.Payments WHERE OrderId = @OrderId ORDER BY CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Payment_UpdateStatus
    @PaymentId INT,
    @PaymentStatus NVARCHAR(40),
    @RazorpayPaymentId NVARCHAR(80) = NULL,
    @RazorpaySignature NVARCHAR(200) = NULL
AS
BEGIN
    UPDATE dbo.Payments
    SET PaymentStatus=@PaymentStatus,
        RazorpayPaymentId=COALESCE(@RazorpayPaymentId, RazorpayPaymentId),
        RazorpaySignature=COALESCE(@RazorpaySignature, RazorpaySignature)
    WHERE PaymentId=@PaymentId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reservation_Create
    @UserId INT = NULL,
    @CustomerName NVARCHAR(120),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(30),
    @ReservationDate DATE,
    @ReservationTime NVARCHAR(10),
    @GuestCount INT,
    @SpecialRequest NVARCHAR(400),
    @Status NVARCHAR(40)
AS
BEGIN
    INSERT INTO dbo.Reservations (UserId, CustomerName, Email, Phone, ReservationDate, ReservationTime, GuestCount, SpecialRequest, Status)
    VALUES (@UserId, @CustomerName, @Email, @Phone, @ReservationDate, @ReservationTime, @GuestCount, @SpecialRequest, @Status);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS ReservationId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reservation_GetAvailability
    @ReservationDate DATE
AS
BEGIN
    SET NOCOUNT ON;
    SELECT s.SlotTime AS [Time],
           s.Capacity,
           s.Capacity - ISNULL((
                SELECT SUM(r.GuestCount)
                FROM dbo.Reservations r
                WHERE r.ReservationDate = @ReservationDate
                  AND r.ReservationTime = s.SlotTime
                  AND r.Status NOT IN ('Cancelled')
           ), 0) AS Remaining
    FROM dbo.ReservationSlots s;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reservation_GetByUser
    @UserId INT
AS
BEGIN
    SELECT * FROM dbo.Reservations WHERE UserId = @UserId ORDER BY ReservationDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reservation_GetAll
AS
BEGIN
    SELECT * FROM dbo.Reservations ORDER BY CreatedDate DESC;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Reservation_UpdateStatus
    @ReservationId INT,
    @Status NVARCHAR(40)
AS
BEGIN
    UPDATE dbo.Reservations SET Status=@Status WHERE ReservationId=@ReservationId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_User_GetByEmail
    @Email NVARCHAR(200)
AS
BEGIN
    SELECT u.*, r.Name AS RoleName
    FROM dbo.Users u
    INNER JOIN dbo.Roles r ON r.RoleId = u.RoleId
    WHERE u.Email = @Email;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_User_GetById
    @UserId INT
AS
BEGIN
    SELECT u.*, r.Name AS RoleName
    FROM dbo.Users u
    INNER JOIN dbo.Roles r ON r.RoleId = u.RoleId
    WHERE u.UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_User_Create
    @RoleId INT,
    @FullName NVARCHAR(120),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(30),
    @PasswordHash NVARCHAR(300),
    @AvatarUrl NVARCHAR(500) = NULL
AS
BEGIN
    INSERT INTO dbo.Users (RoleId, FullName, Email, Phone, PasswordHash, AvatarUrl)
    VALUES (@RoleId, @FullName, @Email, @Phone, @PasswordHash, @AvatarUrl);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS UserId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_User_UpdateProfile
    @UserId INT,
    @FullName NVARCHAR(120),
    @Phone NVARCHAR(30),
    @Address NVARCHAR(400),
    @AvatarUrl NVARCHAR(500)
AS
BEGIN
    UPDATE dbo.Users
    SET FullName=@FullName, Phone=@Phone, Address=@Address, AvatarUrl=@AvatarUrl, ModifiedDate=SYSUTCDATETIME()
    WHERE UserId=@UserId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Favourite_Add
    @UserId INT,
    @MenuItemId INT
AS
BEGIN
    IF NOT EXISTS (SELECT 1 FROM dbo.Favourites WHERE UserId=@UserId AND MenuItemId=@MenuItemId)
        INSERT INTO dbo.Favourites (UserId, MenuItemId) VALUES (@UserId, @MenuItemId);
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Favourite_Remove
    @UserId INT,
    @MenuItemId INT
AS
BEGIN
    DELETE FROM dbo.Favourites WHERE UserId=@UserId AND MenuItemId=@MenuItemId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Favourite_GetByUser
    @UserId INT
AS
BEGIN
    SELECT m.MenuItemId, m.CategoryId, c.Name AS CategoryName, m.Name, m.Description,
           m.Price, m.ImageUrl, m.IsFeatured, m.IsAvailable, m.CreatedDate, m.ModifiedDate
    FROM dbo.Favourites f
    INNER JOIN dbo.MenuItems m ON m.MenuItemId = f.MenuItemId
    INNER JOIN dbo.Categories c ON c.CategoryId = m.CategoryId
    WHERE f.UserId = @UserId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Gallery_GetAll
AS
BEGIN
    SELECT * FROM dbo.Gallery ORDER BY SortOrder, GalleryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Gallery_Create
    @Title NVARCHAR(120),
    @ImageUrl NVARCHAR(500),
    @Caption NVARCHAR(300),
    @SortOrder INT
AS
BEGIN
    INSERT INTO dbo.Gallery (Title, ImageUrl, Caption, SortOrder) VALUES (@Title, @ImageUrl, @Caption, @SortOrder);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS GalleryId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Event_GetAll
AS
BEGIN
    SELECT * FROM dbo.Events ORDER BY EventDate;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Event_Create
    @Title NVARCHAR(160),
    @Description NVARCHAR(500),
    @EventDate DATE,
    @ImageUrl NVARCHAR(500)
AS
BEGIN
    INSERT INTO dbo.Events (Title, Description, EventDate, ImageUrl)
    VALUES (@Title, @Description, @EventDate, @ImageUrl);
    SELECT CAST(SCOPE_IDENTITY() AS INT) AS EventId;
END
GO

CREATE OR ALTER PROCEDURE dbo.sp_Contact_Create
    @Name NVARCHAR(120),
    @Email NVARCHAR(200),
    @Phone NVARCHAR(30),
    @Message NVARCHAR(1000)
AS
BEGIN
    INSERT INTO dbo.ContactMessages (Name, Email, Phone, Message)
    VALUES (@Name, @Email, @Phone, @Message);
END
GO
