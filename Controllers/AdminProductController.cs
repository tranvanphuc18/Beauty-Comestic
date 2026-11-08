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

        // 1. Danh sách sản phẩm (có tìm kiếm)
        public ActionResult Index(string keyword = "")
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var products = db.Products
                             .Include(p => p.Category)
                             .Include(p => p.ProductDetails)
                             .OrderByDescending(p => p.CreatedAt)
                             .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                string kw = keyword.Trim();
                products = products.Where(p => p.Name.Contains(kw) || p.Brand.Contains(kw));
            }

            ViewBag.Keyword = keyword;
            return View(products.ToList());
        }

        // 2. Thêm sản phẩm - GET
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName");
            ViewBag.AllSkinTypes = db.SkinTypes.OrderBy(s => s.SkinTypeId).ToList();
            return View();
        }

        // 2. Thêm sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, decimal price, decimal? discountedPrice, int quantity, int[] selectedSkinTypes, HttpPostedFileBase imageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                // Xử lý upload ảnh (Lưu vào CẢ Content/Uploads/Products VÀ Images để trang khách không bị vỡ)
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(imageFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;

                    string uploadFolder = Server.MapPath("~/Content/Uploads/Products/");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    imageFile.SaveAs(Path.Combine(uploadFolder, fileName));

                    string imagesFolder = Server.MapPath("~/Images/");
                    if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
                    System.IO.File.Copy(Path.Combine(uploadFolder, fileName), Path.Combine(imagesFolder, fileName), true);

                    product.Image = fileName;
                }
                else
                {
                    product.Image = "no-image.png";
                }

                // Gán loại da đã chọn
                if (selectedSkinTypes != null && selectedSkinTypes.Length > 0)
                {
                    var skins = db.SkinTypes.Where(s => selectedSkinTypes.Contains(s.SkinTypeId)).ToList();
                    foreach (var s in skins)
                    {
                        product.SkinTypes.Add(s);
                    }
                }

                product.CreatedAt = DateTime.Now;
                product.Rating = 5;
                product.ViewQuantity = 0;
                db.Products.Add(product);
                db.SaveChanges();

                // Tạo kèm ProductDetail có giá bán, giá khuyến mãi và số lượng tồn kho
                var detail = new ProductDetail
                {
                    ProductId = product.ProductId,
                    DetailName = "Mặc định",
                    Price = price,
                    DiscountedPrice = (discountedPrice.HasValue && discountedPrice.Value > 0) ? discountedPrice : null,
                    Quantity = quantity,
                    SoldQuantity = 0
                };
                db.ProductDetails.Add(detail);
                db.SaveChanges();

                TempData["Success"] = "Thêm sản phẩm mới thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.AllSkinTypes = db.SkinTypes.OrderBy(s => s.SkinTypeId).ToList();
            ViewBag.SelectedSkinTypeIds = selectedSkinTypes != null ? selectedSkinTypes.ToList() : new System.Collections.Generic.List<int>();
            return View(product);
        }

        // 3. Sửa sản phẩm - GET
        public ActionResult Edit(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var product = db.Products
                            .Include(p => p.SkinTypes)
                            .Include(p => p.ProductDetails)
                            .FirstOrDefault(p => p.ProductId == id);

            if (product == null) return HttpNotFound();

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.AllSkinTypes = db.SkinTypes.OrderBy(s => s.SkinTypeId).ToList();
            ViewBag.SelectedSkinTypeIds = product.SkinTypes.Select(s => s.SkinTypeId).ToList();

            var firstDetail = product.ProductDetails.FirstOrDefault();
            ViewBag.Price = firstDetail != null ? firstDetail.Price : 0;
            ViewBag.DiscountedPrice = firstDetail != null ? firstDetail.DiscountedPrice : null;
            ViewBag.Quantity = firstDetail != null ? firstDetail.Quantity : 0;

            return View(product);
        }

        // 3. Sửa sản phẩm - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, decimal? price, decimal? discountedPrice, int? quantity, int[] selectedSkinTypes, HttpPostedFileBase imageFile)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                var existing = db.Products
                                 .Include(p => p.SkinTypes)
                                 .Include(p => p.ProductDetails)
                                 .FirstOrDefault(p => p.ProductId == product.ProductId);

                if (existing == null) return HttpNotFound();

                existing.Name = product.Name;
                existing.CategoryId = product.CategoryId;
                existing.Brand = product.Brand;
                existing.Description = product.Description;
                existing.Recommend = product.Recommend;

                // Cập nhật giá bán, giá khuyến mãi và tồn kho
                var firstDetail = existing.ProductDetails.FirstOrDefault();
                if (firstDetail != null)
                {
                    if (price.HasValue) firstDetail.Price = price.Value;
                    firstDetail.DiscountedPrice = (discountedPrice.HasValue && discountedPrice.Value > 0) ? discountedPrice : null;
                    if (quantity.HasValue) firstDetail.Quantity = quantity.Value;
                }

                // Cập nhật danh sách loại da
                existing.SkinTypes.Clear();
                if (selectedSkinTypes != null && selectedSkinTypes.Length > 0)
                {
                    var skins = db.SkinTypes.Where(s => selectedSkinTypes.Contains(s.SkinTypeId)).ToList();
                    foreach (var s in skins)
                    {
                        existing.SkinTypes.Add(s);
                    }
                }

                // Cập nhật ảnh mới nếu có tải lên (Lưu vào cả 2 thư mục)
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    string ext = Path.GetExtension(imageFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + ext;

                    string uploadFolder = Server.MapPath("~/Content/Uploads/Products/");
                    if (!Directory.Exists(uploadFolder)) Directory.CreateDirectory(uploadFolder);
                    imageFile.SaveAs(Path.Combine(uploadFolder, fileName));

                    string imagesFolder = Server.MapPath("~/Images/");
                    if (!Directory.Exists(imagesFolder)) Directory.CreateDirectory(imagesFolder);
                    System.IO.File.Copy(Path.Combine(uploadFolder, fileName), Path.Combine(imagesFolder, fileName), true);

                    existing.Image = fileName;
                }

                db.SaveChanges();
                TempData["Success"] = "Cập nhật sản phẩm thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewBag.AllSkinTypes = db.SkinTypes.OrderBy(s => s.SkinTypeId).ToList();
            ViewBag.SelectedSkinTypeIds = selectedSkinTypes != null ? selectedSkinTypes.ToList() : new System.Collections.Generic.List<int>();
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
                bool hasOrders = product.ProductDetails.Any(pd => db.OrderDetails.Any(od => od.ProductDetailId == pd.ProductDetailId));
                if (hasOrders)
                {
                    TempData["Error"] = "Không thể xóa sản phẩm này vì đã phát sinh trong đơn hàng của khách!";
                    return RedirectToAction("Index");
                }

                if (!string.IsNullOrEmpty(product.Image) && product.Image != "no-image.png")
                {
                    string imgPath1 = Server.MapPath("~/Content/Uploads/Products/" + product.Image);
                    if (System.IO.File.Exists(imgPath1)) System.IO.File.Delete(imgPath1);

                    string imgPath2 = Server.MapPath("~/Images/" + product.Image);
                    if (System.IO.File.Exists(imgPath2)) System.IO.File.Delete(imgPath2);
                }

                db.ProductDetails.RemoveRange(product.ProductDetails);
                db.Products.Remove(product);
                db.SaveChanges();
                TempData["Success"] = "Xóa sản phẩm thành công!";
            }

            return RedirectToAction("Index");
        }

        // 5. Quản lý tồn kho theo checklist: còn hàng, cần nhập thêm theo ProductDetail
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

        // 6. Nhập thêm hàng vào kho
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStock(int productDetailId, int addQuantity)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (addQuantity <= 0)
            {
                TempData["Error"] = "Số lượng nhập thêm phải lớn hơn 0.";
                return RedirectToAction("Inventory");
            }

            var detail = db.ProductDetails.Include(d => d.Product).FirstOrDefault(d => d.ProductDetailId == productDetailId);
            if (detail == null) return HttpNotFound();

            detail.Quantity += addQuantity;
            db.SaveChanges();
            TempData["Success"] = "Đã nhập thêm " + addQuantity + " sản phẩm vào kho. Tồn kho hiện tại: " + detail.Quantity + ".";
            return RedirectToAction("Inventory");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
