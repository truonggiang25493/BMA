using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class MaterialInOrderViewModel
    {
        public string ProductMaterialName { get; set; }
        public double NeedQuantity { get; set; }
        public double StorageQuantity { get; set; }
        public bool IsEnough { get; set; }
    }
}