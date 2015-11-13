using System;
using System.Collections.Generic;
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
            ViewBag.Title = "Quản lý chi phí khác";
            OtherExpenseBusiness business = new OtherExpenseBusiness();
            List<OtherExpense> resultList = business.GetOtherExpenseList();
            return View(resultList);
        }
    }
}