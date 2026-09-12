-- BreworaDB schema (SQL Server). Do not use Entity Framework.
IF DB_ID('BreworaDB') IS NULL
    CREATE DATABASE BreworaDB;
GO
USE BreworaDB;
GO

IF OBJECT_ID('dbo.OrderItems', 'U') IS NOT NULL DROP TABLE dbo.OrderItems;
IF OBJECT_ID('dbo.Payments', 'U') IS NOT NULL DROP TABLE dbo.Payments;
IF OBJECT_ID('dbo.Orders', 'U') IS NOT NULL DROP TABLE dbo.Orders;
IF OBJECT_ID('dbo.Favourites', 'U') IS NOT NULL DROP TABLE dbo.Favourites;
IF OBJECT_ID('dbo.Addresses', 'U') IS NOT NULL DROP TABLE dbo.Addresses;
IF OBJECT_ID('dbo.Reservations', 'U') IS NOT NULL DROP TABLE dbo.Reservations;
IF OBJECT_ID('dbo.ReservationSlots', 'U') IS NOT NULL DROP TABLE dbo.ReservationSlots;
IF OBJECT_ID('dbo.MenuItems', 'U') IS NOT NULL DROP TABLE dbo.MenuItems;
IF OBJECT_ID('dbo.Categories', 'U') IS NOT NULL DROP TABLE dbo.Categories;
IF OBJECT_ID('dbo.Gallery', 'U') IS NOT NULL DROP TABLE dbo.Gallery;
IF OBJECT_ID('dbo.Events', 'U') IS NOT NULL DROP TABLE dbo.Events;
IF OBJECT_ID('dbo.ContactMessages', 'U') IS NOT NULL DROP TABLE dbo.ContactMessages;
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL DROP TABLE dbo.Users;
IF OBJECT_ID('dbo.Roles', 'U') IS NOT NULL DROP TABLE dbo.Roles;
GO

CREATE TABLE dbo.Roles (
    RoleId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE dbo.Users (
    UserId INT IDENTITY PRIMARY KEY,
    RoleId INT NOT NULL REFERENCES dbo.Roles(RoleId),
    FullName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(200) NOT NULL UNIQUE,
    Phone NVARCHAR(30) NULL,
    PasswordHash NVARCHAR(300) NOT NULL,
    AvatarUrl NVARCHAR(500) NULL,
    Address NVARCHAR(400) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedDate DATETIME2 NULL
);

CREATE TABLE dbo.Categories (
    CategoryId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(80) NOT NULL,
    Slug NVARCHAR(80) NOT NULL,
    Icon NVARCHAR(20) NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE TABLE dbo.MenuItems (
    MenuItemId INT IDENTITY PRIMARY KEY,
    CategoryId INT NOT NULL REFERENCES dbo.Categories(CategoryId),
    Name NVARCHAR(160) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    Price DECIMAL(10,2) NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    IsFeatured BIT NOT NULL DEFAULT 0,
    IsAvailable BIT NOT NULL DEFAULT 1,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedDate DATETIME2 NULL
);

CREATE TABLE dbo.Orders (
    OrderId INT IDENTITY PRIMARY KEY,
    OrderNumber NVARCHAR(30) NOT NULL UNIQUE,
    UserId INT NULL REFERENCES dbo.Users(UserId),
    CustomerName NVARCHAR(120) NOT NULL,
    CustomerEmail NVARCHAR(200) NOT NULL,
    CustomerPhone NVARCHAR(30) NOT NULL,
    DeliveryAddress NVARCHAR(400) NOT NULL,
    Subtotal DECIMAL(10,2) NOT NULL,
    DeliveryCharges DECIMAL(10,2) NOT NULL,
    Discount DECIMAL(10,2) NOT NULL,
    Tax DECIMAL(10,2) NOT NULL,
    TotalAmount DECIMAL(10,2) NOT NULL,
    OrderStatus NVARCHAR(40) NOT NULL,
    PaymentStatus NVARCHAR(40) NOT NULL,
    RazorpayOrderId NVARCHAR(80) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ModifiedDate DATETIME2 NULL
);

CREATE TABLE dbo.OrderItems (
    OrderItemId INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL REFERENCES dbo.Orders(OrderId),
    MenuItemId INT NOT NULL REFERENCES dbo.MenuItems(MenuItemId),
    Quantity INT NOT NULL,
    UnitPrice DECIMAL(10,2) NOT NULL,
    TotalPrice DECIMAL(10,2) NOT NULL,
    Size NVARCHAR(20) NULL
);

CREATE TABLE dbo.Payments (
    PaymentId INT IDENTITY PRIMARY KEY,
    OrderId INT NOT NULL REFERENCES dbo.Orders(OrderId),
    RazorpayOrderId NVARCHAR(80) NULL,
    RazorpayPaymentId NVARCHAR(80) NULL,
    RazorpaySignature NVARCHAR(200) NULL,
    Amount DECIMAL(10,2) NOT NULL,
    Currency NVARCHAR(10) NOT NULL DEFAULT 'INR',
    PaymentStatus NVARCHAR(40) NOT NULL,
    PaymentMethod NVARCHAR(40) NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.ReservationSlots (
    SlotId INT IDENTITY PRIMARY KEY,
    SlotTime NVARCHAR(10) NOT NULL,
    Capacity INT NOT NULL
);

CREATE TABLE dbo.Reservations (
    ReservationId INT IDENTITY PRIMARY KEY,
    UserId INT NULL REFERENCES dbo.Users(UserId),
    CustomerName NVARCHAR(120) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(30) NOT NULL,
    ReservationDate DATE NOT NULL,
    ReservationTime NVARCHAR(10) NOT NULL,
    GuestCount INT NOT NULL,
    SpecialRequest NVARCHAR(400) NULL,
    Status NVARCHAR(40) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

CREATE TABLE dbo.Favourites (
    FavouriteId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL REFERENCES dbo.Users(UserId),
    MenuItemId INT NOT NULL REFERENCES dbo.MenuItems(MenuItemId),
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Fav UNIQUE (UserId, MenuItemId)
);

CREATE TABLE dbo.Addresses (
    AddressId INT IDENTITY PRIMARY KEY,
    UserId INT NOT NULL REFERENCES dbo.Users(UserId),
    Line NVARCHAR(400) NOT NULL,
    IsDefault BIT NOT NULL DEFAULT 0
);

CREATE TABLE dbo.Gallery (
    GalleryId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(120) NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL,
    Caption NVARCHAR(300) NULL,
    SortOrder INT NOT NULL DEFAULT 0
);

CREATE TABLE dbo.Events (
    EventId INT IDENTITY PRIMARY KEY,
    Title NVARCHAR(160) NOT NULL,
    Description NVARCHAR(500) NOT NULL,
    EventDate DATE NOT NULL,
    ImageUrl NVARCHAR(500) NOT NULL
);

CREATE TABLE dbo.ContactMessages (
    ContactMessageId INT IDENTITY PRIMARY KEY,
    Name NVARCHAR(120) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    Phone NVARCHAR(30) NULL,
    Message NVARCHAR(1000) NOT NULL,
    CreatedDate DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
GO
