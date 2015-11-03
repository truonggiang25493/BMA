using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using PagedList.Mvc;
using BMA.Models;
using BMA.Business;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index(int? page)
        {
            ViewBag.Show = "product";
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ProductBusiness pb = new ProductBusiness();
            List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
            ViewBag.lstProductCart = lstProductCart;
            var lstProduct = pb.GetProduct().ToPagedList(pageNumber, pageSize);
            return View(lstProduct);
        }

        public ActionResult Cookie(int? page)
        {
            ViewBag.Show = "product";
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ProductBusiness pb = new ProductBusiness();
            List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
            ViewBag.lstProductCart = lstProductCart;
            var lstCookies = pb.GetCookie().ToPagedList(pageNumber, pageSize);
            return View(lstCookies);
        }

        public ActionResult Saltine(int? page)
        {
            ViewBag.Show = "product";
            int pageSize = 12;
            int pageNumber = (page ?? 1);
            ProductBusiness pb = new ProductBusiness();
            List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
            ViewBag.lstProductCart = lstProductCart;
            var lstSaltine = pb.GetSaltine().ToPagedList(pageNumber, pageSize);
            return View(lstSaltine);
        }

        public ActionResult ProductDetail(int ProductId)
        {
            ViewBag.Show = "product";
            ProductBusiness pb = new ProductBusiness();
            List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
            ViewBag.lstProductCart = lstProductCart;
            var productDetail = pb.GetProductDetail(ProductId);
            var productMaterial = pb.GetProductMaterial(ProductId);
            var otherProduct = pb.GetOtherProduct(ProductId).Take(10);
            ViewBag.otherProduct = otherProduct;
            ViewBag.productMaterial = productMaterial;
            if (productDetail == null)
            {
                RedirectToAction("Index", "Error");
            }
            return View(productDetail);
        }
	}
}