using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class ReportProductIncomeViewModel
    {
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int TotalIncome { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<ReportProductIncomePeriod> ReportProductIncomePeriodList { get; set; }
    }

    public class ReportProductIncomePeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int RevenueAmount { get; set; }
        public List<ReportProductOrder> OrderList { get; set; }
        public int MaterialExpense { get; set; }
        public List<ReportProductMaterial> ProductMaterialList { get; set; }
        public int Income { get; set; }
    }

    public class ReportProductOrder
    {
        public string OrderCode { get; set; }
        public int Quantity { get; set; }
        public int Amount { get; set; }
        public int DiscountAmount { get; set; }
        public int Revenue { get; set; }
    }
}