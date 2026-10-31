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

            // 1) Chỉ lấy sản phẩm còn hàng (có ít nhất 1 biến thể còn tồn kho)
            var query = db.Products
                .Include(p => p.ProductDetails)
                .Where(p => p.ProductDetails.Any(d => d.Quantity > 0));

            // 2) Tìm kiếm theo tên, thương hiệu, mô tả
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                query = query.Where(p => p.Name.Contains(kw)
                                      || p.Brand.Contains(kw)
                                      || p.Description.Contains(kw));
            }

            // 3) Lọc theo danh mục: bảng Categories không có cột Slug nên so khớp slug của CategoryName
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

            // 5) Lọc thương hiệu (value trong filter = slug của tên thương hiệu)
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
