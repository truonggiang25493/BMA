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
            var lstCookies = db.Products.Where(n => n.Category.CategoryName == "Bánh ngọt").ToList();
            return View(lstCookies);
        }

        public ActionResult Saltine()
        {
            var lstSaltine = db.Products.Where(n => n.Category.CategoryName == "Bánh mặn").ToList();
            return View(lstSaltine);
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