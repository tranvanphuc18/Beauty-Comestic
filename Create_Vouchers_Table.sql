-- ==================================================================================
-- SCRIPT TẠO BẢNG VOUCHERS (MÃ GIẢM GIÁ / KHUYẾN MÃI) CHO BEAUTYCOSMETIC
-- ==================================================================================
USE BeautyCosmeticDB;
GO

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
