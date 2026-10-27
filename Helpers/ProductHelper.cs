using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using nv3.Models;

namespace nv3.Helpers
{
    /// <summary>Các hàm dùng chung cho sản phẩm: slug, giá, biến thể còn hàng.</summary>
    public static class ProductHelper
    {
        /// <summary>"Chăm sóc da" -> "cham-soc-da" (bỏ dấu, thay khoảng trắng bằng "-").</summary>
        public static string ToSlug(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return "";
            string s = text.Trim().ToLowerInvariant().Replace('đ', 'd');
            string norm = s.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (char c in norm)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    sb.Append(c);
                }
            }
            s = Regex.Replace(sb.ToString(), "[^a-z0-9]+", "-");
            return s.Trim('-');
        }

        /// <summary>Giá bán thực tế của 1 biến thể: giá khuyến mãi nếu có, không thì giá gốc.</summary>
        public static decimal UnitPrice(ProductDetail d)
        {
            if (d == null) return 0m;
            return d.DiscountedPrice.HasValue ? d.DiscountedPrice.Value : d.Price;
        }

        /// <summary>Biến thể còn hàng đầu tiên (null nếu hết sạch).</summary>
        public static ProductDetail FirstInStock(Product p)
        {
            if (p == null || p.ProductDetails == null) return null;
            return p.ProductDetails.Where(d => d.Quantity > 0).OrderBy(d => d.ProductDetailId).FirstOrDefault();
        }

        /// <summary>Giá hiển thị ngoài danh sách: giá biến thể còn hàng đầu tiên (hoặc biến thể đầu tiên).</summary>
        public static decimal DisplayPrice(Product p)
        {
            var d = FirstInStock(p);
            if (d == null && p != null && p.ProductDetails != null)
            {
                d = p.ProductDetails.OrderBy(x => x.ProductDetailId).FirstOrDefault();
            }
            return UnitPrice(d);
        }
    }
}
