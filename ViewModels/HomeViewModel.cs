using System.Collections.Generic;
using nv3.Models;

namespace nv3.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> FeaturedProducts { get; set; }

        // id sản phẩm -> "hot" (bán chạy) | "love" (được ưa chuộng); không có key = không gắn nhãn
        public Dictionary<int, string> PickBadges { get; set; }
    }
}
