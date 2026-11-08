using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using nv3.Helpers;
using nv3.Models;

namespace nv3.Controllers
{
    public class ProductController : Controller
    {
        private const int PageSize = 9;
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        public ActionResult Index(string keyword, string category, int[] skinTypes, string[] brands, int page = 1)
        {
            skinTypes = skinTypes ?? new int[0];
            brands = brands ?? new string[0];
            if (page < 1) page = 1;

            // 1) Lấy tất cả sản phẩm (bao gồm cả sản phẩm hết hàng để hiển thị giống Shopee)
            var query = db.Products
                .Include(p => p.ProductDetails)
                .Include(p => p.SkinTypes);

            // 2) Tìm kiếm theo tên, thương hiệu, mô tả
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                query = query.Where(p => p.Name.Contains(kw)
                                      || p.Brand.Contains(kw)
                                      || p.Description.Contains(kw));
            }

            // 3) Lọc theo danh mục: so khớp slug của CategoryName
            if (!string.IsNullOrEmpty(category))
            {
                var cat = db.Categories.ToList()
                            .FirstOrDefault(c => ProductHelper.ToSlug(c.CategoryName) == category);
                if (cat == null)
                {
                    query = query.Where(p => false);
                }
                else
                {
                    int catId = cat.CategoryId;
                    query = query.Where(p => p.CategoryId == catId);
                }
            }

            // 4) Lọc loại da (quan hệ nhiều-nhiều Product - SkinType)
            if (skinTypes.Length > 0)
            {
                var skinIds = skinTypes.ToList();
                query = query.Where(p => p.SkinTypes.Any(s => skinIds.Contains(s.SkinTypeId)));
            }

            var list = query.OrderBy(p => p.ProductId).ToList();

            // 5) Lọc thương hiệu
            if (brands.Length > 0)
            {
                list = list.Where(p => brands.Contains(ProductHelper.ToSlug(p.Brand))).ToList();
            }

            // 6) Phân trang
            int totalItems = list.Count;
            int totalPages = (int)Math.Ceiling(totalItems / (double)PageSize);
            if (totalPages < 1) totalPages = 1;
            if (page > totalPages) page = totalPages;

            var items = list.Skip((page - 1) * PageSize).Take(PageSize).ToList();

            ViewBag.Keyword = keyword;
            ViewBag.SkinTypeOptions = db.SkinTypes.OrderBy(s => s.SkinTypeId).ToList();
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
                return HttpNotFound();
            }

            // Tải chi tiết sản phẩm cùng danh mục, biến thể và các loại da phù hợp
            var product = db.Products
                            .Include(p => p.Category)
                            .Include(p => p.ProductDetails)
                            .Include(p => p.SkinTypes)
                            .FirstOrDefault(p => p.ProductId == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            // Tăng lượt xem để đo độ ưa chuộng
            try
            {
                product.ViewQuantity += 1;
                db.SaveChanges();
            }
            catch { }

            // GỢI Ý SẢN PHẨM: Lấy các sản phẩm cùng danh mục được mua nhiều nhất (SoldQuantity) và được xem nhiều nhất
            var recommended = db.Products
                                .Include(p => p.ProductDetails)
                                .Include(p => p.SkinTypes)
                                .Where(p => p.ProductId != id && p.CategoryId == product.CategoryId)
                                .OrderByDescending(p => p.ProductDetails.Sum(d => (int?)d.SoldQuantity) ?? 0)
                                .ThenByDescending(p => p.ViewQuantity)
                                .Take(4)
                                .ToList();

            // Nếu cùng danh mục có ít hơn 4 sản phẩm thì bổ sung các sản phẩm bán chạy nhất hệ thống
            if (recommended.Count < 4)
            {
                var existingIds = recommended.Select(r => r.ProductId).ToList();
                existingIds.Add(product.ProductId);

                var more = db.Products
                             .Include(p => p.ProductDetails)
                             .Include(p => p.SkinTypes)
                             .Where(p => !existingIds.Contains(p.ProductId))
                             .OrderByDescending(p => p.ProductDetails.Sum(d => (int?)d.SoldQuantity) ?? 0)
                             .Take(4 - recommended.Count)
                             .ToList();

                recommended.AddRange(more);
            }

            ViewBag.RecommendedProducts = recommended;

            return View(product);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
