using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class ProductController : Controller
    {
        BMAEntities db = new BMAEntities();
        //
        // GET: /Product/
        public ActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var lstProducts = db.Products.Where(n=>n.IsActive).ToList().ToPagedList(pageNumber, pageSize);
            return View(lstProducts);
        }

        public ActionResult Cookie(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var lstCookies = db.Products.Where(n => n.Category.CategoryName == "Bánh ngọt" && n.IsActive).ToList().ToPagedList(pageNumber, pageSize);
            return View(lstCookies);
        }

        public ActionResult Saltine(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            var lstSaltine = db.Products.Where(n => n.Category.CategoryName == "Bánh mặn" && n.IsActive).ToList().ToPagedList(pageNumber, pageSize);
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