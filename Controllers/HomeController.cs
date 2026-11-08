using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Controllers
{
    public class HomeController : Controller
    {
        private BeautyCosmeticDBEntities db = new BeautyCosmeticDBEntities();

        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Categories = db.Categories.OrderBy(c => c.CategoryId).ToList(),
                // Lấy sản phẩm nổi bật theo số lượng đã bán (kể cả hết hàng vẫn hiển thị badge hết hàng)
                FeaturedProducts = db.Products
                    .Include(p => p.ProductDetails)
                    .OrderByDescending(p => p.ProductDetails.Sum(d => (int?)d.SoldQuantity) ?? 0)
                    .Take(8)
                    .ToList()
            };
            return View(model);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
