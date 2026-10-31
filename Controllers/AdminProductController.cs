using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using nv3.Models;

namespace nv3.Controllers
{
    public class AdminProductController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        private bool CheckAdmin() => Session["Role"] != null && Session["Role"].ToString() == "Admin";

        // 1. Danh sách sản phẩm
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var products = db.Products
                             .Include(p => p.Category)
                             .Include(p => p.ProductDetails)
                             .OrderByDescending(p => p.CreatedAt)
                             .ToList();
            return View(products);
        }

        // 2. Thêm sản phẩm - GET
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // 2. Thêm sản phẩm - POST (Kèm upload ảnh + tạo ProductDetail mặc định)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, decimal price, int quantity, HttpPostedFileBase imageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(imageFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string folder = Server.MapPath("~/Content/Uploads/Products/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    imageFile.SaveAs(Path.Combine(folder, fileName));
                    product.Image = fileName;
                }
                else
                {
                    product.Image = "no-image.png";
                }

                product.CreatedAt = DateTime.Now;
                product.Rating = 5;
                product.ViewQuantity = 0;
                db.Products.Add(product);
                db.SaveChanges();

                // Tạo kèm 1 bản ghi tồn kho & giá trong ProductDetail
                var detail = new ProductDetail
                {
                    ProductId = product.ProductId,
                    DetailName = "Mặc định",
                    Price = price,
                    Quantity = quantity,
                    SoldQuantity = 0
                };
                db.ProductDetails.Add(detail);
                db.SaveChanges();

                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // 3. Sửa sản phẩm - GET
        public ActionResult Edit(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var product = db.Products.Find(id);
            if (product == null) return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // 3. Sửa sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase imageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var existing = db.Products.Find(product.ProductId);
                if (existing == null) return HttpNotFound();

                existing.Name = product.Name;
                existing.CategoryId = product.CategoryId;
                existing.Brand = product.Brand;
                existing.Description = product.Description;
                existing.Recommend = product.Recommend;

                // Cập nhật ảnh mới nếu có tải lên
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(imageFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;
                    string folder = Server.MapPath("~/Content/Uploads/Products/");
                    if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                    imageFile.SaveAs(Path.Combine(folder, fileName));
                    existing.Image = fileName;
                }

                db.SaveChanges();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // 4. Xóa sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var product = db.Products.Include(p => p.ProductDetails).FirstOrDefault(p => p.ProductId == id);
            if (product != null)
            {
                // Kiểm tra xem đã có đơn hàng nào mua sản phẩm này chưa
                bool hasOrders = product.ProductDetails.Any(pd => db.OrderDetails.Any(od => od.ProductDetailId == pd.ProductDetailId));
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa sản phẩm này vì đã phát sinh trong đơn hàng của khách!";
                    return RedirectToAction("Index");
                }

                // Xóa các chi tiết sản phẩm liên quan
                db.ProductDetails.RemoveRange(product.ProductDetails);
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction("Index");
        }

        // 5. Xem chi tiết sản phẩm
        public ActionResult Detail(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var product = db.Products
                            .Include(p => p.Category)
                            .Include(p => p.ProductDetails)
                            .FirstOrDefault(p => p.ProductId == id);
            if (product == null) return HttpNotFound();
            return View(product);
        }

        // 6. Quản lý tồn kho theo checklist: còn hàng, cần nhập thêm theo ProductDetail
        public ActionResult Inventory()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var inventory = db.ProductDetails
                              .Include(pd => pd.Product)
                              .Include(pd => pd.Product.Category)
                              .OrderBy(pd => pd.Quantity)
                              .ToList();
            return View(inventory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}