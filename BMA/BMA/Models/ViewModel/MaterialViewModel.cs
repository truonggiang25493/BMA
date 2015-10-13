using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class MaterialViewModel
    {
        public int ProductMaterialId { get; set; }
        public string ProductMaterialName { get; set; }
        public int NeedQuantity { get; set; }
        public int StorageQuantity { get; set; }
        public bool IsEnough { get; set; }
        public string Unit { get; set; }
    }
}