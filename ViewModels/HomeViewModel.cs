using System.Collections.Generic;
using nv3.Models;

namespace nv3.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Product> FeaturedProducts { get; set; }
    }
}
