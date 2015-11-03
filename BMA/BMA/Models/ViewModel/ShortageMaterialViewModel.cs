using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class ShortageMaterialViewModel
    {
        public int ProductMaterialId { get; set; }
        public string ProductMaterialName { get; set; }
        public string ProductMaterialUnit { get; set; }
        public int CurrentQuantity { get; set; }
        public int NeedQuantity { get; set; }
        public List<ShortageMaterialInOrderViewModel> ShortageMaterialInOrderList { get; set; }
    }

    public class ShortageMaterialInOrderViewModel
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public int NeedQuantity { get; set; }
    }
}