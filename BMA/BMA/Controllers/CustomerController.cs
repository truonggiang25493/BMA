using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class CustomerController : Controller
    {
        private BMAEntities db = new BMAEntities();
        // GET: Customer
        public ActionResult GetCustomerPartialView()
        {
            List<Customer> customerList = db.Customers.Where(m => m.IsActive).ToList();
            return PartialView(customerList);
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, string returnUrl)
        {
            return RedirectToAction(returnUrl);
        }
    }
}