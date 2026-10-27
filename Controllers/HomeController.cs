using System.Linq;
using System.Web.Mvc;
using nv3.Models;
using nv3.ViewModels;

namespace nv3.Controllers
{
    public class HomeController : Controller
    {
        private BeautyCosmeticEntities db = new BeautyCosmeticEntities();

        public ActionResult Index()
        {
            var model = new HomeViewModel
            {
                Categories = db.Categories.OrderBy(c => c.CategoryID).ToList(),
                // Chỉ lấy sản phẩm còn hàng, ưu tiên bán chạy
                FeaturedProducts = db.Products
                    .Where(p => p.Quantity > 0)
                    .OrderByDescending(p => p.SoldQuantity)
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
