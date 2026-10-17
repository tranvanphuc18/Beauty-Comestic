using MyPham_Models;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace nv3.Controllers
{
    public class ProductController : Controller
    {
        private static List<Product> products = new List<Product>
        {
            new Product
            {
                Id = 1,
                Name = "Sữa rửa mặt dịu nhẹ",
                Brand = "CeraVe",
                Price = 250000,
                Image = "ceraVe.jpg",
                Description = "Sữa rửa mặt dịu nhẹ giúp làm sạch da."
            },
            new Product
            {
                Id = 2,
                Name = "Toner cấp ẩm",
                Brand = "Klairs",
                Price = 320000,
                Image = "klairs.jpg",
                Description = "Toner giúp cấp ẩm và làm dịu da."
            },
            new Product
            {
                Id = 3,
                Name = "Serum Vitamin C",
                Brand = "Melano CC",
                Price = 280000,
                Image = "melano.jpg",
                Description = "Serum chăm sóc da với Vitamin C."
            },
            new Product
            {
                Id = 4,
                Name = "Kem dưỡng ẩm phục hồi",
                Brand = "La Roche-Posay",
                Price = 450000,
                Image = "larocheposay.jpg",
                Description = "Kem dưỡng ẩm hỗ trợ phục hồi da."
            },
            new Product
            {
                Id = 5,
                Name = "Kem chống nắng",
                Brand = "Anessa",
                Price = 400000,
                Image = "anessa.jpg",
                Description = "Kem chống nắng dùng hằng ngày."
            },
            new Product
            {
                Id = 6,
                Name = "Son cao cấp",
                Brand = "YSL",
                Price = 950000,
                Image = "ysl.jpg",
                Description = "Son môi cao cấp với thiết kế sang trọng."
            }
        };

        public ActionResult Index()
        {
            return View(products);
        }

        public ActionResult Detail(int id)
        {
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }
    }
}