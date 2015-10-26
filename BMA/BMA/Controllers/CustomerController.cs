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
    public class CustomerController : Controller
    {
        private BMAEntities db = new BMAEntities();

        // GET: Customer
        public ActionResult GetCustomerPartialView()
        {
            CustomerBusiness customerBusiness = new CustomerBusiness();
            List<Customer> customerList = customerBusiness.GetCustomerList();
            return PartialView(customerList);
        }

        [HttpPost]
        public ActionResult Create(FormCollection form, string returnUrl)
        {
            return RedirectToAction(returnUrl);
        }

        [HttpPost]
        public int AddCustomerForOrder(FormCollection form)
        {
            string customerName = form["customerName"];
            string orderIdString = form["orderId"];
            string username = form["username"];
            string email = form["customerEmail"];
            string customerAddress = form["customerAddress"];
            string customerPhoneNumber = form["customerPhoneNumber"];
            string customerTaxCode = form["customerTaxCode"];
            if (
                !(customerName.IsEmpty() || orderIdString.IsEmpty() || username.IsEmpty() || email.IsEmpty() ||
                  customerAddress.IsEmpty() || customerPhoneNumber.IsEmpty() || customerTaxCode.IsEmpty()))
            {
                int orderId = Convert.ToInt32(orderIdString);
                CustomerBusiness customerBusiness = new CustomerBusiness();
                bool rs = customerBusiness.AddCustomerForOrder(username, email, customerName, customerAddress,
                    customerPhoneNumber,customerTaxCode, orderId);
                return rs ? 1 : 0;
            }
            return 0;
        }
    }
}