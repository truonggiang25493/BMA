using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class CustomerCartViewModel
    {
        BMAEntities db = new BMAEntities();
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Price { get; set; }
        public int Quantity { get; set; }
        public int Total {get;set;}

        public CustomerCartViewModel(int id)
        {
            ProductId = id;
            Product product = db.Products.Single(n => n.ProductId == ProductId);
            ProductName = product.ProductName;
            Price = product.ProductStandardPrice;
            Quantity = 0;
        }

        public CustomerCartViewModel()
        {

        }
    }
}