using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class SupplierViewModel
    {
        public int SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierPhoneNumer { get; set; }
        public string SupplierEmail { get; set; }
        public string SupplierTaxCode { get; set; }
        public bool IsActive { get; set; }
    }
}