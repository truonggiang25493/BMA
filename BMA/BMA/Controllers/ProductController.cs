using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using BMA.Models;
using BMA.Business;

namespace BMA.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ProductBusiness pb = new ProductBusiness();
            var lstProduct = pb.GetProduct().ToPagedList(pageNumber, pageSize); ;
            return View(lstProduct);
        }

        public ActionResult Cookie(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ProductBusiness pb = new ProductBusiness();
            var lstCookies = pb.GetCookie().ToPagedList(pageNumber, pageSize);
            return View(lstCookies);
        }

        public ActionResult Saltine(int? page)
        {
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ProductBusiness pb = new ProductBusiness();
            var lstSaltine = pb.GetSaltine().ToPagedList(pageNumber, pageSize);
            return View(lstSaltine);
        }

        public ActionResult ProductDetail(int ProductId)
        {
            ProductBusiness pb = new ProductBusiness();
            var productDetail = pb.GetProductDetail(ProductId);
            var productMaterial = ProductBusiness.GetProductMaterial(ProductId);
            ViewBag.ProductMaterial = productMaterial;
            if (productDetail == null)
            {
                RedirectToAction("Index", "Error");
            }
            return View(productDetail);
        }
	}
}