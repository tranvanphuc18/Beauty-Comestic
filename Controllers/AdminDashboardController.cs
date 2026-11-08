using System;
using System.Linq;
using System.Web.Mvc;
using nv3.Models;

namespace nv3.Controllers
{
    public class AdminDashboardController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        private bool CheckAdmin() => Session["Role"] != null && Session["Role"].ToString() == "Admin";

        public ActionResult Index()
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            ViewBag.TotalProducts = db.Products.Count();
            ViewBag.TotalCategories = db.Categories.Count();
            ViewBag.TotalCustomers = db.Users.Count(u => u.Role == "Customer");
            ViewBag.TotalOrders = db.Orders.Count();

            // Doanh thu thực thu: CHỈ tính các đơn đã HOÀN THÀNH (Completed) VÀ ĐÃ THANH TOÁN (Paid)
            ViewBag.TotalRevenue = db.Orders
                                        .Where(o => o.OrderStatus == "Completed" && o.PaymentStatus == "Paid")
                                        .Select(o => (decimal?)o.TotalAmount)
                                        .Sum() ?? 0;

            ViewBag.PendingOrders = db.Orders.Count(o => o.OrderStatus == "Pending");

            var recentOrders = db.Orders
                                 .Include("User")
                                 .OrderByDescending(o => o.OrderDate)
                                 .Take(5)
                                 .ToList();
            ViewBag.RecentOrders = recentOrders;

            var lowStock = db.ProductDetails
                             .Include("Product")
                             .Where(pd => pd.Quantity <= 5)
                             .OrderBy(pd => pd.Quantity)
                             .Take(10)
                             .ToList();
            ViewBag.LowStockItems = lowStock;

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
