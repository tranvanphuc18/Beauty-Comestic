GO
USE [BeautyCosmeticDB]
GO
/****** Object:  Table [dbo].[Banners]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Banners](
	[BannerId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](200) NOT NULL,
	[Image] [nvarchar](255) NOT NULL,
	[LinkUrl] [nvarchar](500) NULL,
	[IsActive] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[BannerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Categories]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Categories](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryName] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductDetailId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[LineTotal] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[OrderDate] [datetime] NOT NULL,
	[TotalAmount] [decimal](18, 2) NOT NULL,
	[OrderStatus] [varchar](20) NOT NULL,
	[PaymentMethod] [nvarchar](30) NULL,
	[PaymentStatus] [varchar](20) NOT NULL,
	[ShippingAddress] [nvarchar](300) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductDetails]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductDetails](
	[ProductDetailId] [int] IDENTITY(1,1) NOT NULL,
	[ProductId] [int] NOT NULL,
	[DetailName] [nvarchar](100) NOT NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Quantity] [int] NOT NULL,
	[SoldQuantity] [int] NOT NULL,
	[DiscountedPrice] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Brand] [nvarchar](100) NULL,
	[Image] [nvarchar](255) NULL,
	[Description] [nvarchar](max) NULL,
	[Recommend] [bit] NOT NULL,
	[Rating] [decimal](3, 2) NOT NULL,
	[ViewQuantity] [int] NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductSkinTypes]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductSkinTypes](
	[ProductId] [int] NOT NULL,
	[SkinTypeId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC,
	[SkinTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SkinTypes]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SkinTypes](
	[SkinTypeId] [int] IDENTITY(1,1) NOT NULL,
	[SkinTypeName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SkinTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 10/28/2026 8:20:13 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [nvarchar](100) NOT NULL,
	[Email] [nvarchar](254) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[Phone] [varchar](15) NULL,
	[Address] [nvarchar](300) NULL,
	[Role] [varchar](20) NOT NULL,
	[CreatedAt] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Categories] ON 

INSERT [dbo].[Categories] ([CategoryId], [CategoryName], [Description]) VALUES (1, N'Chăm sóc da', N'Sản phẩm chăm sóc da')
SET IDENTITY_INSERT [dbo].[Categories] OFF
GO
SET IDENTITY_INSERT [dbo].[ProductDetails] ON 

INSERT [dbo].[ProductDetails] ([ProductDetailId], [ProductId], [DetailName], [Price], [Quantity], [SoldQuantity], [DiscountedPrice]) VALUES (1, 1, N'Mặc định', CAST(150000.00 AS Decimal(18, 2)), 20, 0, NULL)
SET IDENTITY_INSERT [dbo].[ProductDetails] OFF
GO
SET IDENTITY_INSERT [dbo].[Products] ON 

INSERT [dbo].[Products] ([ProductId], [CategoryId], [Name], [Brand], [Image], [Description], [Recommend], [Rating], [ViewQuantity], [CreatedAt]) VALUES (1, 1, N'Sữa rửa mặt CeraVe', N'CeraVe', N'cerave.jpg', N'Sữa rửa mặt dành cho da dầu và da hỗn hợp', 1, CAST(4.50 AS Decimal(3, 2)), 0, CAST(N'2026-10-25T13:40:20.617' AS DateTime))
SET IDENTITY_INSERT [dbo].[Products] OFF
GO
INSERT [dbo].[ProductSkinTypes] ([ProductId], [SkinTypeId]) VALUES (1, 2)
INSERT [dbo].[ProductSkinTypes] ([ProductId], [SkinTypeId]) VALUES (1, 4)
GO
SET IDENTITY_INSERT [dbo].[SkinTypes] ON 

INSERT [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (2, N'Da dầu')
INSERT [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (4, N'Da hỗn hợp')
INSERT [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (3, N'Da khô')
INSERT [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (5, N'Da nhạy cảm')
INSERT [dbo].[SkinTypes] ([SkinTypeId], [SkinTypeName]) VALUES (1, N'Da thường')
SET IDENTITY_INSERT [dbo].[SkinTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [Phone], [Address], [Role], [CreatedAt]) VALUES (1, N'zin', N'zinkaito1412@gmail.com', N'100000.MRk7V4K6F02xZW8CBcWnvg==./QN0RYvsL8QsMeykmrT3psB/NXANQ3XsYYvdTvgzGGQ=', NULL, NULL, N'Customer', CAST(N'2026-10-27T11:42:00.783' AS DateTime))
INSERT [dbo].[Users] ([UserId], [FullName], [Email], [PasswordHash], [Phone], [Address], [Role], [CreatedAt]) VALUES (2, N'zink', N'zinnnn312@gmail.com', N'CiEoH7IpVApBLr/RoV8DrQ==:aAi1zFORommw5EcT5cP6T19JSf9UP1sPad5IbtvJVoA=', NULL, NULL, N'Customer', CAST(N'2026-10-28T07:35:34.890' AS DateTime))
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Categori__8517B2E00A1DB631]    Script Date: 10/28/2026 8:20:13 AM ******/
ALTER TABLE [dbo].[Categories] ADD UNIQUE NONCLUSTERED 
(
	[CategoryName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_OrderId]    Script Date: 10/28/2026 8:20:13 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_OrderId] ON [dbo].[OrderDetails]
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrderDetails_ProductDetailId]    Script Date: 10/28/2026 8:20:13 AM ******/
CREATE NONCLUSTERED INDEX [IX_OrderDetails_ProductDetailId] ON [dbo].[OrderDetails]
(
	[ProductDetailId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Orders_UserId_OrderDate]    Script Date: 10/28/2026 8:20:13 AM ******/
CREATE NONCLUSTERED INDEX [IX_Orders_UserId_OrderDate] ON [dbo].[Orders]
(
	[UserId] ASC,
	[OrderDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__ProductD__751AE8DDFDAB3CC0]    Script Date: 10/28/2026 8:20:13 AM ******/
ALTER TABLE [dbo].[ProductDetails] ADD UNIQUE NONCLUSTERED 
(
	[ProductId] ASC,
	[DetailName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ProductDetails_ProductId]    Script Date: 10/28/2026 8:20:13 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductDetails_ProductId] ON [dbo].[ProductDetails]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CategoryId]    Script Date: 10/28/2026 8:20:13 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CategoryId] ON [dbo].[Products]
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_ProductSkinTypes_SkinTypeId]    Script Date: 10/28/2026 8:20:13 AM ******/
CREATE NONCLUSTERED INDEX [IX_ProductSkinTypes_SkinTypeId] ON [dbo].[ProductSkinTypes]
(
	[SkinTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__SkinType__3F55C9295E118DD1]    Script Date: 10/28/2026 8:20:13 AM ******/
ALTER TABLE [dbo].[SkinTypes] ADD UNIQUE NONCLUSTERED 
(
	[SkinTypeName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UQ__Users__A9D105346C9D67A6]    Script Date: 10/28/2026 8:20:13 AM ******/
ALTER TABLE [dbo].[Users] ADD UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Banners] ADD  DEFAULT ((1)) FOR [IsActive]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [TotalAmount]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ('Pending') FOR [OrderStatus]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ('Unpaid') FOR [PaymentStatus]
GO
ALTER TABLE [dbo].[ProductDetails] ADD  DEFAULT (N'Mặc định') FOR [DetailName]
GO
ALTER TABLE [dbo].[ProductDetails] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[ProductDetails] ADD  DEFAULT ((0)) FOR [SoldQuantity]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Recommend]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Rating]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [ViewQuantity]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT ('Customer') FOR [Role]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([ProductDetailId])
REFERENCES [dbo].[ProductDetails] ([ProductDetailId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[ProductDetails]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([CategoryId])
REFERENCES [dbo].[Categories] ([CategoryId])
GO
ALTER TABLE [dbo].[ProductSkinTypes]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ProductSkinTypes]  WITH CHECK ADD FOREIGN KEY([SkinTypeId])
REFERENCES [dbo].[SkinTypes] ([SkinTypeId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([LineTotal]>=(0)))
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([OrderStatus]='Cancelled' OR [OrderStatus]='Completed' OR [OrderStatus]='Shipping' OR [OrderStatus]='Confirmed' OR [OrderStatus]='Pending'))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([PaymentStatus]='Refunded' OR [PaymentStatus]='Paid' OR [PaymentStatus]='Unpaid'))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([TotalAmount]>=(0)))
GO
ALTER TABLE [dbo].[ProductDetails]  WITH CHECK ADD CHECK  (([Price]>=(0)))
GO
ALTER TABLE [dbo].[ProductDetails]  WITH CHECK ADD CHECK  (([Quantity]>=(0)))
GO
ALTER TABLE [dbo].[ProductDetails]  WITH CHECK ADD CHECK  (([SoldQuantity]>=(0)))
GO
ALTER TABLE [dbo].[ProductDetails]  WITH CHECK ADD CHECK  (([DiscountedPrice] IS NULL OR [DiscountedPrice]>=(0) AND [DiscountedPrice]<=[Price]))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([Rating]>=(0) AND [Rating]<=(5)))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([ViewQuantity]>=(0)))
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD CHECK  (([Role]='Customer' OR [Role]='Admin'))
GO
USE [master]
GO