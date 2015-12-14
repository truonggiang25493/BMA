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
        BMAEntities db = new BMAEntities();
        //
        // GET: /Product/
        public ActionResult Index(int? page, FormCollection f, int? categoryId)
        {
            try
            {
                ViewBag.Show = "product";
                int pageSize = 12;
                int pageNumber = (page ?? 1);
                ProductBusiness pb = new ProductBusiness();
                List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
                ViewBag.lstProductCart = lstProductCart;
                var lstProduct = pb.GetProduct().OrderByDescending(n => n.ProductId).ToPagedList(pageNumber, pageSize);
                ViewBag.ProductTitle = "Tất cả sản phẩm";
                if (categoryId != null)
                {
                    lstProduct = pb.GetProductByCategory(categoryId).OrderByDescending(n=>n.ProductId).ToPagedList(pageNumber, pageSize);
                    var category = db.Categories.SingleOrDefault(n => n.CategoryId == categoryId);
                    ViewBag.ProductTitle = category.CategoryName;
                }
                if (!String.IsNullOrEmpty(f["txtSearch"]))
                {
                    lstProduct = pb.SearchProduct(f["txtSearch"]).OrderByDescending(n => n.ProductId).ToPagedList(pageNumber, pageSize);
                }
                return View(lstProduct);
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }

        }

        //public ActionResult Cookie(int? page)
        //{
        //    ViewBag.Show = "product";
        //    int pageSize = 12;
        //    int pageNumber = (page ?? 1);
        //    ProductBusiness pb = new ProductBusiness();
        //    List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
        //    ViewBag.lstProductCart = lstProductCart;
        //    var lstCookies = pb.GetCookie().ToPagedList(pageNumber, pageSize);
        //    return View(lstCookies);
        //}

        //public ActionResult Saltine(int? page)
        //{
        //    ViewBag.Show = "product";
        //    int pageSize = 12;
        //    int pageNumber = (page ?? 1);
        //    ProductBusiness pb = new ProductBusiness();
        //    List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
        //    ViewBag.lstProductCart = lstProductCart;
        //    var lstSaltine = pb.GetSaltine().ToPagedList(pageNumber, pageSize);
        //    return View(lstSaltine);
        //}

        public ActionResult ProductDetail(int ProductId)
        {
            try
            {
                ViewBag.Show = "product";
                ProductBusiness pb = new ProductBusiness();
                List<CustomerCartViewModel> lstProductCart = Session["Cart"] as List<CustomerCartViewModel>;
                ViewBag.lstProductCart = lstProductCart;
                var productDetail = pb.GetProductDetail(ProductId);
                var productMaterial = pb.GetProductMaterial(ProductId);
                var otherProduct = pb.GetOtherProduct(ProductId).Take(10);
                var category = db.Categories.FirstOrDefault(n => n.CategoryId == productDetail.CategoryId);
                ViewBag.category = category;
                ViewBag.otherProduct = otherProduct;
                ViewBag.productMaterial = productMaterial;
                if (productDetail == null)
                {
                    RedirectToAction("Index", "Error");
                }
                return View(productDetail);
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult CategoryPartial()
        {
            try
            {
                var lstCategory = db.Categories.ToList();
                ViewBag.lstCategory = lstCategory;
                List<Product> lstProduct = new List<Product>();
                Product product = new Product();
                foreach (var item in lstCategory)
                {
                    product = db.Products.FirstOrDefault(n => n.CategoryId == item.CategoryId);
                    if (product != null)
                    {
                        lstProduct.Add(product);
                    }
                }
                ViewBag.lstProduct = lstProduct;
                return PartialView();

            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }
    }
}