-- ==================================================================================
-- SCRIPT SỬA MÃ ĐƠN HÀNG LIÊN TỤC (1, 2, 3, 4...) & TẮT NHẢY SỐ TRÊN SQL SERVER
-- ==================================================================================
USE BeautyCosmeticDB;
GO

PRINT N'1. Đang tắt tính năng nhảy 1000 số của SQL Server (IDENTITY_CACHE = OFF)...';
ALTER DATABASE SCOPED CONFIGURATION SET IDENTITY_CACHE = OFF;
GO

PRINT N'2. Tạm xóa khóa ngoại FK_OrderDetails_Orders...';
IF OBJECT_ID('dbo.FK_OrderDetails_Orders', 'F') IS NOT NULL
    ALTER TABLE [dbo].[OrderDetails] DROP CONSTRAINT [FK_OrderDetails_Orders];
GO

PRINT N'3. Tạo bảng ánh xạ mã đơn cũ sang mã đơn mới tuần tự 1, 2, 3, 4, 5...';
IF OBJECT_ID('tempdb..#OrderMap') IS NOT NULL DROP TABLE #OrderMap;
SELECT 
    OrderId AS OldOrderId, 
    ROW_NUMBER() OVER (ORDER BY OrderDate ASC, OrderId ASC) AS NewOrderId
INTO #OrderMap
FROM Orders;

PRINT N'4. Sao lưu dữ liệu đơn hàng với mã mới...';
IF OBJECT_ID('tempdb..#NewOrders') IS NOT NULL DROP TABLE #NewOrders;
SELECT 
    m.NewOrderId AS OrderId,
    o.UserId,
    o.OrderDate,
    o.TotalAmount,
    o.OrderStatus,
    o.PaymentStatus,
    o.ShippingAddress,
    o.Phone,
    o.Note
INTO #NewOrders
FROM Orders o
INNER JOIN #OrderMap m ON o.OrderId = m.OldOrderId;

PRINT N'5. Cập nhật chi tiết đơn hàng (OrderDetails) theo mã mới...';
UPDATE d
SET d.OrderId = m.NewOrderId
FROM OrderDetails d
INNER JOIN #OrderMap m ON d.OrderId = m.OldOrderId;

PRINT N'6. Cập nhật bảng Orders theo mã mới tuần tự...';
DELETE FROM Orders;

SET IDENTITY_INSERT Orders ON;
INSERT INTO Orders (OrderId, UserId, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddress, Phone, Note)
SELECT OrderId, UserId, OrderDate, TotalAmount, OrderStatus, PaymentStatus, ShippingAddress, Phone, Note
FROM #NewOrders;
SET IDENTITY_INSERT Orders OFF;

PRINT N'7. Reset seed tự tăng tiếp theo cho bảng Orders...';
DECLARE @maxId INT = (SELECT ISNULL(MAX(OrderId), 0) FROM Orders);
DBCC CHECKIDENT ('Orders', RESEED, @maxId);

PRINT N'8. Khôi phục lại khóa ngoại FK_OrderDetails_Orders...';
ALTER TABLE [dbo].[OrderDetails] 
ADD CONSTRAINT FK_OrderDetails_Orders FOREIGN KEY([OrderId]) REFERENCES [dbo].[Orders] ([OrderId]);
GO

PRINT N'=== HOÀN TẤT THÀNH CÔNG! Toàn bộ đơn hàng đã được đánh số lại 1, 2, 3, 4... liên tục! ===';
GO

-- Xem lại kết quả sau khi đánh số lại
SELECT OrderId, OrderDate, TotalAmount, OrderStatus, PaymentStatus 
FROM Orders 
ORDER BY OrderId DESC;
GO
