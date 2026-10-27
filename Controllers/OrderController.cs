using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using nv3.Attributes;
using nv3.Helpers;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Controllers
{
    [CustomerAuthorize]
    public class OrderController : Controller
    {
        private BeautyCosmeticEntities db = new BeautyCosmeticEntities();

        private int CurrentCustomerId
        {
            get { return (int)Session["CustomerID"]; }
        }

        // ---------- THANH TOÁN ----------
        [HttpGet]
        public ActionResult Checkout()
        {
            var cart = CartHelper.Refresh(db);
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống, không thể thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            var customer = db.Customers.Find(CurrentCustomerId);
            var model = new CheckoutViewModel
            {
                ReceiverName = customer.FullName,
                ReceiverPhone = customer.Phone,
                Items = cart
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Checkout(CheckoutViewModel model)
        {
            var cart = CartHelper.Refresh(db);
            if (!cart.Any())
            {
                TempData["Error"] = "Giỏ hàng đang trống, không thể thanh toán.";
                return RedirectToAction("Index", "Cart");
            }

            model.Items = cart;
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Lấy lại sản phẩm từ DB để kiểm tra tồn kho lần cuối
            var ids = cart.Select(i => i.ProductID).ToList();
            var products = db.Products.Where(p => ids.Contains(p.ProductID)).ToDictionary(p => p.ProductID);

            foreach (var item in cart)
            {
                Product p;
                if (!products.TryGetValue(item.ProductID, out p) || p.Quantity < item.Quantity)
                {
                    TempData["Error"] = "Sản phẩm \"" + item.ProductName + "\" không đủ hàng. Vui lòng kiểm tra lại giỏ.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            // Tạo Order + OrderDetail, trừ tồn kho, cộng số lượng đã bán
            var order = new Order
            {
                CustomerID = CurrentCustomerId,
                OrderDate = DateTime.Now,
                PaymentStatus = "Chưa thanh toán",
                ReceiverName = model.ReceiverName.Trim(),
                ReceiverPhone = model.ReceiverPhone.Trim(),
                ShippingAddress = model.ShippingAddress.Trim()
            };

            decimal total = 0;
            foreach (var item in cart)
            {
                var p = products[item.ProductID];

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductID = p.ProductID,
                    Quantity = item.Quantity,
                    UnitPrice = p.Price
                });

                p.Quantity -= item.Quantity;
                p.SoldQuantity += item.Quantity;
                total += p.Price * item.Quantity;
            }
            order.TotalAmount = total;

            db.Orders.Add(order);
            db.SaveChanges();   // một lần SaveChanges = một transaction: hoặc lưu hết, hoặc không lưu gì

            CartHelper.Clear();
            return RedirectToAction("Success", new { id = order.OrderID });
        }

        public ActionResult Success(int id)
        {
            var order = GetOwnOrder(id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // ---------- LỊCH SỬ GIAO DỊCH ----------
        public ActionResult History()
        {
            int customerId = CurrentCustomerId;
            var orders = db.Orders
                .Where(o => o.CustomerID == customerId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();
            return View(orders);
        }

        public ActionResult Detail(int id)
        {
            var order = GetOwnOrder(id);
            if (order == null) return HttpNotFound();
            return View(order);
        }

        // ---------- MUA LẠI ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Reorder(int id)
        {
            var order = GetOwnOrder(id);
            if (order == null) return HttpNotFound();

            var cart = CartHelper.GetCart();
            var skipped = new List<string>();

            foreach (var d in order.OrderDetails)
            {
                var p = d.Product;
                if (p == null || p.Quantity <= 0)
                {
                    skipped.Add(p == null ? "(sản phẩm đã bị xóa)" : p.ProductName);
                    continue;
                }

                var item = cart.FirstOrDefault(i => i.ProductID == p.ProductID);
                int current = item == null ? 0 : item.Quantity;
                int canAdd = Math.Min(d.Quantity, p.Quantity - current);

                if (canAdd <= 0)
                {
                    skipped.Add(p.ProductName);
                    continue;
                }

                if (item == null)
                {
                    cart.Add(new CartItem
                    {
                        ProductID = p.ProductID,
                        ProductName = p.ProductName,
                        Image = p.Image,
                        Price = p.Price,
                        Quantity = canAdd,
                        Stock = p.Quantity
                    });
                }
                else
                {
                    item.Quantity += canAdd;
                }
            }

            if (skipped.Count > 0)
            {
                TempData["Error"] = "Không thêm được (hết hàng/không đủ tồn): " + string.Join(", ", skipped);
            }
            else
            {
                TempData["Success"] = "Đã thêm các sản phẩm của đơn #" + id + " vào giỏ hàng.";
            }
            return RedirectToAction("Index", "Cart");
        }

        // Chỉ trả về đơn nếu đúng là của khách đang đăng nhập
        private Order GetOwnOrder(int id)
        {
            int customerId = CurrentCustomerId;
            return db.Orders
                .Include(o => o.OrderDetails.Select(d => d.Product))
                .FirstOrDefault(o => o.OrderID == id && o.CustomerID == customerId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
