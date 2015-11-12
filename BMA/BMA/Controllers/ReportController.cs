using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult ReviewIncomeByTime(string start, string end)
        {
            ViewBag.Title = "Báo cáo thu nhập";
            DateTime startDate;
            DateTime endDate;
            if (start == null || end == null)
            {
                startDate = DateTime.Parse("2015/10/25");
                endDate = DateTime.Parse("2015/11/14");
            }
            else
            {
                startDate = DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                endDate = DateTime.ParseExact(end,"dd/MM/yyyy",CultureInfo.InvariantCulture);
            }

            ReportBusiness business = new ReportBusiness();

            List<sp_GetIncomeWeekly_Result> result = business.GetIncomeWeeklyStore(startDate, endDate);
            return View(result);
        }

        // 
        public ActionResult ReviewIncomeByTimeDetail(DateTime startDate, DateTime endDate)
        {
            ReportBusiness reportBusiness = new ReportBusiness();
            ReportIncomeViewModel reportIncome = reportBusiness.ReviewIncomeByTimeDetail(startDate, endDate);
            return View(reportIncome);
        }

        public ActionResult ReviewRevenueByProduct()
        {
            return View();
        }
    }
}