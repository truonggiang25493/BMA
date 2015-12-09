using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class AllProductInfoViewModel
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public string MaterialUnit { get; set; }
        public int MaterialQuantity { get; set; }
    }
}