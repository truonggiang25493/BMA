using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class MaterialInfoForProductListViewModel
    {
        public bool IsEnough { get; set; }
        public int MaterialCost { get; set; }
        public int OrderId { get; set; }
        public List<MaterialViewModel> MaterialList { get; set; }
    }
}