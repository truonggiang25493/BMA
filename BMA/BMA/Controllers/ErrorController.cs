using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManageError()
        {
            if (Session["User"] != null)
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
	}
}