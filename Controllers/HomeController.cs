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
                // Chỉ lấy sản phẩm còn hàng (có ít nhất 1 biến thể còn tồn), ưu tiên bán chạy
                FeaturedProducts = db.Products
                    .Include(p => p.ProductDetails)
                    .Where(p => p.ProductDetails.Any(d => d.Quantity > 0))
                    .OrderByDescending(p => p.ProductDetails.Sum(d => d.SoldQuantity))
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
