using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models
{
    public class Cart
    {
        BMAEntities db = new BMAEntities();
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total {get;set;}
        //{
        //    get { return price * quantity; }
        //    set;
        //}

        public Cart(int id)
        {
            ProductId = id;
            Product product = db.Products.Single(n => n.ProductId == ProductId);
            ProductName = product.ProductName;
            Price = product.ProductStandardPrice;
            Quantity = 0;
        }

        public Cart()
        {

        }
    }
}