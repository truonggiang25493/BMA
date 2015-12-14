using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class TaxRateViewModel
    {
        public List<TaxRate> VatTaxList { get; set; }
        public List<TaxRate> ExciseTaxList { get; set; }
        public List<TaxRate> CurrentTaxList { get; set; } 
    }
}