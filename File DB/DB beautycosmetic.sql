SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Banners')
CREATE TABLE [dbo].[Banners](
	[BannerId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Image] [nvarchar](255) NOT NULL,
	[LinkUrl] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL DEFAULT ((1)),
PRIMARY KEY CLUSTERED ([BannerId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Categories')
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED ([CategoryId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SkinTypes')
CREATE TABLE [dbo].[SkinTypes](
	[SkinTypeId] [int] IDENTITY(1,1) NOT NULL,
	[SkinTypeName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED ([SkinTypeId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](254) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Phone] [varchar](15) NULL,
	[Address] [nvarchar](300) NULL,
	[Role] [varchar](20) NOT NULL DEFAULT ('Customer'),
	[CreatedAt] [datetime] NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED ([UserId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Products')
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Brand] [nvarchar](100) NULL,
	[Image] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Recommend] [bit] NOT NULL DEFAULT ((0)),
	[Rating] [decimal](3, 2) NOT NULL DEFAULT ((0)),
	[ViewQuantity] [int] NOT NULL DEFAULT ((0)),
	[CreatedAt] [datetime] NOT NULL DEFAULT (getdate()),
PRIMARY KEY CLUSTERED ([ProductId] ASC)
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductDetails')
CREATE TABLE [dbo].[ProductDetails](
	[ProductDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[DetailName] [nvarchar](100) NOT NULL DEFAULT (N'Mặc định'),
	[Price] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL DEFAULT ((0)),
	[SoldQuantity] [int] NOT NULL DEFAULT ((0)),
	[DiscountedPrice] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED ([ProductDetailId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Orders')
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL DEFAULT (getdate()),
	[TotalAmount] [decimal](18, 2) NOT NULL DEFAULT ((0)),
	[OrderStatus] [varchar](20) NOT NULL DEFAULT ('Pending'),
	[PaymentMethod] [nvarchar](30) NULL,
	[PaymentStatus] [varchar](20) NOT NULL DEFAULT ('Unpaid'),
	[ShippingAddress] [nvarchar](300) NOT NULL,
PRIMARY KEY CLUSTERED ([OrderId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderDetails')
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductDetailId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[LineTotal] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED ([OrderDetailId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ProductSkinTypes')
CREATE TABLE [dbo].[ProductSkinTypes](
	[ProductId] [int] NOT NULL,
	[SkinTypeId] [int] NOT NULL,
PRIMARY KEY CLUSTERED ([ProductId] ASC, [SkinTypeId] ASC)
) ON [PRIMARY]
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderDetails_OrderId')
CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderId] ON [dbo].[OrderDetails] ([OrderId] ASC)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OrderDetails_ProductDetailId')
CREATE NONCLUSTERED INDEX [IX_OrderDetails_ProductDetailId] ON [dbo].[OrderDetails] ([ProductDetailId] ASC)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Orders_UserId_OrderDate')
CREATE NONCLUSTERED INDEX [IX_Orders_UserId_OrderDate] ON [dbo].[Orders] ([UserId] ASC, [OrderDate] ASC)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductDetails_ProductId')
CREATE NONCLUSTERED INDEX [IX_ProductDetails_ProductId] ON [dbo].[ProductDetails] ([ProductId] ASC)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Products_CategoryId')
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products] ([CategoryId] ASC)
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ProductSkinTypes_SkinTypeId')
CREATE NONCLUSTERED INDEX [IX_ProductSkinTypes_SkinTypeId] ON [dbo].[ProductSkinTypes] ([SkinTypeId] ASC)
GO

-- FOREIGN KEYS
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderDetails_Orders')
ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY([OrderId]) REFERENCES [dbo].[Orders] ([OrderId])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_OrderDetails_ProductDetails')
ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT FK_OrderDetails_ProductDetails FOREIGN KEY([ProductDetailId]) REFERENCES [dbo].[ProductDetails] ([ProductDetailId])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Orders_Users')
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT FK_Orders_Users FOREIGN KEY([UserId]) REFERENCES [dbo].[Users] ([UserId])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductDetails_Products')
ALTER TABLE [dbo].[ProductDetails] ADD CONSTRAINT FK_ProductDetails_Products FOREIGN KEY([ProductId]) REFERENCES [dbo].[Products] ([ProductId])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Products_Categories')
ALTER TABLE [dbo].[Products] ADD CONSTRAINT FK_Products_Categories FOREIGN KEY([CategoryId]) REFERENCES [dbo].[Categories] ([CategoryId])
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductSkinTypes_Products')
ALTER TABLE [dbo].[ProductSkinTypes] ADD CONSTRAINT FK_ProductSkinTypes_Products FOREIGN KEY([ProductId]) REFERENCES [dbo].[Products] ([ProductId]) ON DELETE NO ACTION
GO

IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ProductSkinTypes_SkinTypes')
ALTER TABLE [dbo].[ProductSkinTypes] ADD CONSTRAINT FK_ProductSkinTypes_SkinTypes FOREIGN KEY([SkinTypeId]) REFERENCES [dbo].[SkinTypes] ([SkinTypeId]) ON DELETE NO ACTION
GO

IF NOT EXISTS (SELECT 1 FROM SkinTypes WHERE SkinTypeId = 1)
    SET IDENTITY_INSERT [dbo].[SkinTypes] ON;
IF NOT EXISTS (SELECT 1 FROM SkinTypes WHERE SkinTypeId = 1)
    INSERT INTO [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (1, N'Da thường');
IF NOT EXISTS (SELECT 1 FROM SkinTypes WHERE SkinTypeId = 2)
    INSERT INTO [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (2, N'Da dầu');
IF NOT EXISTS (SELECT 1 FROM SkinTypes WHERE SkinTypeId = 3)
    INSERT INTO [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (3, N'Da khô');
IF NOT EXISTS (SELECT 1 FROM SkinTypes WHERE SkinTypeId = 4)
    INSERT INTO [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (4, N'Da hỗn hợp');
IF NOT EXISTS (SELECT 1 FROM SkinTypes WHERE SkinTypeId = 5)
    INSERT INTO [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (5, N'Da nhạy cảm');
SET IDENTITY_INSERT [dbo].[SkinTypes] OFF;
GO


IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Chăm sóc da')
    INSERT INTO Categories (CategoryName, Description) VALUES (N'Chăm sóc da', N'Làm sạch và dưỡng da');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Trang điểm')
    INSERT INTO Categories (CategoryName, Description) VALUES (N'Trang điểm', N'Son môi và trang điểm');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Chống nắng')
    INSERT INTO Categories (CategoryName, Description) VALUES (N'Chống nắng', N'Bảo vệ da chống nắng');

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = N'Dưỡng ẩm')
    INSERT INTO Categories (CategoryName, Description) VALUES (N'Dưỡng ẩm', N'Cấp ẩm và phục hồi');
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'zinkaito1412@gmail.com')
BEGIN
    SET IDENTITY_INSERT [dbo].[Users] ON;
    INSERT INTO [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [Phone], [Address], [Role], [CreatedAt])
    VALUES (1, N'zin', N'zinkaito1412@gmail.com', N'100000.MRk7V4K6F02xZW8CBcWnvg==./QN0RYvsL8QsMeykmrT3psB/NXANQ3XsYYvdTvgzGGQ=', NULL, NULL, N'Admin', CAST(N'2026-10-27T11:42:00.783' AS DateTime));
    SET IDENTITY_INSERT [dbo].[Users] OFF;
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Email = N'zinnnn312@gmail.com')
BEGIN
    SET IDENTITY_INSERT [dbo].[Users] ON;
    INSERT INTO [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [Phone], [Address], [Role], [CreatedAt])
    VALUES (2, N'zink', N'zinnnn312@gmail.com', N'CiEoH7IpVApBLr/RoV8DrQ==:aAi1zFORommw5EcT5cP6T19JSf9UP1sPad5IbtvJVoA=', NULL, NULL, N'Customer', CAST(N'2026-10-28T07:35:34.890' AS DateTime));
    SET IDENTITY_INSERT [dbo].[Users] OFF;
END
GO

DELETE FROM [dbo].[ProductSkinTypes];
DELETE FROM [dbo].[OrderDetails];
DELETE FROM [dbo].[ProductDetails];
DELETE FROM [dbo].[Products];
GO

SET IDENTITY_INSERT [dbo].[Products] ON;
INSERT INTO [dbo].[Products] ([ProductId], [CategoryId], [Name], [Brand], [Image], [Description], [Recommend], [Rating], [ViewQuantity], [CreatedAt])
VALUES
(1, 1, N'Sữa rửa mặt dịu nhẹ',    N'CeraVe',          N'ceraVe.jpg',       N'Sữa rửa mặt CeraVe',              1, 4.8, 100, GETDATE()),
(2, 1, N'Toner cấp ẩm',            N'Klairs',           N'klairs.jpg',       N'Toner Klairs',                     1, 4.7, 100, GETDATE()),
(3, 1, N'Serum Vitamin C',          N'Melano CC',        N'melano.jpg',       N'Serum Melano CC',                  1, 4.9, 100, GETDATE()),
(4, 1, N'Kem dưỡng ẩm phục hồi',   N'La Roche-Posay',  N'larocheposay.jpg', N'Kem dưỡng La Roche-Posay',         1, 4.8, 100, GETDATE()),
(5, 1, N'Kem chống nắng nâng tông', N'Anessa',           N'anessa.jpg',       N'Kem chống nắng Anessa',            1, 5.0, 100, GETDATE()),
(6, 1, N'Son môi cao cấp',          N'YSL',              N'ysl.jpg',          N'Son YSL',                          1, 5.0, 100, GETDATE());
SET IDENTITY_INSERT [dbo].[Products] OFF;
GO

SET IDENTITY_INSERT [dbo].[ProductDetails] ON;
INSERT INTO [dbo].[ProductDetails] ([ProductDetailId], [ProductId], [DetailName], [Price], [Quantity], [SoldQuantity], [DiscountedPrice])
VALUES
(1, 1, N'Mặc định', 250000, 50, 0, NULL),
(2, 2, N'Mặc định', 320000, 50, 0, NULL),
(3, 3, N'Mặc định', 350000, 50, 0, NULL),
(4, 4, N'Mặc định', 450000, 50, 0, NULL),
(5, 5, N'Mặc định', 520000, 50, 0, NULL),
(6, 6, N'Mặc định', 950000, 50, 0, NULL);
SET IDENTITY_INSERT [dbo].[ProductDetails] OFF;
GO

INSERT INTO [dbo].[ProductSkinTypes] ([ProductId], [SkinTypeId]) VALUES
(1,1),(1,2),(2,1),(2,3),(3,1),(3,2),(4,1),(4,3),(5,1),(5,2),(6,1),(6,2);
GO

UPDATE Products SET CategoryId = (SELECT TOP 1 CategoryId FROM Categories WHERE CategoryName = N'Chăm sóc da')
WHERE Brand IN (N'CeraVe', N'Klairs', N'Melano CC');

UPDATE Products SET CategoryId = (SELECT TOP 1 CategoryId FROM Categories WHERE CategoryName = N'Trang điểm')
WHERE Brand = N'YSL';

UPDATE Products SET CategoryId = (SELECT TOP 1 CategoryId FROM Categories WHERE CategoryName = N'Chống nắng')
WHERE Brand = N'Anessa';

UPDATE Products SET CategoryId = (SELECT TOP 1 CategoryId FROM Categories WHERE CategoryName = N'Dưỡng ẩm')
WHERE Brand = N'La Roche-Posay';
GO

UPDATE [dbo].[Users] SET [Role] = 'Admin' WHERE [UserId] = 1;
GO

PRINT 'BeautyCosmeticDB setup hoàn tất!';
GO

GO

-- ==================================================================================
-- SCRIPT TẠO BẢNG VOUCHERS (MÃ GIẢM GIÁ / KHUYẾN MÃI) CHO BEAUTYCOSMETIC
-- ==================================================================================
-- 1. Tạo bảng Vouchers nếu chưa tồn tại
IF OBJECT_ID('dbo.Vouchers', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Vouchers] (
        [VoucherId] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Code] NVARCHAR(50) NOT NULL UNIQUE,
        [Title] NVARCHAR(200) NOT NULL,
        [DiscountType] NVARCHAR(20) NOT NULL, -- 'Percent' hoặc 'FixedAmount'
        [DiscountValue] DECIMAL(18,2) NOT NULL,
        [MinOrderAmount] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [MaxDiscountAmount] DECIMAL(18,2) NULL,
        [Quantity] INT NOT NULL DEFAULT 100,
        [StartDate] DATETIME NOT NULL DEFAULT GETDATE(),
        [EndDate] DATETIME NOT NULL,
        [IsActive] BIT NOT NULL DEFAULT 1,
        [CreatedAt] DATETIME NOT NULL DEFAULT GETDATE()
    );
    PRINT N'Đã tạo bảng Vouchers thành công!';
END
ELSE
BEGIN
    PRINT N'Bảng Vouchers đã tồn tại.';
END
GO

-- 2. Chèn các mã Voucher mẫu hấp dẫn
IF NOT EXISTS (SELECT 1 FROM Vouchers WHERE Code = 'CHAOBANMOI')
BEGIN
    INSERT INTO Vouchers (Code, Title, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, Quantity, StartDate, EndDate, IsActive)
    VALUES ('CHAOBANMOI', N'Giảm 15% cho khách hàng mới', 'Percent', 15.00, 150000.00, 50000.00, 100, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 1);
END

IF NOT EXISTS (SELECT 1 FROM Vouchers WHERE Code = 'BEAUTY50K')
BEGIN
    INSERT INTO Vouchers (Code, Title, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, Quantity, StartDate, EndDate, IsActive)
    VALUES ('BEAUTY50K', N'Giảm ngay 50.000đ cho đơn từ 300.000đ', 'FixedAmount', 50000.00, 300000.00, NULL, 50, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 1);
END

IF NOT EXISTS (SELECT 1 FROM Vouchers WHERE Code = 'FREESHIP')
BEGIN
    INSERT INTO Vouchers (Code, Title, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, Quantity, StartDate, EndDate, IsActive)
    VALUES ('FREESHIP', N'Miễn phí vận chuyển (Giảm 30.000đ)', 'FixedAmount', 30000.00, 200000.00, NULL, 200, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 1);
END

IF NOT EXISTS (SELECT 1 FROM Vouchers WHERE Code = 'VIP100K')
BEGIN
    INSERT INTO Vouchers (Code, Title, DiscountType, DiscountValue, MinOrderAmount, MaxDiscountAmount, Quantity, StartDate, EndDate, IsActive)
    VALUES ('VIP100K', N'Ưu đãi VIP giảm 100.000đ cho đơn từ 600.000đ', 'FixedAmount', 100000.00, 600000.00, NULL, 30, GETDATE(), DATEADD(MONTH, 3, GETDATE()), 1);
END
GO

PRINT N'=== HOÀN TẤT CÀI ĐẶT BẢNG VOUCHERS & MÃ MẪU! ===';
SELECT * FROM Vouchers;
GO
UPDATE Users
SET PasswordHash = N'4RhH5papxsx/UoBsNSjinA==:Z6PoGV+wIghrDwdf7GMzwhd2FXtnIT1p4YwPt8mP1E0=',
    Role = 'Admin'
WHERE Email = N'admin@gmail.com';
GO