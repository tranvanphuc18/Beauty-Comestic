using System;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity;
using nv3.Models;

namespace nv3.Controllers
{
    public class AdminOrderController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();
        private const int PAGE_SIZE = 15; 

        private bool CheckAdmin() => Session["Role"] != null && Session["Role"].ToString() == "Admin";

      
        public ActionResult Index(string searchCustomer = "", string paymentStatus = "", string orderStatus = "",
                                  string fromDate = "", string toDate = "", int page = 1)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var orders = db.Orders.Include(o => o.User).AsQueryable();

          
            if (!string.IsNullOrEmpty(searchCustomer))
            {
                orders = orders.Where(o => o.User.FullName.Contains(searchCustomer) || o.User.Email.Contains(searchCustomer));
            }

           
            if (!string.IsNullOrEmpty(paymentStatus))
            {
                orders = orders.Where(o => o.PaymentStatus == paymentStatus);
            }

           
            if (!string.IsNullOrEmpty(orderStatus))
            {
                orders = orders.Where(o => o.OrderStatus == orderStatus);
            }

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime dtFrom = DateTime.Parse(fromDate);
                orders = orders.Where(o => o.OrderDate >= dtFrom);
            }
            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime dtTo = DateTime.Parse(toDate).AddDays(1);
                orders = orders.Where(o => o.OrderDate < dtTo);
            }

         
            int totalOrders = orders.Count();
            int totalPages = (int)Math.Ceiling((double)totalOrders / PAGE_SIZE);
            if (totalPages == 0) totalPages = 1;
            if (page < 1) page = 1;
            if (page > totalPages) page = totalPages;

            var pagedOrders = orders.OrderByDescending(o => o.OrderDate)
                                    .Skip((page - 1) * PAGE_SIZE)
                                    .Take(PAGE_SIZE)
                                    .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SearchCustomer = searchCustomer;
            ViewBag.PaymentStatus = paymentStatus;
            ViewBag.OrderStatus = orderStatus;
            ViewBag.FromDate = fromDate;
            ViewBag.ToDate = toDate;
            ViewBag.TotalCount = totalOrders;

            return View(pagedOrders);
        }

        public ActionResult Detail(int id)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var order = db.Orders
                          .Include(o => o.User)
                          .Include(o => o.OrderDetails.Select(od => od.ProductDetail.Product))
                          .FirstOrDefault(o => o.OrderId == id);

            if (order == null) return HttpNotFound();
            return View(order);
        }

      
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int orderId, string orderStatus, string paymentStatus)
        {
            if (!CheckAdmin()) return RedirectToAction("Login", "Account");

            var order = db.Orders.Find(orderId);
            if (order != null)
            {
                order.OrderStatus = orderStatus;
                if (!string.IsNullOrEmpty(paymentStatus))
                {
                    order.PaymentStatus = paymentStatus;
                }
                db.SaveChanges();
                TempData["Success"] = "Cập nhật trạng thái đơn hàng #" + orderId + " thành công!";
            }

            return RedirectToAction("Detail", new { id = orderId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}