USE [OnlineStoreDB]
GO


/****** Object:  Table [dbo].[CartItems]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CartItems](
	[CartItemId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[CartItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO






/****** Object:  Table [dbo].[Customers]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[ProfilePicture] [nvarchar](max) NULL,
	[FullName] [nvarchar](100) NULL,
	[CreatedAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[Phone] [nvarchar](50) NULL,
	[Email] [nvarchar](256) NOT NULL,
	[PasswordHash] [nvarchar](max) NOT NULL,
	[Role] [nvarchar](50) NOT NULL,
	[UserName] [nvarchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Customers_Email] UNIQUE NONCLUSTERED 
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
 CONSTRAINT [UQ_Customers_UserName] UNIQUE NONCLUSTERED 
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO






/****** Object:  Table [dbo].[DeliveryAddresses]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DeliveryAddresses](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[Address] [nvarchar](200) NOT NULL,
	[City] [nvarchar](50) NOT NULL,
	[State] [nvarchar](50) NOT NULL,
	[Country] [nvarchar](50) NOT NULL,
	[PostalCode] [nvarchar](20) NOT NULL,
	[Phone] [nvarchar](20) NOT NULL,
	[CreatedAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
	[IsPrimary] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO





/****** Object:  Table [dbo].[OrderItems]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderItems](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](18, 2) NOT NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO





/****** Object:  Table [dbo].[Orders]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[OrderDate] [datetime] NULL,
	[DeliveryDate] [datetime] NULL,
	[Status] [nvarchar](20) NULL,
	[IsDeleted] [bit] NULL,
	[AddressId] [int] NULL,
	[TotalPrice] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO






/****** Object:  Table [dbo].[Products]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Category] [nvarchar](50) NULL,
	[Brand] [nvarchar](50) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[Discount] [decimal](5, 2) NULL,
	[StockQuantity] [int] NOT NULL,
	[Image] [nvarchar](max) NULL,
	[Weight] [decimal](10, 2) NULL,
	[Length] [decimal](10, 2) NULL,
	[Breadth] [decimal](10, 2) NULL,
	[Height] [decimal](10, 2) NULL,
	[IsFeatured] [bit] NULL,
	[IsPopular] [bit] NULL,
	[CreatedAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO







/****** Object:  Table [dbo].[Reviews]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Reviews](
	[ReviewId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Rating] [int] NULL,
	[Comment] [nvarchar](max) NULL,
	[CreatedAt] [datetime] NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[ReviewId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO





/****** Object:  Table [dbo].[Wishlist]    Script Date: 14-07-2025 11:35:56 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Wishlist](
	[WishlistId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[IsDeleted] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[WishlistId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO






ALTER TABLE [dbo].[CartItems] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Customers] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DeliveryAddresses] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[DeliveryAddresses] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[DeliveryAddresses] ADD  DEFAULT ((0)) FOR [IsPrimary]
GO
ALTER TABLE [dbo].[OrderItems] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ('Placed') FOR [Status]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Discount]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [IsFeatured]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [IsPopular]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Reviews] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[Wishlist] ADD  DEFAULT ((0)) FOR [IsDeleted]
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[DeliveryAddresses]  WITH CHECK ADD  CONSTRAINT [FK_DeliveryAddresses_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[DeliveryAddresses] CHECK CONSTRAINT [FK_DeliveryAddresses_Customers]
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([OrderId])
REFERENCES [dbo].[Orders] ([OrderId])
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_DeliveryAddresses] FOREIGN KEY([AddressId])
REFERENCES [dbo].[DeliveryAddresses] ([AddressId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK_Orders_DeliveryAddresses]
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[Wishlist]  WITH CHECK ADD FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
GO
ALTER TABLE [dbo].[Wishlist]  WITH CHECK ADD FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[CartItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([Quantity]>(0)))
GO
ALTER TABLE [dbo].[OrderItems]  WITH CHECK ADD CHECK  (([UnitPrice]>=(0)))
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD CHECK  (([Status]='Cancelled' OR [Status]='Delivered' OR [Status]='Shipped' OR [Status]='Placed'))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([Discount]>=(0)))
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD CHECK  (([StockQuantity]>=(0)))
GO
ALTER TABLE [dbo].[Reviews]  WITH CHECK ADD CHECK  (([Rating]>=(1) AND [Rating]<=(5)))
GO
