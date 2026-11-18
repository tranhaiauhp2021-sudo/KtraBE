USE [MyStore_BE_Ktra];
GO
-- 1) Tong so don (31 neu moi tao DB)
SELECT COUNT(*) AS SoDonHang FROM dbo.[Order];
-- 2) Kiem tra 15 don trang dau
SELECT TOP 15 o.OrderID, o.OrderDate, c.CustomerName, o.PaymentStatus, o.TotalAmount
FROM dbo.[Order] o INNER JOIN dbo.Customer c ON o.CustomerID = c.CustomerID
ORDER BY o.OrderDate DESC, o.OrderID DESC;
-- 3) Tong tien hoa don co khop voi cac dong hang khong?
SELECT o.OrderID, o.TotalAmount AS TongDon,
       SUM(d.Quantity * d.UnitPrice) AS TongChiTiet
FROM dbo.[Order] o JOIN dbo.OrderDetail d ON o.OrderID = d.OrderID
GROUP BY o.OrderID, o.TotalAmount
HAVING o.TotalAmount <> SUM(d.Quantity * d.UnitPrice);
-- Query 3 can tra 0 dong.
