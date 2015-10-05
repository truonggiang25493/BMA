using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMA.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
	}
}