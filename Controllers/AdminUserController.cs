using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using nv3.Models;

namespace nv3.Controllers
{
    public class AdminUserController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();


        private bool CheckAdmin() => Session["Role"] != null && Session["Role"].ToString() == "Admin";

        
        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var admins = db.Users.Where(u => u.Role == "Admin").ToList();
            return View(admins);
        }

       
        public ActionResult Create()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user, string Password)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            if (string.IsNullOrWhiteSpace(Password))
            {
                ModelState.AddModelError("PasswordHash", "Mật khẩu không được để trống!");
            }

            if (db.Users.Any(u => u.Email.Trim().ToLower() == user.Email.Trim().ToLower()))
            {
                ModelState.AddModelError("Email", "Email này đã được sử dụng!");
            }

            if (ModelState.IsValid)
            {
                user.PasswordHash = Password; 
                user.Role = "Admin";
                user.CreatedAt = DateTime.Now;
                db.Users.Add(user);
                db.SaveChanges();
                TempData["Success"] = "Tạo tài khoản Quản trị viên thành công!";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var user = db.Users.Find(id);
            if (user != null)
            {
              
                if (Session["UserEmail"] != null && Session["UserEmail"].ToString() == user.Email)
                {
                    TempData["Error"] = "Không thể xóa chính tài khoản bạn đang đăng nhập!";
                    return RedirectToAction("Index");
                }

                db.Users.Remove(user);
                db.SaveChanges();
                TempData["Success"] = "Đã xóa tài khoản Admin!";
            }
            return RedirectToAction("Index");
        }

      
        public ActionResult Customers()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var customers = db.Users.Where(u => u.Role == "Customer").OrderByDescending(u => u.CreatedAt).ToList();
            return View(customers);
        }

      
        public ActionResult CustomerDetail(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");
            var customer = db.Users.Include(u => u.Orders).FirstOrDefault(u => u.UserId == id);
            if (customer == null) return HttpNotFound();
            return View(customer);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}