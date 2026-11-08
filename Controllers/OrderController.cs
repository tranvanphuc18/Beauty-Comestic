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
        private const string DefaultOrderStatus = "Pending";
        private const string DefaultPaymentStatus = "Unpaid";
        private const string DefaultPaymentMethod = "COD";

        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

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

            var user = db.Users.Find(CurrentCustomerId);
            var model = new CheckoutViewModel
            {
                ReceiverName = user.FullName,
                ReceiverPhone = user.Phone,
                ShippingAddress = user.Address,
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

            var ids = cart.Select(i => i.ProductDetailId).ToList();
            var details = db.ProductDetails
                            .Where(d => ids.Contains(d.ProductDetailId))
                            .ToDictionary(d => d.ProductDetailId);

            foreach (var item in cart)
            {
                ProductDetail d;
                if (!details.TryGetValue(item.ProductDetailId, out d) || d.Quantity < item.Quantity)
                {
                    TempData["Error"] = "Sản phẩm \"" + item.ProductName + "\" không đủ hàng. Vui lòng kiểm tra lại giỏ.";
                    return RedirectToAction("Index", "Cart");
                }
            }

            string shipping = model.ReceiverName.Trim() + " - " + model.ReceiverPhone.Trim()
                              + " - " + model.ShippingAddress.Trim();

            var order = new Order
            {
                UserId = CurrentCustomerId,
                OrderDate = DateTime.Now,
                OrderStatus = DefaultOrderStatus,
                PaymentMethod = DefaultPaymentMethod,
                PaymentStatus = DefaultPaymentStatus,
                ShippingAddress = shipping
            };

            decimal total = 0;
            foreach (var item in cart)
            {
                var d = details[item.ProductDetailId];
                decimal unitPrice = ProductHelper.UnitPrice(d);
                decimal lineTotal = unitPrice * item.Quantity;

                order.OrderDetails.Add(new OrderDetail
                {
                    ProductDetailId = d.ProductDetailId,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal
                });

                d.Quantity -= item.Quantity;
                d.SoldQuantity += item.Quantity;
                total += lineTotal;
            }
            order.TotalAmount = total;

            db.Orders.Add(order);
            db.SaveChanges();

            CartHelper.Clear();
            return RedirectToAction("Success", new { id = order.OrderId });
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
            int userId = CurrentCustomerId;
            var orders = db.Orders
                .Include(o => o.OrderDetails.Select(od => od.ProductDetail.Product))
                .Where(o => o.UserId == userId)
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

        // ---------- KHÁCH HÀNG HỦY ĐƠN ----------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cancel(int id)
        {
            var order = GetOwnOrder(id);
            if (order == null) return HttpNotFound();

            if (order.OrderStatus != "Pending")
            {
                TempData["Error"] = "Đơn hàng #" + id + " chỉ có thể hủy khi đang ở trạng thái 'Chờ duyệt' (Pending).";
                return RedirectToAction("Detail", new { id = id });
            }

            order.OrderStatus = "Cancelled";

            // Hoàn trả lại số lượng tồn kho và giảm số lượng đã bán
            foreach (var line in order.OrderDetails)
            {
                if (line.ProductDetail != null)
                {
                    line.ProductDetail.Quantity += line.Quantity;
                    line.ProductDetail.SoldQuantity = Math.Max(0, line.ProductDetail.SoldQuantity - line.Quantity);
                }
            }

            db.SaveChanges();
            TempData["Success"] = "Đã hủy đơn hàng #" + id + " thành công. Số lượng sản phẩm đã được hoàn lại vào kho.";
            return RedirectToAction("History");
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

            foreach (var line in order.OrderDetails)
            {
                var d = line.ProductDetail;
                string name = d == null ? "(sản phẩm đã bị xóa)" : d.Product.Name + " (" + d.DetailName + ")";

                if (d == null || d.Quantity <= 0)
                {
                    skipped.Add(name);
                    continue;
                }

                var item = cart.FirstOrDefault(i => i.ProductDetailId == d.ProductDetailId);
                int current = item == null ? 0 : item.Quantity;
                int canAdd = Math.Min(line.Quantity, d.Quantity - current);

                if (canAdd <= 0)
                {
                    skipped.Add(name);
                    continue;
                }

                if (item == null)
                {
                    cart.Add(CartHelper.ToCartItem(d, canAdd));
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

        private Order GetOwnOrder(int id)
        {
            int userId = CurrentCustomerId;
            return db.Orders
                .Include(o => o.OrderDetails.Select(d => d.ProductDetail.Product))
                .FirstOrDefault(o => o.OrderId == id && o.UserId == userId);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
