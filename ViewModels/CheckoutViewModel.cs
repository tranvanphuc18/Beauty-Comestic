using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace nv3.ViewModels
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên người nhận.")]
        [StringLength(100)]
        public string ReceiverName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại.")]
        [RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại gồm 10 chữ số, bắt đầu bằng số 0.")]
        public string ReceiverPhone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ giao hàng.")]
        [StringLength(255)]
        public string ShippingAddress { get; set; }

        // Không lấy từ form, controller tự gán lại từ giỏ hàng
        public List<CartItem> Items { get; set; }

        public decimal Total
        {
            get { return Items == null ? 0 : Items.Sum(i => i.Total); }
        }
    }
}
