using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Helpers
{
    /// <summary>Giỏ hàng lưu trong Session. Mỗi dòng giỏ = 1 biến thể sản phẩm (ProductDetail).</summary>
    public static class CartHelper
    {
        private const string CartKey = "Cart";

        public static List<CartItem> GetCart()
        {
            var session = HttpContext.Current.Session;
            var cart = session[CartKey] as List<CartItem>;
            if (cart == null)
            {
                cart = new List<CartItem>();
                session[CartKey] = cart;
            }
            return cart;
        }

        public static void Clear()
        {
            HttpContext.Current.Session[CartKey] = new List<CartItem>();
        }

        public static int Count()
        {
            return GetCart().Sum(i => i.Quantity);
        }

        /// <summary>Tạo dòng giỏ từ 1 biến thể sản phẩm.</summary>
        public static CartItem ToCartItem(ProductDetail d, int quantity)
        {
            return new CartItem
            {
                ProductDetailId = d.ProductDetailId,
                ProductId = d.ProductId,
                ProductName = d.Product.Name,
                DetailName = d.DetailName,
                Image = d.Product.Image,
                Price = ProductHelper.UnitPrice(d),
                Quantity = quantity,
                Stock = d.Quantity
            };
        }

        /// <summary>
        /// Đồng bộ giỏ với database: cập nhật giá/tồn kho mới nhất,
        /// bỏ biến thể đã hết hàng, hạ số lượng nếu vượt tồn kho.
        /// </summary>
        public static List<CartItem> Refresh(BeautyCosmeticDBEntities db)
        {
            var cart = GetCart();
            var ids = cart.Select(i => i.ProductDetailId).ToList();
            var details = db.ProductDetails.Include(d => d.Product)
                            .Where(d => ids.Contains(d.ProductDetailId)).ToList();

            for (int i = cart.Count - 1; i >= 0; i--)
            {
                var item = cart[i];
                var d = details.FirstOrDefault(x => x.ProductDetailId == item.ProductDetailId);

                if (d == null || d.Quantity <= 0)
                {
                    cart.RemoveAt(i);
                    continue;
                }

                item.ProductName = d.Product.Name;
                item.DetailName = d.DetailName;
                item.Image = d.Product.Image;
                item.Price = ProductHelper.UnitPrice(d);
                item.Stock = d.Quantity;

                if (item.Quantity > d.Quantity)
                {
                    item.Quantity = d.Quantity;
                }
            }
            return cart;
        }
    }
}
