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
            StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
            List<Product> lstNewProduct = db.Products.OrderBy(n => n.ProductId).Take(4).ToList();
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
            List<Product> lstNewProduct = db.Products.OrderBy(n => n.ProductId).Take(4).ToList();
            ViewBag.lstProduct = lstNewProduct;
            List<Order> lstOrder = db.Orders.Where(n => n.OrderStatus == 0).ToList();
            ViewBag.orderWaiting = lstOrder.Count;
            List<Customer> lstCustomer = db.Customers.ToList();
            ViewBag.customer = lstCustomer.Count;
            Policy policy = db.Policies.SingleOrDefault();
            ViewBag.policy = policy;
            List<DiscountByQuantity> discountByQuantity = db.DiscountByQuantities.Where(n => n.beUsing).ToList();
            ViewBag.discountByQuantity = discountByQuantity;
            var quantityFrom = db.DiscountByQuantities.Select(n => n.QuantityFrom).ToList();
            var quantityTo = db.DiscountByQuantities.Select(n => n.QuantityTo).ToList();
            //int[] quantity = new int[]{};
            //for (int i = 0; i < discountByQuantity.Count; i++)
            //{
            //    quantity[i] = Convert.ToInt32(discountByQuantity[i]);
            //}
            ViewBag.quantityFrom = quantityFrom;
            ViewBag.quantityTo = quantityTo;
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

        public ActionResult LogoPartial()
        {
            StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
            ViewBag.storeInfo = storeInfo;
            return PartialView();
        }
    }
}