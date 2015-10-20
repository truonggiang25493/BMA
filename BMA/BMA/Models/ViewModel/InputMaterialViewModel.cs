using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class InputMaterialViewModel
    {
        public int InputMaterialId { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public int ImportQuantity { get; set; }
        public int RemainQuantity { get; set; }
        public double InputMaterialPrice { get; set; }
        public DateTime ImportDate { get; set; }
        public DateTime InputMaterialExpiryDate { get; set; }
        public string InputMaterialNote { get; set; }
        public int InputBillId { get; set; }
        public int ProductMaterialId { get; set; }
        public bool IsActive { get; set; }
    }
}