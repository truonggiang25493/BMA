using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class CustomerRevenueReport
    {
        public string Username { get; set; }
        public string Fullname { get; set; }
        public int TotalRevenue { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int StartMonth { get; set; }
        public int EndMonth { get; set; }
        public int StartYear { get; set; }
        public int EndYear { get; set; }
        public List<CustomerRevenueReportPeriod> CustomerRevenueReportPeriodList { get; set; }
    }

    public class CustomerRevenueReportPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int RevenueAmount { get; set; }
        public int CompletedOrderValue { get; set; }
        public List<Order> CompletedOrderList { get; set; }
        public int CanceledOrderValue { get; set; }
        public List<Order> CanceledOrderList { get; set; }
    }
}