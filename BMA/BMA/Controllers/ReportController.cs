using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Common;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class ReportController : Controller
    {
        #region Report Income by Time
        public ActionResult ReviewIncomeByTime(string start, string end)
        {
            // Check autherization
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "StoreInfor");
            }
            ViewBag.Title = "Báo cáo thu nhập";
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "incomeReport";
            return View();
        }
        public ActionResult ReviewByTimePartialView(string start, string end, int? type)
        {
            ReportBusiness business = new ReportBusiness();
            if (type == null)
            {
                type = 1;
            }
            if (type == 1)
            {
                DateTime startDate;
                DateTime endDate;
                if (start == null || end == null)
                {
                    startDate = DateTime.Now.FirstDayOfWeek().AddDays(-7);
                    endDate = DateTime.Now.LastDayOfWeek();
                }
                else
                {
                    startDate = DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }


                List<sp_GetIncomeWeekly_Result> result = business.GetIncomeWeeklyStore(startDate, endDate);
                return PartialView("ReviewIncomeWeeklyPartialView", result);
            }
            if (type == 2)
            {
                DateTime startMonth = DateTime.ParseExact(start, "MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endMonth = DateTime.ParseExact(end, "MM/yyyy", CultureInfo.InvariantCulture);

                List<sp_GetIncomeMonthly_Result> result = business.GetIncomeMonthlyStore(startMonth.Month,
                    startMonth.Year, endMonth.Month, endMonth.Year);
                return PartialView("ReviewIncomeMonthlyPartialView", result);

            }
            if (type == 3)
            {
                int startYear = DateTime.ParseExact(start, "yyyy", CultureInfo.InvariantCulture).Year;
                int endYear = DateTime.ParseExact(end, "yyyy", CultureInfo.InvariantCulture).Year;
                List<sp_GetIncomeYearly_Result> result = business.GetIncomeYearlyStore(startYear, endYear);
                return PartialView("ReviewIncomeYearlyPartialView", result);
            }

            return null;
        }

        public ActionResult ReviewIncomeWeeklyDetail(DateTime startDate, DateTime endDate)
        {
            // Check autherization
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "StoreInfor");
            }
            ViewBag.Title = "Lợi nhuận từ " + startDate.ToString("dd/MM/yyyy") + " đến " + endDate.ToString("dd/MM/yyyy");
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "incomeReport";
            ReportBusiness reportBusiness = new ReportBusiness();
            ReportIncomeViewModel reportIncome = reportBusiness.ReviewIncomeWeeklyDetail(startDate, endDate);
            return View(reportIncome);
        }

        public ActionResult ReviewIncomeMonthlyDetail(int month, int year)
        {
            // Check autherization
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "StoreInfor");
            }
            ViewBag.Title = "Lợi nhuận tháng " + month + "/" + year;
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "incomeReport";
            ReportBusiness reportBusiness = new ReportBusiness();
            ReportIncomeViewModel reportIncome = reportBusiness.ReviewIncomeMonthlyDetail(month, year);
            return View(reportIncome);
        }
        public ActionResult ReviewIncomeYearlyDetail(int year)
        {
            // Check autherization
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "StoreInfor");
            }
            ViewBag.Title = "Lợi nhuận năm " + year;
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "incomeReport";
            ReportBusiness reportBusiness = new ReportBusiness();
            ReportIncomeViewModel reportIncome = reportBusiness.ReviewIncomeYearlyDetail(year);
            return View(reportIncome);
        }
        #endregion

        #region Review by type of goods

        public ActionResult ReviewRevenueByProduct()
        {
            ViewBag.Title = "Thống kê thu nhập theo sản phẩm";
            ViewBag.TreeViewMenu = "incomeProduct";
            ViewBag.TreeView = "report";
            return View();
        }
        [HttpPost]
        public ActionResult ReviewIncomePerProductPartialView(string start, string end, int? type)
        {
            ReportBusiness business = new ReportBusiness();

            if (type == null)
            {
                type = 1;
            }
            if (type == 1)
            {
                DateTime startDate;
                DateTime endDate;
                if (start == null || end == null)
                {
                    startDate = DateTime.Now.FirstDayOfWeek().AddDays(-7);
                    endDate = DateTime.Now;
                }
                else
                {
                    startDate = DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }


                List<sp_GetTop10IncomeOfProductWeekly_Result> result = business.GetTop10ProductIncomeWeekly(startDate, endDate);
                return PartialView("Top10ProductIncomeWeeklyPartialView", result);
            }
            else if (type == 2)
            {
                DateTime startDate = DateTime.ParseExact(start, "MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(end, "MM/yyyy", CultureInfo.InvariantCulture);

                List<sp_GetTop10IncomeOfProductMonthly_Result> result =
                    business.GetTop10ProductIncomeMonthly(startDate, endDate);

                return PartialView("Top10ProductIncomeMonthlyPartialView", result);
            }
            else if (type == 3)
            {
                DateTime startDate = DateTime.ParseExact(start, "yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(end, "yyyy", CultureInfo.InvariantCulture);

                List<sp_GetTop10IncomeOfProductYearly_Result> result =
                    business.GetTop10ProductIncomeYearly(startDate, endDate);

                return PartialView("Top10ProductIncomeYearlyPartialView", result);
            }
            return null;
        }

        public ActionResult Top10ProductIncomeDetail(string query)
        {
            if (!query.IsEmpty())
            {
                ViewBag.Title = "Chi tiết lợi nhuận từ sản phẩm";
                string[] tempStrings = query.Split(';');

                int id = Convert.ToInt32(tempStrings[0].Trim());

                if (tempStrings[1].Length == 23)
                {
                    DateTime startDate = DateTime.ParseExact(tempStrings[1].Substring(0, 10), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(tempStrings[1].Substring(13, 10), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);

                    ReportBusiness business = new ReportBusiness();

                    ReportProductIncomeViewModel result = business.GetReportProductIncomeViewModel(id, startDate, endDate, null, null,
                        null, null);

                    return View("Top10ProductIncomeWeeklyDetail", result);
                }
                else if (tempStrings[1].Length == 17)
                {
                    DateTime startDate = DateTime.ParseExact(tempStrings[1].Substring(0, 7), "MM/yyyy",
                        CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(tempStrings[1].Substring(10, 7), "MM/yyyy",
                        CultureInfo.InvariantCulture);

                    ReportBusiness business = new ReportBusiness();

                    ReportProductIncomeViewModel result = business.GetReportProductIncomeViewModel(id, null, null, startDate.Month,
                        startDate.Year, endDate.Month, endDate.Year);

                    return View("Top10ProductIncomeMonthlyDetail", result);

                }
                else if (tempStrings[1].Length == 11)
                {
                    int startYear = Convert.ToInt32(tempStrings[1].Substring(0, 4));
                    int endYear = Convert.ToInt32(tempStrings[1].Substring(7, 4));

                    ReportBusiness business = new ReportBusiness();

                    ReportProductIncomeViewModel result = business.GetReportProductIncomeViewModel(id, null, null, null, startYear,
                        null, endYear);

                    return View("Top10ProductIncomeYearlyDetail", result);

                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion

        #region Review Revenue per Customer

        public ActionResult ReviewRevenuePerCustomer()
        {
            ViewBag.Title = "Thống kê doanh thu theo khách hàng";
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "customerRevenue";

            return View();
        }

        [HttpPost]
        public ActionResult ReviewRevenuePerCustomerPartialView(string start, string end, int? type)
        {
            ReportBusiness business = new ReportBusiness();

            if (type == null)
            {
                type = 1;
            }
            if (type == 1)
            {
                DateTime startDate;
                DateTime endDate;
                if (start == null || end == null)
                {
                    startDate = DateTime.Now.FirstDayOfWeek().AddDays(-7);
                    endDate = DateTime.Now;
                }
                else
                {
                    startDate = DateTime.ParseExact(start, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    endDate = DateTime.ParseExact(end, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                }


                List<sp_GetTop10CustomerRevenueWeekly_Result> result = business.GetTop10CustomerRevenueWeekly(startDate, endDate);
                return PartialView("Top10CustomerRevenueWeeklyPartialView", result);
            }
            else if (type == 2)
            {
                DateTime startDate = DateTime.ParseExact(start, "MM/yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(end, "MM/yyyy", CultureInfo.InvariantCulture);

                List<sp_GetTop10CustomerRevenueMonthly_Result> result =
                    business.GetTop10CustomerRevenueMonthly(startDate, endDate);

                return PartialView("Top10CustomerRevenueMonthlyPartialView", result);
            }
            else if (type == 3)
            {
                DateTime startDate = DateTime.ParseExact(start, "yyyy", CultureInfo.InvariantCulture);
                DateTime endDate = DateTime.ParseExact(end, "yyyy", CultureInfo.InvariantCulture);

                List<sp_GetTop10CustomerRevenueYearly_Result> result =
                    business.GetTop10CustomerRevenueYearly(startDate, endDate);

                return PartialView("Top10CustomerRevenueYearlyPartialView", result);
            }

            return null;
        }

        public ActionResult Top10CustomerRevenueDetail(string query)
        {
            if (!query.IsEmpty())
            {
                ViewBag.Title = "Chi tiết doanh thu từ khách hàng";
                string[] tempStrings = query.Split(';');

                int id = Convert.ToInt32(tempStrings[0].Trim());
                if (tempStrings[1].Length == 23)
                {
                    DateTime startDate = DateTime.ParseExact(tempStrings[1].Substring(0, 10), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(tempStrings[1].Substring(13, 10), "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);

                    ReportBusiness business = new ReportBusiness();

                    CustomerRevenueReport result = business.GetCustomerRevenueReport(id, startDate, endDate, null, null,
                        null, null);

                    return View("Top10CustomerRevenueWeeklyDetail", result);
                }
                else if (tempStrings[1].Length == 17)
                {
                    DateTime startDate = DateTime.ParseExact(tempStrings[1].Substring(0, 7), "MM/yyyy",
                        CultureInfo.InvariantCulture);
                    DateTime endDate = DateTime.ParseExact(tempStrings[1].Substring(10, 7), "MM/yyyy",
                        CultureInfo.InvariantCulture);

                    ReportBusiness business = new ReportBusiness();

                    CustomerRevenueReport result = business.GetCustomerRevenueReport(id, null, null, startDate.Month,
                        startDate.Year, endDate.Month, endDate.Year);

                    return View("Top10CustomerRevenueMonthlyDetail", result);

                }
                else if (tempStrings[1].Length == 11)
                {
                    int startYear = Convert.ToInt32(tempStrings[1].Substring(0, 4));
                    int endYear = Convert.ToInt32(tempStrings[1].Substring(7, 4));

                    ReportBusiness business = new ReportBusiness();

                    CustomerRevenueReport result = business.GetCustomerRevenueReport(id, null, null, null, startYear,
                        null, endYear);

                    return View("Top10CustomerRevenueYearlyDetail", result);

                }
            }
            return RedirectToAction("Index", "Home");
        }

        #endregion







    }
}