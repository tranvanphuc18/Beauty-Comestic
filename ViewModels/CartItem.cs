using System;

namespace nv3.ViewModels
{
    [Serializable]
    public class CartItem
    {
        public int ProductDetailId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string DetailName { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int Stock { get; set; }

        public decimal Total
        {
            get { return Price * Quantity; }
        }
    }
}
