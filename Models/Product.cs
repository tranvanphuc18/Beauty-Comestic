using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyPham_Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }

        // Loại da phù hợp: oily, dry, combination, sensitive
        public string[] SkinTypes { get; set; }
    }
}
