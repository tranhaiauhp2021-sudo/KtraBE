-- KTRA BE: Tao Database RIENG de KHONG ghi de du lieu Bakery Store / MyStore cu.
-- Run this whole file in SQL Server Object Explorer > New Query (LocalDB MSSQLLocalDB).
-- Can run twice safely: seed only when no data.
IF DB_ID(N'MyStore_BE_Ktra') IS NULL CREATE DATABASE [MyStore_BE_Ktra];
GO
USE [MyStore_BE_Ktra];
GO

IF OBJECT_ID(N'dbo.[User]', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.[User] (
        Username NVARCHAR(255) NOT NULL PRIMARY KEY,
        [Password] NVARCHAR(255) NOT NULL,
        UserRole NCHAR(1) NOT NULL
    );
END;
GO
IF OBJECT_ID(N'dbo.Customer', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Customer (
        CustomerID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CustomerName NVARCHAR(MAX) NOT NULL,
        CustomerPhone NVARCHAR(15) NOT NULL,
        CustomerEmail NVARCHAR(MAX) NULL,
        CustomerAddress NVARCHAR(MAX) NULL,
        Username NVARCHAR(255) NOT NULL,
        CONSTRAINT FK_User_Customer FOREIGN KEY (Username) REFERENCES dbo.[User] (Username)
    );
END;
GO
IF OBJECT_ID(N'dbo.Category', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Category (
        CategoryID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CategoryName NVARCHAR(MAX) NOT NULL
    );
END;
GO
IF OBJECT_ID(N'dbo.Product', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Product (
        ProductID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CategoryID INT NOT NULL,
        ProductName NVARCHAR(MAX) NOT NULL,
        ProductDecription NVARCHAR(MAX) NOT NULL,
        ProductPrice DECIMAL(18,2) NOT NULL,
        ProductImage NVARCHAR(MAX) NULL,
        CONSTRAINT FK_Pro_Category FOREIGN KEY (CategoryID) REFERENCES dbo.Category (CategoryID)
    );
END;
GO
IF OBJECT_ID(N'dbo.[Order]', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.[Order] (
        OrderID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        CustomerID INT NOT NULL,
        OrderDate DATE NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL,
        PaymentStatus NVARCHAR(MAX) NULL,
        AddressDelivery NVARCHAR(MAX) NOT NULL,
        OrderStatus NVARCHAR(40) NULL,
        PaymentMethod NVARCHAR(60) NULL,
        DeliveryMethod NVARCHAR(60) NULL,
        CONSTRAINT FK_Order_Customer FOREIGN KEY (CustomerID) REFERENCES dbo.Customer (CustomerID)
    );
END;
GO
IF OBJECT_ID(N'dbo.OrderDetail', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.OrderDetail (
        ID INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ProductID INT NOT NULL,
        OrderID INT NOT NULL,
        Quantity INT NOT NULL CONSTRAINT CK_OrderDetail_Quantity CHECK (Quantity > 0),
        UnitPrice DECIMAL(18,2) NOT NULL,
        TotalPrice AS (Quantity * UnitPrice) PERSISTED,
        CONSTRAINT FK_OrderDetail_Product FOREIGN KEY (ProductID) REFERENCES dbo.Product (ProductID),
        CONSTRAINT FK_OrderDetail_Order FOREIGN KEY (OrderID) REFERENCES dbo.[Order] (OrderID)
    );
END;
GO

-- Upgrade safely if an earlier exam database was created without these fields.
IF COL_LENGTH('dbo.[Order]', 'OrderStatus') IS NULL ALTER TABLE dbo.[Order] ADD OrderStatus NVARCHAR(40) NULL;
IF COL_LENGTH('dbo.[Order]', 'PaymentMethod') IS NULL ALTER TABLE dbo.[Order] ADD PaymentMethod NVARCHAR(60) NULL;
IF COL_LENGTH('dbo.[Order]', 'DeliveryMethod') IS NULL ALTER TABLE dbo.[Order] ADD DeliveryMethod NVARCHAR(60) NULL;
IF COL_LENGTH('dbo.OrderDetail', 'TotalPrice') IS NULL ALTER TABLE dbo.OrderDetail ADD TotalPrice AS (Quantity * UnitPrice) PERSISTED;
GO

-- Tai khoan Admin mau (mat khau PBKDF2, khong luu plaintext trong bang User).
IF NOT EXISTS (SELECT 1 FROM dbo.[User] WHERE Username = N'admin')
    INSERT INTO dbo.[User] (Username, [Password], UserRole)
    VALUES (N'admin', N'PBKDF2$100000$peEtzZBscZiJtbELZUzUvA==$7c8mYOKpFElMaEO55/7HAX17hj0BUtncCwNGBs86kCE=', N'A');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Customer)
BEGIN
    INSERT INTO dbo.[User] (Username, [Password], UserRole)
    VALUES
    (N'khach01', N'NOLOGIN', N'C'), (N'khach02', N'NOLOGIN', N'C'),
    (N'khach03', N'NOLOGIN', N'C'), (N'khach04', N'NOLOGIN', N'C'),
    (N'khach05', N'NOLOGIN', N'C'), (N'khach06', N'NOLOGIN', N'C'),
    (N'khach07', N'NOLOGIN', N'C'), (N'khach08', N'NOLOGIN', N'C');
    INSERT INTO dbo.Customer (CustomerName,CustomerPhone,CustomerEmail,CustomerAddress,Username)
    VALUES
    (N'Nguyễn Thị Minh Anh',N'0901000001',N'minhanh@example.com',N'Quận 1, TP.HCM',N'khach01'),
    (N'Trần Quốc Bảo',N'0901000002',N'bao@example.com',N'Quận 3, TP.HCM',N'khach02'),
    (N'Lê Hoàng Phúc',N'0901000003',N'phuc@example.com',N'Thủ Đức, TP.HCM',N'khach03'),
    (N'Phạm Ngọc Hà',N'0901000004',N'ha@example.com',N'Quận 7, TP.HCM',N'khach04'),
    (N'Võ Thanh Tùng',N'0901000005',N'tung@example.com',N'Bình Thạnh, TP.HCM',N'khach05'),
    (N'Huỳnh Bảo Trâm',N'0901000006',N'tram@example.com',N'Gò Vấp, TP.HCM',N'khach06'),
    (N'Đặng Minh Khoa',N'0901000007',N'khoa@example.com',N'Tân Bình, TP.HCM',N'khach07'),
    (N'Ngô Mỹ Duyên',N'0901000008',N'duyen@example.com',N'Quận 10, TP.HCM',N'khach08');
END;
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Category)
    INSERT INTO dbo.Category(CategoryName) VALUES (N'Sách'),(N'Văn phòng phẩm'),(N'Phụ kiện');
GO
IF NOT EXISTS (SELECT 1 FROM dbo.Product)
BEGIN
    INSERT INTO dbo.Product(CategoryID, ProductName, ProductDecription, ProductPrice, ProductImage)
    VALUES
    (1,N'Nghệ thuật tư duy',N'Sách phát triển kỹ năng tư duy',99000,NULL),
    (1,N'Hành trình khám phá',N'Sách văn học kỹ năng',129000,NULL),
    (1,N'Tư duy thiết kế',N'Sách sáng tạo và quản trị',115000,NULL),
    (2,N'Sổ tay A5',N'Sổ tay ghi chép',39000,NULL),
    (2,N'Bút ký cao cấp',N'Bút viết mực xanh',59000,NULL),
    (3,N'Túi đựng laptop',N'Túi vải chống sốc',249000,NULL),
    (3,N'Bình nước giữ nhiệt',N'Bình thép 500ml',179000,NULL),
    (2,N'Bộ sticker sáng tạo',N'Sticker trang trí',25000,NULL);
END;
GO

-- 31 don hang mau de test TIM KIEM, xem CHI TIET va PHAN TRANG 15/trang.
IF NOT EXISTS (SELECT 1 FROM dbo.[Order])
BEGIN
    DECLARE @i INT = 1, @orderID INT, @cid INT, @p1 INT, @p2 INT, @quantity1 INT, @quantity2 INT;
    DECLARE @cost1 DECIMAL(18,2), @cost2 DECIMAL(18,2);
    WHILE @i <= 31
    BEGIN
        SET @cid = ((@i - 1) % 8) + 1;
        SET @p1 = ((@i - 1) % 8) + 1;
        SET @p2 = (@i % 8) + 1;
        SET @quantity1 = (@i % 3) + 1;
        SET @quantity2 = (@i % 2) + 1;
        SELECT @cost1 = ProductPrice FROM dbo.Product WHERE ProductID = @p1;
        SELECT @cost2 = ProductPrice FROM dbo.Product WHERE ProductID = @p2;

        INSERT INTO dbo.[Order](CustomerID,OrderDate,TotalAmount,PaymentStatus,AddressDelivery,OrderStatus,PaymentMethod,DeliveryMethod)
        VALUES (@cid, DATEADD(day, -(@i % 24), CONVERT(date,GETDATE())), 0,
            CASE WHEN @i % 7 = 0 THEN N'Đã hoàn tiền'
                 WHEN @i % 3 = 0 THEN N'Chưa thanh toán' ELSE N'Đã thanh toán' END,
            N'TP. Hồ Chí Minh, Việt Nam',
            CASE WHEN @i % 6 = 0 THEN N'Đã hủy'
                 WHEN @i % 4 = 0 THEN N'Đã giao'
                 WHEN @i % 3 = 0 THEN N'Đang giao' ELSE N'Chờ xử lý' END,
            CASE WHEN @i % 2 = 0 THEN N'Chuyển khoản' ELSE N'Tiền mặt' END,
            CASE WHEN @i % 2 = 0 THEN N'Giao nhanh' ELSE N'Giao tiêu chuẩn' END);
        SET @orderID = CONVERT(INT, SCOPE_IDENTITY());
        INSERT INTO dbo.OrderDetail(ProductID,OrderID,Quantity,UnitPrice)
        VALUES (@p1,@orderID,@quantity1,@cost1),(@p2,@orderID,@quantity2,@cost2);
        UPDATE dbo.[Order] SET TotalAmount = @cost1 * @quantity1 + @cost2 * @quantity2 WHERE OrderID = @orderID;
        SET @i = @i + 1;
    END;
END;
GO
SELECT COUNT(*) AS TotalOrders FROM dbo.[Order];
SELECT TOP 5 o.OrderID,c.CustomerName,o.OrderDate,o.TotalAmount,o.PaymentStatus,o.DeliveryMethod
FROM dbo.[Order] o INNER JOIN dbo.Customer c ON o.CustomerID = c.CustomerID
ORDER BY o.OrderID DESC;
