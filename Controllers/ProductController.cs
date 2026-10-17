using MyPham_Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace nv3.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Sữa rửa mặt dịu nhẹ",
                Brand = "CeraVe",
                Price = 250000,
                Image = "ceraVe.jpg",
                Description = "Sữa rửa mặt dịu nhẹ giúp làm sạch da.",
                SkinTypes = new[] { "oily", "combination" }
            },
            new Product
            {
                Id = 2,
                Name = "Toner cấp ẩm",
                Brand = "Klairs",
                Price = 320000,
                Image = "klairs.jpg",
                Description = "Toner giúp cấp ẩm và làm dịu da.",
                SkinTypes = new[] { "dry", "combination", "sensitive" }
            },
            new Product
            {
                Id = 3,
                Name = "Serum Vitamin C",
                Brand = "Melano CC",
                Price = 280000,
                Image = "melano.jpg",
                Description = "Serum chăm sóc da với Vitamin C.",
                SkinTypes = new[] { "oily", "combination" }
            },
            new Product
            {
                Id = 4,
                Name = "Kem dưỡng ẩm phục hồi",
                Brand = "La Roche-Posay",
                Price = 450000,
                Image = "larocheposay.jpg",
                Description = "Kem dưỡng ẩm hỗ trợ phục hồi da.",
                SkinTypes = new[] { "dry", "sensitive" }
            },
            new Product
            {
                Id = 5,
                Name = "Kem chống nắng",
                Brand = "Anessa",
                Price = 400000,
                Image = "anessa.jpg",
                Description = "Kem chống nắng dùng hằng ngày.",
                SkinTypes = new[] { "oily", "dry", "combination", "sensitive" }
            },
            new Product
            {
                Id = 6,
                Name = "Son cao cấp",
                Brand = "YSL",
                Price = 950000,
                Image = "ysl.jpg",
                Description = "Son môi cao cấp với thiết kế sang trọng.",
                SkinTypes = new string[0]
            }
        };

        public ActionResult Index(string[] skinTypes, string[] brands)
        {
            skinTypes = skinTypes ?? new string[0];
            brands = brands ?? new string[0];

            IEnumerable<Product> result = products;

            // Lọc theo loại da: sản phẩm phù hợp với ít nhất 1 loại da đã chọn
            if (skinTypes.Length > 0)
            {
                result = result.Where(p => (p.SkinTypes ?? new string[0]).Intersect(skinTypes).Any());
            }

            // Lọc theo thương hiệu: "La Roche-Posay" -> "la-roche-posay"
            if (brands.Length > 0)
            {
                result = result.Where(p => brands.Contains(p.Brand.ToLower().Replace(" ", "-")));
            }

            // Để form nhớ các ô đã tick sau khi tải lại trang
            ViewBag.SelectedSkinTypes = skinTypes;
            ViewBag.SelectedBrands = brands;

            return View(result.ToList());
        }

        public ActionResult Detail(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
    }
}