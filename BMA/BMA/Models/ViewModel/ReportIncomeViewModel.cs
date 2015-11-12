using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class ReportIncomeViewModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int RevenueAmount { get; set; }
        public int CompletedOrderValue { get; set; }
        public List<Order> CompletedOrderList { get; set; }
        public int CanceledOrderValue { get; set; }
        public List<Order> CanceledOrderList { get; set; }
        public int MaterialExpense { get; set; }
        public List<ReportProductMaterial> ProductMaterialList { get; set; }
        public int Income { get; set; }
    }

    public class ReportProductMaterial
    {
        public string ProductMaterialName { get; set; }
        public int ProductMaterialQuantity { get; set; }
        public int ProductMaterialAmount { get; set; }
        public string ProductMaterialUnit { get; set; }
    }
}