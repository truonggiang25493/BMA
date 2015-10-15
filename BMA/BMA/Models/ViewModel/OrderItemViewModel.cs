using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class OrderItemViewModel
    {
        public OrderItem OrderItem { get; set; }
        public string ProductName { get; set; }
        public List<MaterialViewModel> MaterialList { get; set; }
    }
}