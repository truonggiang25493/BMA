using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class ProductController : Controller
    {
        BMAEntities db = new BMAEntities();
        //
        // GET: /Product/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cookie()
        {
            return View();
        }

        public ActionResult Saltine()
        {
            return View();
        }

        public ActionResult ProductDetail()
        {
            return View();
        }
	}
}