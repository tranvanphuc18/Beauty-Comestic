using nv3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace nv3.Controllers
{
    public class ProductController : Controller
    {
        private readonly BeautyCosmeticDBEntities db =
            new BeautyCosmeticDBEntities();

        public ActionResult Index(
            string keyword,
            string category,
            string[] skinTypes,
            string[] brands)
        {
            skinTypes = skinTypes ?? new string[0];
            brands = brands ?? new string[0];

            var products = db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductDetails)
                .Include(p => p.SkinTypes)
                .ToList();

            IEnumerable<Product> result = products;

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();

                result = result.Where(p =>
                    (p.Name != null &&
                     p.Name.IndexOf(
                         kw,
                         StringComparison.OrdinalIgnoreCase) >= 0) ||

                    (p.Brand != null &&
                     p.Brand.IndexOf(
                         kw,
                         StringComparison.OrdinalIgnoreCase) >= 0) ||

                    (p.Description != null &&
                     p.Description.IndexOf(
                         kw,
                         StringComparison.OrdinalIgnoreCase) >= 0));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                string selectedCategory =
                    category.Trim().ToLowerInvariant();

                result = result.Where(p =>
                {
                    string categoryName = p.Category == null
                        ? ""
                        : (p.Category.CategoryName ?? "")
                            .ToLowerInvariant();

                    switch (selectedCategory)
                    {
                        case "cham-soc-da":
                            return categoryName.Contains("chăm sóc da")
                                || categoryName.Contains("cham soc da");

                        case "trang-diem":
                            return categoryName.Contains("trang điểm")
                                || categoryName.Contains("trang diem")
                                || categoryName.Contains("makeup");

                        case "chong-nang":
                            return categoryName.Contains("chống nắng")
                                || categoryName.Contains("chong nang");

                        case "duong-am":
                            return categoryName.Contains("dưỡng ẩm")
                                || categoryName.Contains("duong am");

                        default:
                            return true;
                    }
                });
            }

            if (skinTypes.Length > 0)
            {
                var selectedSkinNames = skinTypes
                    .Select(MapSkinTypeName)
                    .ToList();

                result = result.Where(p =>
                    p.SkinTypes.Any(s =>
                        selectedSkinNames.Contains(
                            (s.SkinTypeName ?? "").Trim(),
                            StringComparer.OrdinalIgnoreCase)));
            }

            if (brands.Length > 0)
            {
                result = result.Where(p =>
                    brands.Any(b =>
                        string.Equals(
                            (p.Brand ?? "").Replace(" ", "-"),
                            b,
                            StringComparison.OrdinalIgnoreCase)));
            }

            ViewBag.Keyword = keyword;
            ViewBag.SelectedSkinTypes = skinTypes;
            ViewBag.SelectedBrands = brands;
            ViewBag.SelectedCategory = category;

            return View(result.OrderBy(p => p.ProductId).ToList());
        }

        public ActionResult Detail(int id)
        {
            var product = db.Products
                .Include(p => p.Category)
                .Include(p => p.ProductDetails)
                .Include(p => p.SkinTypes)
                .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            ViewBag.Rating = product.Rating.ToString("0.0");
            ViewBag.ReviewCount = "Đánh giá của khách hàng";

            var detail = product.ProductDetails
                .OrderBy(d => d.ProductDetailId)
                .FirstOrDefault();

            ViewBag.SoldCount = detail == null
                ? "Chưa có dữ liệu bán"
                : "Đã bán " + detail.SoldQuantity;

            return View(product);
        }

        private static string MapSkinTypeName(string value)
        {
            switch ((value ?? "").Trim().ToLowerInvariant())
            {
                case "oily":
                    return "Da dầu";

                case "dry":
                    return "Da khô";

                case "combination":
                    return "Da hỗn hợp";

                case "sensitive":
                    return "Da nhạy cảm";

                case "normal":
                    return "Da thường";

                default:
                    return value;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}