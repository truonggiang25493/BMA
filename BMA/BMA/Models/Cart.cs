using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models
{
    public class Cart
    {
        BMAEntities db = new BMAEntities();
        public int productId { get; set; }
        public string productName { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public double Total
        {
            get { return price * quantity; }
        }

        public Cart(int id)
        {
            productId = id;
            Product product = db.Products.Single(n => n.ProductId == productId);
            productName = product.ProductName;
            price = product.ProductStandardPrice;
            quantity = 1;
        }
    }
}