using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;

namespace BMA.Controllers
{
    public class ContactController : Controller
    {
        //
        // GET: /Contact/
        public ActionResult Index()
        {
            try
            {
                ContactBusiness cb = new ContactBusiness();
                ViewBag.Show = "procedure";
                ViewBag.staffInfor = cb.GetStaff();
                ViewBag.staffPhone = cb.staffPhone(cb.GetStaff().UserId);
                ViewBag.storeOwner = cb.StoreOwner();
                return View();
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }        
        }
	}
}