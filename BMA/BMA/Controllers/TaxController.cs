using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class TaxController : Controller
    {
        // GET: Tax
        public ActionResult Index()
        {
            ViewBag.Title = "Báo cáo thuế";
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "taxRepory";
            TaxBusiness taxBusiness = new TaxBusiness();
            List<TaxRate> taxRateList = taxBusiness.GetCurrentTaxRate();
            return View(taxRateList);
        }

        // Change VAT
        [HttpPost]
        public int ChangeVat(FormCollection form)
        {
            string vatRateString = form["vatRate"];
            string beginDateString = form["beginDate"];
            if (!(vatRateString.IsEmpty() || beginDateString.IsEmpty()))
            {
                int vatRate;

                if (!(int.TryParse(vatRateString, out vatRate)))
                {
                    return 0;
                }
                DateTime beginDate;
                DateTime.TryParse(beginDateString, out beginDate);
                if (!(DateTime.TryParse(beginDateString, out beginDate)))
                {
                    return 0;
                }
                TaxBusiness taxBusiness = new TaxBusiness();
                bool rs = taxBusiness.ChangeVat(vatRate, beginDate);
                return rs ? 1 : 0;
            }
            return 0;
        }
    }
}