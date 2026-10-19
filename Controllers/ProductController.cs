
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

        public ActionResult Index(
      string keyword,     // <--- 1. MỚI THÊM: Đón từ khóa người dùng gõ
      string category,
      string[] skinTypes,
      string[] brands)
        {
            skinTypes = skinTypes ?? new string[0];
            brands = brands ?? new string[0];
            IEnumerable<Product> result = products;
            // ================= 2. MỚI THÊM: XỬ LÝ TÌM KIẾM =================
            if (!string.IsNullOrEmpty(keyword))
            {
                string kw = keyword.Trim().ToLower();
                // Tìm theo cả Tên sản phẩm, Tên thương hiệu và Mô tả
                result = result.Where(p =>
                    p.Name.ToLower().Contains(kw) ||
                    p.Brand.ToLower().Contains(kw) ||
                    (p.Description != null && p.Description.ToLower().Contains(kw))
                );
            }
            // ==============================================================
            // Lọc theo danh mục (giữ nguyên của bạn)
            if (!string.IsNullOrEmpty(category))
            {
                switch (category.ToLower())
                {
                    case "cham-soc-da":
                        result = result.Where(p =>
                            p.Id == 1 || p.Id == 2 ||
                            p.Id == 3 || p.Id == 4);
                        break;
                    case "trang-diem":
                        result = result.Where(p => p.Id == 6);
                        break;
                    case "chong-nang":
                        result = result.Where(p => p.Id == 5);
                        break;
                    case "duong-am":
                        result = result.Where(p =>
                            p.Id == 2 || p.Id == 4);
                        break;
                }
            }
            // Lọc theo loại da (giữ nguyên của bạn)
            if (skinTypes.Length > 0)
            {
                result = result.Where(p =>
                    (p.SkinTypes ?? new string[0])
                    .Intersect(skinTypes)
                    .Any());
            }
            // Lọc theo thương hiệu (giữ nguyên của bạn)
            if (brands.Length > 0)
            {
                result = result.Where(p =>
                    brands.Contains(
                        p.Brand.ToLower().Replace(" ", "-")));
            }
            ViewBag.Keyword = keyword; // Lưu lại từ khóa
            ViewBag.SelectedSkinTypes = skinTypes;
            ViewBag.SelectedBrands = brands;
            ViewBag.SelectedCategory = category;
            return View(result.ToList());
        }

        public ActionResult Detail(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // === THÊM ĐÁNH GIÁ RIÊNG CHO TỪNG SẢN PHẨM TẠI ĐÂY ===
            switch (id)
            {
                case 1: // CeraVe
                    ViewBag.Rating = "5.0";
                    ViewBag.ReviewCount = "128 đánh giá";
                    ViewBag.SoldCount = "Đã bán 1.2k";
                    break;

                case 2: // Klairs
                    ViewBag.Rating = "4.9";
                    ViewBag.ReviewCount = "89 đánh giá";
                    ViewBag.SoldCount = "Đã bán 850";
                    break;

                case 3: // Melano CC
                    ViewBag.Rating = "4.7";
                    ViewBag.ReviewCount = "215 đánh giá";
                    ViewBag.SoldCount = "Đã bán 2.4k";
                    break;

                case 4: // La Roche-Posay
                    ViewBag.Rating = "5.0";
                    ViewBag.ReviewCount = "340 đánh giá";
                    ViewBag.SoldCount = "Đã bán 3.1k";
                    break;

                case 5: // Anessa
                    ViewBag.Rating = "4.9";
                    ViewBag.ReviewCount = "512 đánh giá";
                    ViewBag.SoldCount = "Đã bán 5.0k";
                    break;

                case 6: // YSL
                    ViewBag.Rating = "5.0";
                    ViewBag.ReviewCount = "64 đánh giá";
                    ViewBag.SoldCount = "Đã bán 320";
                    break;

                default:
                    ViewBag.Rating = "5.0";
                    ViewBag.ReviewCount = "100 đánh giá";
                    ViewBag.SoldCount = "Đã bán 500";
                    break;
            }

            return View(product);
        }
    }
}
