using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class CartViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int StandardPrice { get; set; }
        public int RealPrice { get; set; }
        public int Quantity { get; set; }
    }
}