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
        // Giá trị mặc định khi tạo đơn. Nếu database có CHECK constraint cho các cột này
        // (OrderStatus / PaymentStatus / PaymentMethod) thì sửa lại cho đúng với script SQL của nhóm.
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

            // Lấy lại biến thể từ DB để kiểm tra tồn kho lần cuối
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

            // Bảng Orders không có cột tên/SĐT người nhận nên ghép vào ShippingAddress
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
            db.SaveChanges();   // một lần SaveChanges = một transaction: hoặc lưu hết, hoặc không lưu gì

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

        // Chỉ trả về đơn nếu đúng là của khách đang đăng nhập
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
