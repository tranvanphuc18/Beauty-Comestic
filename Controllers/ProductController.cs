using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using nv3.Models;

namespace nv3.Controllers
{
    public class ProductController : Controller
    {
        private const int PageSize = 9;
        private BeautyCosmeticEntities db = new BeautyCosmeticEntities();

        public ActionResult Index(string keyword, string category, string[] skinTypes, string[] brands, int page = 1)
        {
            skinTypes = skinTypes ?? new string[0];
            brands = brands ?? new string[0];
            if (page < 1) page = 1;

            // 1) Chỉ lấy sản phẩm còn hàng
            var query = db.Products.Where(p => p.Quantity > 0);

            // 2) Tìm kiếm theo tên, thương hiệu, mô tả
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                query = query.Where(p => p.ProductName.Contains(kw)
                                      || p.Brand.Contains(kw)
                                      || p.Description.Contains(kw));
            }

            // 3) Lọc theo danh mục (slug)
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.Slug == category);
            }

            var list = query.OrderBy(p => p.ProductID).ToList();

            // 4) Lọc loại da (SkinTypes lưu dạng "oily,dry") - lọc trong bộ nhớ cho đơn giản
            if (skinTypes.Length > 0)
            {
                list = list.Where(p => !string.IsNullOrEmpty(p.SkinTypes)
                    && p.SkinTypes.Split(',').Select(s => s.Trim()).Intersect(skinTypes).Any()).ToList();
            }

            // 5) Lọc thương hiệu (value trong filter = tên viết thường, thay dấu cách bằng "-")
            if (brands.Length > 0)
            {
                list = list.Where(p => brands.Contains(p.Brand.ToLower().Replace(" ", "-"))).ToList();
            }

            // 6) Phân trang
            int totalItems = list.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = list.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.SelectedSkinTypes = skinTypes;
            ViewBag.SelectedBrands = brands;
            ViewBag.SelectedCategory = category;
            ViewBag.Page = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(items);
        }

        public ActionResult Detail(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
