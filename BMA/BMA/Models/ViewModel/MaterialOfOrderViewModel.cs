using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class MaterialOfOrderViewModel
    {
        public int MaterialId { get; set; }
        public string MaterialName { get; set; }
        public int TotalQuantity { get; set; }
        public List<OutputMaterial> OutputMaterialList { get; set; }
    }
}