using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class OtherExpenseController : Controller
    {
        // GET: OtherExpense
        public ActionResult Index()
        {
            try
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
                ViewBag.Title = "Quản lý chi phí khác";
                OtherExpenseBusiness business = new OtherExpenseBusiness();
                List<OtherExpense> resultList = business.GetOtherExpenseList();
                return View(resultList);
            }
            catch (Exception)
            {
                return RedirectToAction("ManageError", "Error");
            }

        }

        // POST
        [HttpPost]
        public int Edit(FormCollection form)
        {
            try
            {
                string idString = form["otherExpenseId"];
                string name = form["otherExpenseName"];
                string amountString = form["otherExpenseAmount"];
                string editTimeString = form["editTime"];
                int otherExpenseType = Convert.ToInt32(form["otherExpenseType"].Trim());
                try
                {
                    int amount = Convert.ToInt32(amountString);
                    int month = DateTime.ParseExact(editTimeString, "MM/yyyy", CultureInfo.InvariantCulture).Month;
                    int year = DateTime.ParseExact(editTimeString, "MM/yyyy", CultureInfo.InvariantCulture).Year;
                    int id = Convert.ToInt32(idString);

                    OtherExpenseBusiness business = new OtherExpenseBusiness();
                    return (business.EditOtherExpense(id, name, amount, month, year, otherExpenseType) ? 1 : 0);
                }
                catch (Exception)
                {
                    return 0;
                }
            }
            catch (Exception)
            {
                return 0;
            }

        }

        // Add Other Expense
        [HttpPost]
        public int AddOtherExpense(FormCollection form)
        {
            try
            {
                string name = form["otherExpenseName"];
                string amountString = form["otherExpenseAmount"];
                string timeType = form["timeType"];
                string timePointString = form["timePoint"];
                string fromTimeString = form["fromTime"];
                string toTimeString = form["toTime"];
                int otherExpenseType = Convert.ToInt32(form["otherExpenseType"].Trim());

                if (timeType.Equals("1"))
                {
                    try
                    {
                        int month = DateTime.ParseExact(timePointString, "MM/yyyy", CultureInfo.InvariantCulture).Month;
                        int year = DateTime.ParseExact(timePointString, "MM/yyyy", CultureInfo.InvariantCulture).Year;
                        int amount = Convert.ToInt32(amountString);

                        OtherExpenseBusiness business = new OtherExpenseBusiness();
                        return (business.AddOtherExpense(name, amount, 1, month, year, null, null, otherExpenseType) ? 1 : 0);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
                else
                {
                    try
                    {
                        DateTime fromTime = DateTime.ParseExact(fromTimeString, "MM/yyyy", CultureInfo.InvariantCulture);
                        DateTime toTime = DateTime.ParseExact(toTimeString, "MM/yyyy", CultureInfo.InvariantCulture);
                        int amount = Convert.ToInt32(amountString);

                        OtherExpenseBusiness business = new OtherExpenseBusiness();
                        return (business.AddOtherExpense(name, amount, 2, null, null, fromTime, toTime, otherExpenseType) ? 1 : 0);
                    }
                    catch (Exception)
                    {
                        return 0;
                    }
                }
            }
            catch (Exception)
            {
                return 0;
            }
            
        }
        // Delete Other expense
        [HttpPost]
        public int DeleteOtherExpense(int id)
        {
            try
            {
                OtherExpenseBusiness business = new OtherExpenseBusiness();
                return (business.DeleteOtherExpense(id) ? 1 : 0);
            }
            catch (Exception)
            {

                return 0;
            }

        }
    }
}