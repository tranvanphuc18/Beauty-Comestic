namespace nv3.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Voucher
    {
        public int VoucherId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã Voucher.")]
        [StringLength(50, ErrorMessage = "Mã Voucher không quá 50 ký tự.")]
        [RegularExpression(@"^[A-Z0-9_-]+$", ErrorMessage = "Mã voucher chỉ gồm chữ in hoa, số và dấu gạch.")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề khuyến mãi.")]
        [StringLength(200, ErrorMessage = "Tiêu đề không quá 200 ký tự.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại giảm giá.")]
        public string DiscountType { get; set; } // "Percent" hoặc "FixedAmount"

        [Required(ErrorMessage = "Vui lòng nhập giá trị giảm giá.")]
        [Range(1, 100000000, ErrorMessage = "Giá trị giảm giá không hợp lệ.")]
        public decimal DiscountValue { get; set; }

        [Display(Name = "Đơn hàng tối thiểu")]
        [Range(0, 100000000, ErrorMessage = "Đơn tối thiểu không hợp lệ.")]
        public decimal MinOrderAmount { get; set; }

        [Display(Name = "Giảm tối đa")]
        [Range(0, 100000000, ErrorMessage = "Giảm tối đa không hợp lệ.")]
        public Nullable<decimal> MaxDiscountAmount { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số lượng voucher.")]
        [Range(0, 100000, ErrorMessage = "Số lượng từ 0 đến 100,000.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày bắt đầu.")]
        public System.DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày kết thúc.")]
        public System.DateTime EndDate { get; set; }

        public bool IsActive { get; set; }
        public System.DateTime CreatedAt { get; set; }
    }
}
