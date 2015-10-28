using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using System.Net;
using System.Data;
using BMA.Business;
using System.IO;

namespace BMA.Controllers
{
    public class ManageProductController : Controller
    {
        public ActionResult Index()
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var product = mpb.GetProduct();
            return View(product);
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var category = mpb.GetCategory();
            ViewBag.category = category;
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(FormCollection f, HttpPostedFileBase file)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var allowedExtensions = new[] {  
            ".Jpg", ".png", ".jpg", "jpeg"  
            };
            var maxSize = 1048576;
            string productName = f["productName"];
            string productUnit = f["productUnit"];
            double productWeight = double.Parse(f["productWeight"]);
            string productDes = f["productDes"];
            string productNote = f["productNote"];
            int productPrice = int.Parse(f["productPrice"]);
            int dropCate = int.Parse(f["dropCate"]);
            string productCode = f["productCode"];
            var fileName = "";

            //Get Image
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
                            var path = Path.Combine(Server.MapPath("~/Content/Images/BakeryImages"), fileName);
                            file.SaveAs(path);
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Kích thước hình ảnh quá lớn";
                        string strURL = Request.UrlReferrer.AbsolutePath;
                        return Redirect(strURL);
                    }
                }
                else
                {
                    TempData["Message"] = "Xin chọn file hình ảnh";
                    string strURL = Request.UrlReferrer.AbsolutePath;
                    return Redirect(strURL);
                }
            }

            var productList = mpb.GetActiveProduct();
            for (int i = 0; i < productList.Count; i++)
            {
                if (productName == productList[i].ProductName)
                {
                    string strURL = Request.UrlReferrer.AbsolutePath;
                    TempData["Error"] = String.Format("{0}{1}", productName, " đã tồn tại");
                    return Redirect(strURL);
                }
            }
            try
            {
                mpb.AddProduct(productName, productUnit, productWeight, productDes, productNote, productPrice, dropCate, productCode, fileName);
                int productId = mpb.GetProductId();
                return RedirectToAction("ProductMaterial", new { productId });
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Không thể thêm. Xin vui lòng thử lại.");
            }
            return View();
        }
        public ActionResult Detail(int productId)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var product = mpb.GetProductDetail(productId);
            var productMaterial = mpb.GetListMaterial(productId);
            ViewBag.ProductMaterial = productMaterial;
            return View(product);
        }

        public ActionResult Edit(int productId)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var product = mpb.GetProductDetail(productId);
            var category = mpb.GetCategory();
            ViewBag.category = category;
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirm(int productId, FormCollection f, HttpPostedFileBase file)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var allowedExtensions = new[] {  
            ".Jpg", ".png", ".jpg", "jpeg"  
            };
            var maxSize = 1048576;
            string productName = f["productName"];
            string productUnit = f["productUnit"];
            double productWeight = double.Parse(f["productWeight"]);
            string productDes = f["productDes"];
            string productNote = f["productNote"];
            int productPrice = int.Parse(f["productPrice"]);
            int dropCate = int.Parse(f["dropCate"]);
            string productCode = f["productCode"];
            var fileName = "";
            //Get Image
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
                            var path = Path.Combine(Server.MapPath("~/Content/Images/BakeryImages"), fileName);
                            file.SaveAs(path);
                        }
                    }
                    else
                    {
                        TempData["Message"] = "Kích thước hình ảnh quá lớn";
                        string strURL = Request.UrlReferrer.AbsolutePath;
                        string URL = String.Format("{0}{1}{2}", strURL, "?ProductId=", productId);
                        return Redirect(URL);
                    }
                }
                else
                {
                    TempData["Message"] = "Xin chọn file hình ảnh";
                    string strURL = Request.UrlReferrer.AbsolutePath;
                    string URL = String.Format("{0}{1}{2}", strURL, "?ProductId=", productId);
                    return Redirect(URL);
                }
            }


            var productToUpdate = mpb.GetProductDetail(productId);
            var productList = mpb.GetActiveProduct();
            if (productName != productToUpdate.ProductName)
            {
                for (int i = 0; i < productList.Count; i++)
                {
                    if (productName == productList[i].ProductName)
                    {
                        string strURL = Request.UrlReferrer.AbsolutePath;
                        string URL = String.Format("{0}{1}{2}", strURL, "?ProductId=", productId);
                        TempData["Error"] = String.Format("{0}{1}", productName, " đã tồn tại");
                        return Redirect(URL);
                    }
                }
            }
            try
            {
                if (ModelState.IsValid)
                {
                    mpb.EditProduct(productId, productName, productUnit, productWeight, productDes, productNote, productPrice, dropCate, productCode, fileName);
                    return RedirectToAction("Index");
                }
                return View();
            }
            catch (DataException)
            {
                ModelState.AddModelError("", "Không thể sửa. Xin vui lòng thử lại.");
            }
            return View(productToUpdate);
        }

        public ActionResult ChangeStatus(int productId, bool status, string strURL)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var radioButton = Convert.ToBoolean(Request.Form["status"]);
            mpb.ChangeStatus(productId, radioButton);
            return Redirect(strURL);
        }

        public ActionResult ProductMaterial(int productId)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var productMaterial = mpb.GetListMaterial(productId);
            var product = mpb.GetProductDetail(productId);
            ViewBag.productName = product.ProductName;
            ViewBag.productId = product.ProductId;
            return View(productMaterial);
        }

        public ActionResult DeleteProductMaterial(int productId, int productMaterialId, string strURL)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            mpb.DeleteProductMaterial(productId, productMaterialId);
            return Redirect(strURL);
        }

        public ActionResult UpdateProductMaterial(FormCollection f, int productId, string strURL)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            String[] lstQuantity = f["txtQuantity"].ToString().Split(',');
            mpb.UpdateProductMaterial(productId, lstQuantity);
            return Redirect(strURL);
        }

        [HttpPost]
        public ActionResult AddProductMaterial(FormCollection form)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            int productId = int.Parse(form["productId"]);
            int materialId = int.Parse(form["productMaterialId"]);
            string strURL = String.Format("{0}{1}", "/ManageProduct/ProductMaterial?ProductId=", productId);
            mpb.AddProductMaterial(productId, materialId);
            return Redirect(strURL);
        }
    }
}