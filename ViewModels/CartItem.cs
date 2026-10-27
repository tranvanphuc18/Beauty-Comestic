using System;

namespace nv3.ViewModels
{
    [Serializable]
    public class CartItem
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
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
