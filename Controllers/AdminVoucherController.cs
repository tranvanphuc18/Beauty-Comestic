using System;
using System.Linq;
using System.Web.Mvc;
using nv3.Models;

namespace nv3.Controllers
{
    public class AdminVoucherController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        private bool CheckAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // 1. Danh sách Voucher
        public ActionResult Index(string search = "", string status = "")
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var query = db.Vouchers.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                string s = search.Trim().ToLower();
                query = query.Where(v => v.Code.ToLower().Contains(s) || v.Title.ToLower().Contains(s));
            }

            DateTime now = DateTime.Now;
            if (status == "active")
            {
                query = query.Where(v => v.IsActive && v.StartDate <= now && v.EndDate >= now && v.Quantity > 0);
            }
            else if (status == "inactive")
            {
                query = query.Where(v => !v.IsActive);
            }
            else if (status == "expired")
            {
                query = query.Where(v => v.EndDate < now);
            }
            else if (status == "out_of_stock")
            {
                query = query.Where(v => v.Quantity <= 0);
            }

            var list = query.OrderByDescending(v => v.VoucherId).ToList();

            ViewBag.Search = search;
            ViewBag.Status = status;
            ViewBag.TotalCount = list.Count;

            return View(list);
        }

        // 2. Thêm Voucher - GET
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var model = new Voucher
            {
                DiscountType = "Percent",
                DiscountValue = 10,
                MinOrderAmount = 0,
                Quantity = 100,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                IsActive = true
            };
            return View(model);
        }

        // 2. Thêm Voucher - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Voucher model)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                model.Code = model.Code.Trim().ToUpper();
                if (db.Vouchers.Any(v => v.Code == model.Code))
                {
                    ModelState.AddModelError("Code", "Mã Voucher này đã tồn tại trong hệ thống!");
                }
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            if (model.DiscountType == "Percent" && model.DiscountValue > 100)
            {
                ModelState.AddModelError("DiscountValue", "Giảm theo phần trăm không được vượt quá 100%.");
            }

            if (ModelState.IsValid)
            {
                model.CreatedAt = DateTime.Now;
                db.Vouchers.Add(model);
                db.SaveChanges();

                TempData["Success"] = "Thêm mã khuyến mãi \"" + model.Code + "\" thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // 3. Sửa Voucher - GET
        public ActionResult Edit(int? id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return HttpNotFound();

            var voucher = db.Vouchers.Find(id);
            if (voucher == null) return HttpNotFound();

            return View(voucher);
        }

        // 3. Sửa Voucher - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Voucher model)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (!string.IsNullOrWhiteSpace(model.Code))
            {
                model.Code = model.Code.Trim().ToUpper();
                if (db.Vouchers.Any(v => v.Code == model.Code && v.VoucherId != model.VoucherId))
                {
                    ModelState.AddModelError("Code", "Mã Voucher này đã tồn tại ở bản ghi khác!");
                }
            }

            if (model.EndDate < model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
            }

            if (model.DiscountType == "Percent" && model.DiscountValue > 100)
            {
                ModelState.AddModelError("DiscountValue", "Giảm theo phần trăm không được vượt quá 100%.");
            }

            if (ModelState.IsValid)
            {
                var existing = db.Vouchers.Find(model.VoucherId);
                if (existing == null) return HttpNotFound();

                existing.Code = model.Code;
                existing.Title = model.Title;
                existing.DiscountType = model.DiscountType;
                existing.DiscountValue = model.DiscountValue;
                existing.MinOrderAmount = model.MinOrderAmount;
                existing.MaxDiscountAmount = model.MaxDiscountAmount;
                existing.Quantity = model.Quantity;
                existing.StartDate = model.StartDate;
                existing.EndDate = model.EndDate;
                existing.IsActive = model.IsActive;

                db.SaveChanges();
                TempData["Success"] = "Cập nhật mã Voucher \"" + model.Code + "\" thành công!";
                return RedirectToAction("Index");
            }

            return View(model);
        }

        // 4. Bật/Tắt nhanh trạng thái
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ToggleStatus(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var voucher = db.Vouchers.Find(id);
            if (voucher != null)
            {
                voucher.IsActive = !voucher.IsActive;
                db.SaveChanges();
                TempData["Success"] = (voucher.IsActive ? "Đã kích hoạt" : "Đã tạm dừng") + " mã \"" + voucher.Code + "\"!";
            }
            return RedirectToAction("Index");
        }

        // 5. Xóa Voucher
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var voucher = db.Vouchers.Find(id);
            if (voucher != null)
            {
                string code = voucher.Code;
                db.Vouchers.Remove(voucher);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa mã Voucher \"" + code + "\"!";
            }
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
