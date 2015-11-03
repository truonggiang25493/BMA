using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BMA.Controllers
{
    public class ProcedureController : Controller
    {
        //
        // GET: /Procedure/
        public ActionResult Index()
        {
            ViewBag.Show = "procedure";
            return View();
        }
	}
}