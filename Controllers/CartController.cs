using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using nv3.Helpers;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Controllers
{
    public class CartController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        public ActionResult Index()
        {
            var cart = CartHelper.Refresh(db);
            return View(cart);
        }

        // Hiện số lượng trên icon giỏ hàng ở _Layout
        [ChildActionOnly]
        public ActionResult Badge()
        {
            int count = CartHelper.Count();
            return Content(count > 99 ? "99+" : count.ToString());
        }

        // id = ProductDetailId (biến thể sản phẩm), không phải ProductId
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(int id, int? quantity, string returnUrl, bool buyNow = false)
        {
            var detail = db.ProductDetails.Include(d => d.Product)
                           .FirstOrDefault(d => d.ProductDetailId == id);
            if (detail == null)
            {
                return HttpNotFound();
            }

            string name = detail.Product.Name + " (" + detail.DetailName + ")";
            int qty = quantity ?? 1;
            if (qty < 1)
            {
                TempData["Error"] = "Số lượng phải từ 1 trở lên.";
                return RedirectBack(returnUrl);
            }

            if (detail.Quantity <= 0)
            {
                TempData["Error"] = "Sản phẩm \"" + name + "\" đã hết hàng.";
                return RedirectBack(returnUrl);
            }

            var cart = CartHelper.GetCart();
            var item = cart.FirstOrDefault(i => i.ProductDetailId == id);
            int current = item == null ? 0 : item.Quantity;

            if (current + qty > detail.Quantity)
            {
                TempData["Error"] = "Chỉ còn " + detail.Quantity + " sản phẩm \"" + name
                                    + "\" (trong giỏ đã có " + current + ").";
                return RedirectBack(returnUrl);
            }

            if (item == null)
            {
                cart.Add(CartHelper.ToCartItem(detail, qty));
            }
            else
            {
                item.Quantity += qty;
            }

            if (buyNow)
            {
                return RedirectToAction("Index");
            }

            TempData["Success"] = "Đã thêm \"" + name + "\" vào giỏ hàng.";
            return RedirectBack(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(int id, int? quantity)
        {
            var detail = db.ProductDetails.Find(id);
            var cart = CartHelper.GetCart();
            var item = cart.FirstOrDefault(i => i.ProductDetailId == id);

            if (item == null || detail == null)
            {
                return RedirectToAction("Index");
            }

            if (quantity == null || quantity < 1)
            {
                TempData["Error"] = "Số lượng phải là số nguyên từ 1 trở lên. Muốn bỏ sản phẩm hãy bấm Xóa.";
            }
            else if (quantity > detail.Quantity)
            {
                TempData["Error"] = "Chỉ còn " + detail.Quantity + " sản phẩm \"" + item.ProductName + "\" trong kho.";
            }
            else
            {
                item.Quantity = quantity.Value;
                TempData["Success"] = "Đã cập nhật số lượng.";
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Remove(int id)
        {
            var cart = CartHelper.GetCart();
            cart.RemoveAll(i => i.ProductDetailId == id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Clear()
        {
            CartHelper.Clear();
            return RedirectToAction("Index");
        }

        private ActionResult RedirectBack(string returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
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
