using System;
using System.Collections.Generic;
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
            ViewBag.TreeView = "inputBill";
            ViewBag.TreeViewMenu = "listInputBill";
            var inputBillList = InputBillBusiness.GetInputBillList();
            if (inputBillList == null)
            {
                RedirectToAction("InputBillIndex", "InputBill");
            }
            return View(inputBillList);
        }
        #endregion

        #region Get Input Material Detail
        public ActionResult InputBillDetail(int id)
        {
            ViewBag.TreeView = "inputBill";
            InputBill inputBillDetail = inputBillBusiness.GetInputBill(id);
            if (inputBillDetail == null)
            {
                RedirectToAction("InputBillIndex", "InputBill");

            }
            return View(inputBillDetail);

        }

        #endregion

        #region Get input material in bill
        public ActionResult GetInputMaterialInBill(int id)
        {
            List<InputMaterial> inputMaterialList = db.InputMaterials.Where(n => n.InputBillId == id).ToList();
            return PartialView("InputMaterialInBillPartialView", inputMaterialList);
        }
        #endregion

        #region Get Popup Supplier
        public ActionResult GetSupplierList()
        {
            List<Supplier> supplierList = db.Suppliers.ToList();
            return PartialView("SupplierPartialView", supplierList);
        }
        #endregion

        #region Add New Input Bill View
        public ActionResult AddInputBill()
        {
            ViewBag.TreeView = "inputBill";
            ViewBag.TreeViewMenu = "addInputBill";
            List<InputBill> inputBills = db.InputBills.ToList();
            return View(inputBills);
        }
        #endregion
        [HttpPost]
        #region Add New Input Bill
        public int AddInputBill(string supplierIdString, string inputBillAmountString, string inputTaxAmountString, string importDate, string fileName)
        {
            InputBill inputBill = new InputBill();
            try
            {
                inputBill.SupplierId = Convert.ToInt32(supplierIdString);
                inputBill.InputBillAmount = Convert.ToInt32(inputBillAmountString);
                inputBill.InputTaxAmount = Convert.ToInt32(inputTaxAmountString);
                inputBill.ImportDate = Convert.ToDateTime(importDate);
                inputBill.InputBillCode = supplierIdString + importDate;
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
        #endregion

        #region Add input bill image
        [HttpPost]
        public int AddImage(HttpPostedFileBase inputBillImage)
        {
            var allowedExtensions = new[] {  
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
                        var comparePath = Server.MapPath(string.Format("{0}{1}", "~/Content/Images/InputBillImages", fileName));
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
            ViewBag.TreeView = "inputBill";
            InputBill inputBill = db.InputBills.SingleOrDefault(m => m.InputBillId == id);
            return View(inputBill);
        }
        #endregion

        #region Edit Input Bill
        [HttpPost]
        public int EditInputBill(string supplierIdString, string inputBillAmountString, string inputTaxAmountString, string importDate, string fileName)
        {
            InputBill inputBill = new InputBill();
            try
            {
                inputBill.SupplierId = Convert.ToInt32(supplierIdString);
                inputBill.InputBillAmount = Convert.ToInt32(inputBillAmountString);
                inputBill.InputTaxAmount = Convert.ToInt32(inputTaxAmountString);
                inputBill.ImportDate = Convert.ToDateTime(importDate);
                inputBill.InputBillCode = supplierIdString + importDate;
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
        #endregion
    }

}