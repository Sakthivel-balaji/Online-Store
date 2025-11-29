--Customers
INSERT INTO Customers (ProfilePicture, FullName, CreatedAt, IsDeleted, Phone, Email, PasswordHash, Role, UserName)
VALUES 
(NULL, 'Alice Johnson', GETDATE(), 0, '9876543210', 'alice@example.com', 'hash1', 'User', 'alicej'),
(NULL, 'Bob Smith', GETDATE(), 0, '9123456789', 'bob@example.com', 'hash2', 'Admin', 'bobsmith'),
(NULL, 'Carol Lee', GETDATE(), 0, '9988776655', 'carol@example.com', 'hash3', 'User', 'caroll'),
(NULL, 'David Kim', GETDATE(), 0, '9776655443', 'david@example.com', 'hash4', 'User', 'davidk'),
(NULL, 'Eva Green', GETDATE(), 0, '9665544332', 'eva@example.com', 'hash5', 'Seller', 'evag');


--Products
INSERT INTO Products (Name, Description, Category, Brand, Price, Discount, StockQuantity, Image, Weight, Length, Breadth, Height, IsFeatured, IsPopular, CreatedAt, IsDeleted)
VALUES 
('Laptop', 'Gaming Laptop', 'Electronics', 'Dell', 85000, 10, 50, NULL, 2.5, 35, 25, 3, 1, 1, GETDATE(), 0),
('Phone', 'Smartphone with OLED', 'Electronics', 'Samsung', 45000, 5, 100, NULL, 0.3, 15, 7, 1, 1, 1, GETDATE(), 0),
('Shoes', 'Running shoes', 'Footwear', 'Nike', 5500, 15, 200, NULL, 1.2, 30, 10, 12, 0, 1, GETDATE(), 0),
('Backpack', 'Travel backpack', 'Accessories', 'Wildcraft', 2500, 0, 80, NULL, 0.8, 40, 30, 15, 0, 0, GETDATE(), 0),
('Watch', 'Smartwatch', 'Wearables', 'Apple', 32000, 8, 40, NULL, 0.2, 4, 4, 1, 1, 0, GETDATE(), 0);


--CartItems
INSERT INTO CartItems (CustomerId, ProductId, Quantity, IsDeleted)
VALUES 
(1, 2, 1, 0),
(2, 3, 2, 0),
(3, 1, 1, 0),
(4, 4, 1, 0),
(5, 5, 1, 0);


--DeliveryAddresses
INSERT INTO DeliveryAddresses (CustomerId, Address, City, State, Country, PostalCode, Phone, CreatedAt, IsDeleted, IsPrimary)
VALUES 
(1, '12 Park Street', 'Chennai', 'TN', 'India', '600001', '9876543210', GETDATE(), 0, 1),
(2, '45 Lake View', 'Bangalore', 'KA', 'India', '560002', '9123456789', GETDATE(), 0, 1),
(3, '78 Hill Road', 'Hyderabad', 'TS', 'India', '500003', '9988776655', GETDATE(), 0, 1),
(4, '33 MG Road', 'Pune', 'MH', 'India', '411004', '9776655443', GETDATE(), 0, 1),
(5, '90 Nehru Street', 'Delhi', 'DL', 'India', '110005', '9665544332', GETDATE(), 0, 1);


--Orders
INSERT INTO Orders (CustomerId, OrderDate, DeliveryDate, [Status], IsDeleted, AddressId, TotalPrice)
VALUES 
(1, GETDATE(), GETDATE()+5, 'Placed', 0, 1, 85000),
(2, GETDATE(), GETDATE()+3, 'Shipped', 0, 2, 5500),
(3, GETDATE(), GETDATE()+7, 'Delivered', 0, 3, 45000),
(4, GETDATE(), GETDATE()+2, 'Cancelled', 0, 4, 2500),
(5, GETDATE(), GETDATE()+4, 'Placed', 0, 5, 32000);


--OrderItems
INSERT INTO OrderItems (OrderId, ProductId, Quantity, UnitPrice, IsDeleted)
VALUES 
(1, 1, 1, 85000, 0),
(2, 3, 1, 5500, 0),
(3, 2, 1, 45000, 0),
(4, 4, 1, 2500, 0),
(5, 5, 1, 32000, 0);


--Reviews
INSERT INTO Reviews (CustomerId, ProductId, Rating, Comment, CreatedAt, IsDeleted)
VALUES 
(1, 1, 5, 'Great performance!', GETDATE(), 0),
(2, 3, 4, 'Comfortable shoes', GETDATE(), 0),
(3, 2, 5, 'Excellent display', GETDATE(), 0),
(4, 4, 3, 'Average quality', GETDATE(), 0),
(5, 5, 5, 'Love the features!', GETDATE(), 0);


--Wishlist
INSERT INTO Wishlist (CustomerId, ProductId, IsDeleted)
VALUES 
(1, 5, 0),
(2, 4, 0),
(3, 2, 0),
(4, 1, 0),
(5, 3, 0);
 