using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class ExportFrom
    {
        // Info from OutputMaterial table
        public int ProductMaterialId { get; set; }
        public int OrderItemId { get; set; }
        // Info from InputBill table
        public int InputBillId { get; set; }
        // Additional attribute
        public int ExportFromQuantity { get; set; }

    }
}