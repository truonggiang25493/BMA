using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Business;
using System.IO;

namespace BMA.Controllers
{
    public class StoreInforController : Controller
    {
        BMAEntities db = new BMAEntities();
        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] == 3)
            {
                return RedirectToAction("Index");
            }
            StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
            List<Product> lstNewProduct = db.Products.OrderByDescending(n => n.ProductId).Take(4).ToList();
            ViewBag.lstProduct = lstNewProduct;
            List<Order> lstOrder = db.Orders.Where(n => n.OrderStatus == 0).ToList();
            ViewBag.orderWaiting = lstOrder.Count;
            List<Customer> lstCustomer = db.Customers.ToList();
            ViewBag.customer = lstCustomer.Count;
            return View(storeInfo);
        }

        public int EditImage(HttpPostedFileBase file)
        {
            var allowedExtensions = new[] {  
            ".Jpg", ".png", ".jpg", "jpeg", ".JPG", ".PNG", ".JPEG"  
            };
            var maxSize = 1048576;
            var fileName = "";
            if (file != null)
            {
                var productSize = file.ContentLength;
                fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                if (allowedExtensions.Contains(ext))
                {
                    if (productSize <= maxSize)
                    {
                        var comparePath = Server.MapPath(string.Format("{0}{1}", "~/Content/Images/BakeryImages", fileName));
                        if (!System.IO.File.Exists(comparePath))
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/CustomerLayout/Images/"), fileName);
                            file.SaveAs(path);
                        }
                        return 1;
                    }
                    else
                    {
                        //TempData["Message"] = "Kích thước hình ảnh quá lớn";
                        string strURL = Request.UrlReferrer.AbsolutePath;
                        return -2;
                    }
                }
                else
                {
                    //TempData["Message"] = "Xin chọn file hình ảnh";
                    string strURL = Request.UrlReferrer.AbsolutePath;
                    return -3;
                }
            }
            else
            {
                return 1;
            }
        }

        [HttpGet]
        public ActionResult EditStoreInfo()
        {
            StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
            return View(storeInfo);
        }

        [HttpPost, ActionName("EditStoreInfo")]
        public int EditStoreInfoConfirm(string storeName, string ownerName, string address, string phoneNumber, string taxCode, string fileName)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            try
            {
                sib.EditStoreInfo(storeName, ownerName, address, phoneNumber, taxCode, fileName);
                return 2;
            }
            catch
            {
                return -4;
            }
        }

        public ActionResult ConfigIndex()
        {
            if (Session["User"] == null || Session["UserRole"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] == 2)
            {
                return RedirectToAction("Index");
            }
            List<Product> lstNewProduct = db.Products.OrderBy(n => n.ProductId).Take(4).ToList();
            ViewBag.lstProduct = lstNewProduct;
            List<Order> lstOrder = db.Orders.Where(n => n.OrderStatus == 0).ToList();
            ViewBag.orderWaiting = lstOrder.Count;
            List<Customer> lstCustomer = db.Customers.ToList();
            ViewBag.customer = lstCustomer.Count;
            Policy policy = db.Policies.SingleOrDefault();
            ViewBag.policy = policy;
            int? maxPrice = db.StoreInfoes.Select(n => n.ProductMaxPrice).SingleOrDefault();
            ViewBag.maxPrice = maxPrice;
            List<DiscountByQuantity> discountByQuantity = db.DiscountByQuantities.ToList();
            ViewBag.discountByQuantity = discountByQuantity;
            var quantityFrom = db.DiscountByQuantities.Select(n => n.QuantityFrom).ToList();
            var quantityTo = db.DiscountByQuantities.Select(n => n.QuantityTo).ToList();
            var discountRate = db.DiscountByQuantities.Select(n => n.DiscountValue).ToList();
            ViewBag.QuantityFrom = quantityFrom;
            ViewBag.quantityTo = quantityTo;
            ViewBag.DiscountValue = discountRate;
            List<Category> category = db.Categories.Where(n => n.CategoryName != "Bánh").ToList();
            ViewBag.category = category;
            return View();
        }

        [HttpPost]
        public int MinQuantity(int bound)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            try
            {
                sib.MinQuantity(bound);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        [HttpPost]
        public int changeDiscountQuantity(int[] quantityFrom, int[] quantityTo, int[] discountRate, string beUsing)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            bool boolUsing = false;
            if (beUsing == "1")
            {
                boolUsing = true;
            }
            try
            {
                sib.changeDiscountQuantity(quantityFrom, quantityTo, discountRate, boolUsing);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        [HttpPost]
        public int changeMaxPrice(int maxPrice)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            var nowMaxPriceProduct = db.Products.OrderByDescending(i => i.ProductStandardPrice).Select(n => n.ProductStandardPrice).FirstOrDefault();
            if (maxPrice < nowMaxPriceProduct)
            {
                return -1;
            }
            try
            {
                sib.changeMaxPrice(maxPrice);
                return 1;
            }
            catch
            {
                return -2;
            }
        }

        [HttpPost]
        public int ChangeCategory(string[] categoryName)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            try
            {
                sib.changeCategory(categoryName);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        [HttpPost]
        public int DeleteCategory(int categoryId)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            List<Product> lstProduct = db.Products.ToList();
            for (int i = 0; i < lstProduct.Count; i++)
            {
                if (lstProduct[i].CategoryId == categoryId)
                {
                    return -1;
                }
            }
            try
            {
                sib.deleteCategory(categoryId);
                return 1;
            }
            catch
            {
                return -2;
            }
        }

        [HttpPost]
        public int AddCategory(string categoryName)
        {
            StoreInforBusiness sib = new StoreInforBusiness();
            try
            {
                sib.addCategory(categoryName);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public ActionResult LogoPartial()
        {
            StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
            ViewBag.storeInfo = storeInfo;
            return PartialView();
        }
    }
}