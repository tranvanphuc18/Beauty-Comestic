using System;
using System.Linq;
using System.Web.Mvc;
using nv3.Attributes;
using nv3.Helpers;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Controllers
{
    public class AccountController : Controller
    {
        private BeautyCosmeticEntities db = new BeautyCosmeticEntities();

        // ---------- ĐĂNG KÝ ----------
        [HttpGet]
        public ActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email.Trim().ToLower();

            // Phát hiện tài khoản trùng
            if (db.Customers.Any(c => c.Email == email))
            {
                ModelState.AddModelError("Email", "Email này đã được đăng ký. Vui lòng dùng email khác hoặc đăng nhập.");
                return View(model);
            }

            var customer = new Customer
            {
                FullName = model.FullName.Trim(),
                Email = email,
                PasswordHash = PasswordHelper.Hash(model.Password),
                CreatedDate = DateTime.Now
            };
            db.Customers.Add(customer);
            db.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // ---------- ĐĂNG NHẬP ----------
        [HttpGet]
        public ActionResult Login(string returnUrl)
        {
            if (Session["CustomerID"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            string email = model.Email.Trim().ToLower();
            var customer = db.Customers.FirstOrDefault(c => c.Email == email);

            if (customer == null || !PasswordHelper.Verify(model.Password, customer.PasswordHash))
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }

            Session["CustomerID"] = customer.CustomerID;
            Session["CustomerName"] = customer.FullName;

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }

        // ---------- HỒ SƠ ----------
        [CustomerAuthorize]
        [HttpGet]
        public ActionResult Profile()
        {
            int customerId = (int)Session["CustomerID"];
            var customer = db.Customers.Find(customerId);
            if (customer == null)
            {
                return RedirectToAction("Logout");
            }

            return View(new ProfileViewModel
            {
                FullName = customer.FullName,
                Email = customer.Email,
                Phone = customer.Phone,
                SkinType = customer.SkinType
            });
        }

        [CustomerAuthorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(ProfileViewModel model)
        {
            int customerId = (int)Session["CustomerID"];
            var customer = db.Customers.Find(customerId);
            if (customer == null)
            {
                return RedirectToAction("Logout");
            }

            model.Email = customer.Email; // email không cho sửa

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            customer.FullName = model.FullName.Trim();
            customer.Phone = string.IsNullOrWhiteSpace(model.Phone) ? null : model.Phone.Trim();
            customer.SkinType = string.IsNullOrWhiteSpace(model.SkinType) ? null : model.SkinType;
            db.SaveChanges();

            Session["CustomerName"] = customer.FullName;
            TempData["Success"] = "Đã lưu thông tin cá nhân.";
            return RedirectToAction("Profile");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
