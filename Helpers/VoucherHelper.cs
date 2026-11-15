using System;
using System.Linq;
using nv3.Models;

namespace nv3.Helpers
{
    public class VoucherResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; }
        public Voucher Voucher { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalTotal { get; set; }
    }

    public static class VoucherHelper
    {
        public static VoucherResult ValidateAndCalculate(BeautyCosmeticDBEntities db, string code, decimal orderTotal)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return new VoucherResult { IsValid = false, Message = "Vui lòng nhập mã giảm giá.", FinalTotal = orderTotal };
            }

            string cleanCode = code.Trim().ToUpper();
            Voucher voucher = null;
            try
            {
                voucher = db.Vouchers.FirstOrDefault(v => v.Code == cleanCode);
            }
            catch (Exception)
            {
                return new VoucherResult
                {
                    IsValid = false,
                    Message = "Chưa tạo bảng Vouchers trong CSDL! Hãy mở và chạy file Create_Vouchers_Table.sql.",
                    FinalTotal = orderTotal
                };
            }

            if (voucher == null)
            {
                return new VoucherResult { IsValid = false, Message = "Mã voucher không tồn tại.", FinalTotal = orderTotal };
            }

            if (!voucher.IsActive)
            {
                return new VoucherResult { IsValid = false, Message = "Mã voucher này hiện đang bị tạm dừng.", FinalTotal = orderTotal };
            }

            DateTime now = DateTime.Now;
            if (now < voucher.StartDate)
            {
                return new VoucherResult { IsValid = false, Message = "Mã voucher chưa đến thời gian áp dụng.", FinalTotal = orderTotal };
            }

            if (now > voucher.EndDate)
            {
                return new VoucherResult { IsValid = false, Message = "Mã voucher đã hết hạn sử dụng.", FinalTotal = orderTotal };
            }

            if (voucher.Quantity <= 0)
            {
                return new VoucherResult { IsValid = false, Message = "Mã voucher đã hết số lượt sử dụng.", FinalTotal = orderTotal };
            }

            if (orderTotal < voucher.MinOrderAmount)
            {
                return new VoucherResult
                {
                    IsValid = false,
                    Message = string.Format("Đơn hàng tối thiểu phải từ {0:N0} đ để dùng mã này.", voucher.MinOrderAmount),
                    FinalTotal = orderTotal
                };
            }

            decimal discount = 0;
            if (voucher.DiscountType == "Percent")
            {
                discount = orderTotal * (voucher.DiscountValue / 100m);
                if (voucher.MaxDiscountAmount.HasValue && discount > voucher.MaxDiscountAmount.Value)
                {
                    discount = voucher.MaxDiscountAmount.Value;
                }
            }
            else // FixedAmount
            {
                discount = Math.Min(voucher.DiscountValue, orderTotal);
            }

            decimal finalTotal = Math.Max(0, orderTotal - discount);

            return new VoucherResult
            {
                IsValid = true,
                Message = string.Format("Áp dụng mã \"{0}\" thành công! Giảm {1:N0} đ", voucher.Code, discount),
                Voucher = voucher,
                DiscountAmount = discount,
                FinalTotal = finalTotal
            };
        }
    }
}
