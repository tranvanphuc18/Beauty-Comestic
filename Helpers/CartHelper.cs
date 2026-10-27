using System.Collections.Generic;
using System.Linq;
using System.Web;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Helpers
{
    /// <summary>Giỏ hàng lưu trong Session của người dùng.</summary>
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

        /// <summary>
        /// Đồng bộ giỏ với database: cập nhật giá/tồn kho mới nhất,
        /// bỏ sản phẩm đã hết hàng, hạ số lượng nếu vượt tồn kho.
        /// </summary>
        public static List<CartItem> Refresh(BeautyCosmeticEntities db)
        {
            var cart = GetCart();
            var ids = cart.Select(i => i.ProductID).ToList();
            var products = db.Products.Where(p => ids.Contains(p.ProductID)).ToList();

            for (int i = cart.Count - 1; i >= 0; i--)
            {
                var item = cart[i];
                var p = products.FirstOrDefault(x => x.ProductID == item.ProductID);

                if (p == null || p.Quantity <= 0)
                {
                    cart.RemoveAt(i);
                    continue;
                }

                item.ProductName = p.ProductName;
                item.Image = p.Image;
                item.Price = p.Price;
                item.Stock = p.Quantity;

                if (item.Quantity > p.Quantity)
                {
                    item.Quantity = p.Quantity;
                }
            }
            return cart;
        }
    }
}
