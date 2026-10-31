using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using nv3.Models;

namespace nv3.Controllers
{
    public class AdminCategoryController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        private bool CheckAdmin()
        {
            return Session["Role"] != null && Session["Role"].ToString() == "Admin";
        }

        // 1. Danh sách danh mục
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var categories = db.Categories.Include(c => c.Products).ToList();
            return View(categories);
        }

        // 2. Thêm danh mục - GET
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // 2. Thêm danh mục - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Category category)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (db.Categories.Any(c => c.CategoryName.Trim().ToLower() == category.CategoryName.Trim().ToLower()))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục này đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                db.Categories.Add(category);
                db.SaveChanges();
                TempData["Success"] = "Thêm danh mục mới thành công!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        public ActionResult Edit(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var category = db.Categories.Find(id);
            if (category == null) return HttpNotFound();
            return View(category);
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Category category)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (db.Categories.Any(c => c.CategoryId != category.CategoryId && c.CategoryName.Trim().ToLower() == category.CategoryName.Trim().ToLower()))
            {
                ModelState.AddModelError("CategoryName", "Tên danh mục này đã được sử dụng!");
            }

            if (ModelState.IsValid)
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
                TempData["Success"] = "Cập nhật danh mục thành công!";
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var category = db.Categories.Include(c => c.Products).FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return HttpNotFound();

            if (category.Products != null && category.Products.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục này vì đang có " + category.Products.Count + " sản phẩm liên kết!";
                return RedirectToAction("Index");
            }

            db.Categories.Remove(category);
            db.SaveChanges();
            TempData["Success"] = "Xóa danh mục thành công!";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}