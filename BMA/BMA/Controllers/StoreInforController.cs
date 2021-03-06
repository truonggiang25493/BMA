﻿using System;
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
            try
            {
                if (Session["User"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.TreeView = "storeInfor";
                ViewBag.TreeViewMenu = "storeInforList";
                AccountBusiness ab = new AccountBusiness();
                StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
                List<Product> lstNewProduct = db.Products.OrderByDescending(n => n.ProductId).Take(4).ToList();
                ViewBag.lstProduct = lstNewProduct;
                List<Order> lstOrder = db.Orders.Where(n => n.OrderStatus == 0).ToList();
                ViewBag.orderWaiting = lstOrder.Count;
                List<Customer> lstCustomer = db.Customers.ToList();
                ViewBag.customer = lstCustomer.Count;
                List<ProductMaterial> lstLowQuantity = ab.StaffOffLowQuantityNoty();
                ViewBag.lowQuantity = lstLowQuantity.Count;
                return View(storeInfo);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
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
            try
            {
                if (Session["User"] == null || Session["UserId"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if ((int)Session["UserRole"] == 2)
                {
                    return RedirectToAction("Index");
                }
                StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
                return View(storeInfo);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        [HttpPost, ActionName("EditStoreInfo")]
        public int EditStoreInfoConfirm(string storeName, string ownerName, string Email, string address, string Province, string District, string phoneNumber, string Fax, string taxCode, string fileName)
        {
            try
            {
                StoreInforBusiness sib = new StoreInforBusiness();
                sib.EditStoreInfo(storeName, ownerName, Email, address, Province, District, phoneNumber, Fax, taxCode, fileName);
                return 2;
            }
            catch
            {
                return -4;
            }
        }

        public ActionResult ConfigIndex()
        {
            try
            {
                if (Session["User"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if ((int)Session["UserRole"] == 2)
                {
                    return RedirectToAction("Index");
                }
                ViewBag.TreeView = "storeInfor";
                ViewBag.TreeViewMenu = "configStoreInfor";
                AccountBusiness ab = new AccountBusiness();
                List<Product> lstNewProduct = db.Products.OrderBy(n => n.ProductId).Take(4).ToList();
                ViewBag.lstProduct = lstNewProduct;
                List<Order> lstOrder = db.Orders.Where(n => n.OrderStatus == 0).ToList();
                ViewBag.orderWaiting = lstOrder.Count;
                List<Customer> lstCustomer = db.Customers.ToList();
                ViewBag.customer = lstCustomer.Count;
                List<ProductMaterial> lstLowQuantity = ab.StaffOffLowQuantityNoty();
                ViewBag.lowQuantity = lstLowQuantity.Count;

                Policy policy = db.Policies.SingleOrDefault(n => n.PolicyId == 1);
                ViewBag.minQuantity = policy;
                Policy policy2 = db.Policies.SingleOrDefault(n => n.PolicyId == 2);
                ViewBag.maxPrice = policy2;
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
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
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
            try
            {
                StoreInforBusiness sib = new StoreInforBusiness();
                bool boolUsing = false;
                if (beUsing == "1")
                {
                    boolUsing = true;
                }
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
            try
            {
                StoreInforBusiness sib = new StoreInforBusiness();
                var nowMaxPriceProduct = db.Products.OrderByDescending(i => i.ProductStandardPrice).Select(n => n.ProductStandardPrice).FirstOrDefault();
                if (maxPrice < nowMaxPriceProduct)
                {
                    return -1;
                }

                maxPrice = Convert.ToInt32(maxPrice.ToString().Replace(".", ""));
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
            try
            {
                StoreInforBusiness sib = new StoreInforBusiness();
                for (int i = 0; i < categoryName.Length; i++)
                {
                    if ((i + 1) != categoryName.Length)
                    {
                        if (StringComparer.CurrentCultureIgnoreCase.Equals(categoryName[i], categoryName[i + 1]))
                        {
                            return -2;
                        }
                    }

                    if (i > 0)
                    {
                        if (StringComparer.CurrentCultureIgnoreCase.Equals(categoryName[i], categoryName[i - 1]))
                        {
                            return -2;
                        }
                    }
                }

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
            try
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
            try
            {
                StoreInforBusiness sib = new StoreInforBusiness();
                var categoryList = db.Categories.ToList();
                for (int i = 0; i < categoryList.Count; i++)
                {
                    if (StringComparer.CurrentCultureIgnoreCase.Equals(categoryName, categoryList[i].CategoryName))
                    {
                        return -2;
                    }
                }

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
            try
            {
                StoreInfo storeInfo = db.StoreInfoes.SingleOrDefault();
                ViewBag.storeInfo = storeInfo;
                return PartialView();
            }
            catch
            {
                return null;
            }
        }

        public ActionResult NotificatePartial()
        {
            try
            {
                if (Session["NotificateCount"] == null)
                {
                    int count = 0;
                    ViewBag.notificatePartialCount = count;
                    Session["NotificateCount"] = count;
                    int newOrderCount = 0;
                    ViewBag.newOrderCountPartial = newOrderCount;
                    Session["NewOrderCountPartial"] = newOrderCount;
                    int confirmOrderCount = 0;
                    ViewBag.confirmOrderCountPartial = confirmOrderCount;
                    Session["ConfirmOrderCountPartial"] = confirmOrderCount;
                    int cancelOrderCount = 0;
                    ViewBag.cancelOrderCountPartial = cancelOrderCount;
                    Session["CancelOrderCountPartial"] = cancelOrderCount;
                    int lowMaterialCount = 0;
                    ViewBag.lowMaterialCountPartial = lowMaterialCount;
                    Session["LowMaterialCountPartial"] = lowMaterialCount;
                }
                else
                {
                    int count = Convert.ToInt32(Session["NotificateCount"]);
                    ViewBag.notificatePartialCount = count;
                    int newOrderCount = Convert.ToInt32(Session["NewOrderCountPartial"]);
                    ViewBag.newOrderCountPartial = newOrderCount;
                    int confirmOrderCount = Convert.ToInt32(Session["ConfirmOrderCountPartial"]);
                    ViewBag.confirmOrderCountPartial = confirmOrderCount;
                    int cancelOrderCount = Convert.ToInt32(Session["CancelOrderCountPartial"]);
                    ViewBag.cancelOrderCountPartial = cancelOrderCount;
                    int lowMaterialCount = Convert.ToInt32(Session["LowMaterialCountPartial"]);
                    ViewBag.lowMaterialCountPartial = lowMaterialCount;
                }
                return PartialView();
            }
            catch
            {
                return null;
            }
        }

        public int NotificatePartialLink(int count, int newOrderCount, int confirmOrderCount, int cancelOrderCount, int lowMaterialCount)
        {
            try
            {
                ViewBag.notificatePartialCount = count;
                Session["NotificateCount"] = count;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["NewOrderCountPartial"] = newOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["ConfirmOrderCountPartial"] = confirmOrderCount;
                ViewBag.cancelOrderCountPartial = cancelOrderCount;
                Session["CancelOrderCountPartial"] = cancelOrderCount;
                ViewBag.lowMaterialCountPartial = lowMaterialCount;
                Session["LowMaterialCountPartial"] = lowMaterialCount;
                return 1;
            }
            catch
            {
                return -1;
            }
           
        }

        public int RemoveOrderNoty(int lowMaterialCount, int confirmOrderCount, int cancelOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = lowMaterialCount + confirmOrderCount + cancelOrderCount;
                Session["NotificateCount"] = lowMaterialCount + confirmOrderCount + cancelOrderCount;
                int newOrderCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["NewOrderCountPartial"] = newOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["ConfirmOrderCountPartial"] = confirmOrderCount;
                ViewBag.cancelOrderCountPartial = cancelOrderCount;
                Session["CancelOrderCountPartial"] = cancelOrderCount;
                ViewBag.lowMaterialCountPartial = lowMaterialCount;
                Session["LowMaterialCountPartial"] = lowMaterialCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int RemoveConfirmStatusNoty(int newOrderCount, int lowMaterialCount, int cancelOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = lowMaterialCount + newOrderCount + cancelOrderCount;
                Session["NotificateCount"] = lowMaterialCount + newOrderCount + cancelOrderCount;
                int confirmOrderCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["NewOrderCountPartial"] = newOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["ConfirmOrderCountPartial"] = confirmOrderCount;
                ViewBag.cancelOrderCountPartial = cancelOrderCount;
                Session["CancelOrderCountPartial"] = cancelOrderCount;
                ViewBag.lowMaterialCountPartial = lowMaterialCount;
                Session["LowMaterialCountPartial"] = lowMaterialCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int RemoveCancelStatusNoty(int newOrderCount, int lowMaterialCount, int confirmOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = lowMaterialCount + newOrderCount + confirmOrderCount;
                Session["NotificateCount"] = lowMaterialCount + newOrderCount + confirmOrderCount;
                int cancelOrderCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["NewOrderCountPartial"] = newOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["ConfirmOrderCountPartial"] = confirmOrderCount;
                ViewBag.cancelOrderCountPartial = cancelOrderCount;
                Session["CancelOrderCountPartial"] = cancelOrderCount;
                ViewBag.lowMaterialCountPartial = lowMaterialCount;
                Session["LowMaterialCountPartial"] = lowMaterialCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int RemoveMaterialNoty(int newOrderCount, int confirmOrderCount, int cancelOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = newOrderCount + confirmOrderCount + cancelOrderCount;
                Session["NotificateCount"] = newOrderCount + confirmOrderCount + cancelOrderCount;
                int lowMaterialCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["NewOrderCountPartial"] = newOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["ConfirmOrderCountPartial"] = confirmOrderCount;
                ViewBag.cancelOrderCountPartial = cancelOrderCount;
                Session["CancelOrderCountPartial"] = cancelOrderCount;
                ViewBag.lowMaterialCountPartial = lowMaterialCount;
                Session["LowMaterialCountPartial"] = lowMaterialCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }
    }
}