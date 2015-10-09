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
            var lstProducts = db.Products.ToList();
            return View(lstProducts);
        }

        public ActionResult Cookie()
        {
            return View();
        }

        public ActionResult Saltine()
        {
            return View();
        }

        public ActionResult ProductDetail(int ProductId)
        {
            var productDetail = db.Products.SingleOrDefault(n => n.ProductId == ProductId);
            var productMaterial = db.Recipes.Where(p => p.ProductId == productDetail.ProductId).ToList();
            ViewBag.ProductMaterial = productMaterial;
            if (productDetail == null)
            {
                RedirectToAction("Index", "Error");
            }
            return View(productDetail);
        }
	}
}