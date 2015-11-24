using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Generator;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class InputBillController : Controller
    {

        private BMAEntities db = new BMAEntities();
        private InputBillBusiness inputBillBusiness = new InputBillBusiness();

        #region Get input bill list

        public ActionResult InputBillIndex()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                db = new BMAEntities();
                ViewBag.TreeView = "inputBill";
                ViewBag.TreeViewMenu = "listInputBill";
                var inputBillList = InputBillBusiness.GetInputBillList();
                if (inputBillList == null)
                {
                    RedirectToAction("InputBillIndex", "InputBill");
                }
                return View(inputBillList);
            }
        }

        #endregion

        #region Get Input Material Detail

        public ActionResult InputBillDetail(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "inputBill";
                InputBill inputBillDetail = inputBillBusiness.GetInputBill(id);
                if (inputBillDetail == null)
                {
                    RedirectToAction("InputBillIndex", "InputBill");

                }
                return View(inputBillDetail);
            }
        }

        #endregion

        #region Get input material in bill

        public ActionResult GetInputMaterialInBill(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                List<InputMaterial> inputMaterialList = db.InputMaterials.Where(n => n.InputBillId == id).ToList();
                return PartialView("InputMaterialInBillPartialView", inputMaterialList);
            }
        }

        #endregion

        #region Get Popup Supplier

        public ActionResult GetSupplierList()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                List<Supplier> supplierList = db.Suppliers.ToList();
                return PartialView("SupplierPartialView", supplierList);
            }
        }

        #endregion

        #region Add New Input Bill View

        public ActionResult AddInputBill()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "inputBill";
                ViewBag.TreeViewMenu = "addInputBill";

                OrderBusiness business = new OrderBusiness();
                ViewBag.TaxRate = business.GetVatRateAtTime(DateTime.Now);

                List<InputBill> inputBills = db.InputBills.ToList();
                return View(inputBills);
            }
        }

        #endregion

        [HttpPost]

        #region Add New Input Bill

        public int AddInputBill(string supplierIdString, string inputBillAmountString, string inputTaxAmountString,
            string importDate, string fileName)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                InputBill inputBill = new InputBill();
                try
                {
                    inputBill.SupplierId = Convert.ToInt32(supplierIdString);
                    inputBill.InputBillAmount = Convert.ToInt32(inputBillAmountString);
                    inputBill.InputTaxAmount = Convert.ToInt32(inputTaxAmountString);
                    inputBill.ImportDate = DateTime.ParseExact(importDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    String inputBillCode = supplierIdString + importDate;
                    inputBill.InputBillCode = inputBillCode.Replace("-", "");
                    inputBill.InputRawImage = fileName;
                }
                catch (Exception)
                {
                    return 0;

                }

                bool result = InputBillBusiness.AddInputBill(inputBill);
                if (result)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Add input bill image

        [HttpPost]
        public int AddImage(HttpPostedFileBase inputBillImage)
        {
            var allowedExtensions = new[]
            {
                ".Jpg", ".png", ".jpg", "jpeg", ".JPG", ".PNG", ".JPEG"
            };
            var maxSize = 1048576;
            var fileName = "";
            if (inputBillImage != null)
            {
                var productSize = inputBillImage.ContentLength;
                fileName = Path.GetFileName(inputBillImage.FileName);
                var ext = Path.GetExtension(inputBillImage.FileName);
                if (allowedExtensions.Contains(ext))
                {
                    if (productSize <= maxSize)
                    {
                        var comparePath =
                            Server.MapPath(string.Format("{0}{1}", "~/Content/Images/InputBillImages", fileName));
                        if (!System.IO.File.Exists(comparePath))
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/Images/InputBillImages"), fileName);
                            inputBillImage.SaveAs(path);
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
                return -1;
            }
        }

        #endregion

        #region Edit Input Bill View

        public ActionResult EditInputBill(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "inputBill";
                OrderBusiness business = new OrderBusiness();
                ViewBag.TaxRate = business.GetVatRateAtTime(DateTime.Now);
                InputBill inputBill = db.InputBills.SingleOrDefault(m => m.InputBillId == id);
                return View(inputBill);
            }
        }



        #endregion

        #region Edit Input Bill Image
        [HttpPost]
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
                        var comparePath = Server.MapPath(string.Format("{0}{1}", "~/Content/Images/InputBillImages", fileName));
                        if (!System.IO.File.Exists(comparePath))
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/Images/InputBillImages"), fileName);
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
        #endregion

        #region Edit Input Bill

        [HttpPost]
        public int EditInputBill(int inputBillId, string supplierIdString, string inputBillAmountString,
            string inputTaxAmountString, string importDate, string fileName)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                int supplierId = Convert.ToInt32(supplierIdString);
                int inputBillAmount = Convert.ToInt32(inputBillAmountString);
                int inputBillTaxAmount = Convert.ToInt32(inputTaxAmountString);
                String inputBillCode = supplierIdString + importDate;

                bool result = InputBillBusiness.EditInputBill(inputBillId, supplierId, inputBillCode, inputBillAmount,
                    inputBillTaxAmount, fileName, importDate);
                if (result)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion
    }

}