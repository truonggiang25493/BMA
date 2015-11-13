using System;
using System.Collections.Generic;
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
        public int AddInputBill(FormCollection f)
        {

            String supplierIdString = f["supplierId"];
            String inputBillAmountString = f["txtInputBillAmount"];
            String inputTaxAmountString = f["txtInputTaxAmount"];
            String importDate = f["txtImportDate"];
            InputBill inputBill = new InputBill();
            try
            {
                inputBill.SupplierId = Convert.ToInt32(supplierIdString);
                inputBill.InputBillAmount = Convert.ToInt32(inputBillAmountString);
                inputBill.InputTaxAmount = Convert.ToInt32(inputTaxAmountString);
                inputBill.ImportDate = Convert.ToDateTime(importDate);
                inputBill.InputBillCode = supplierIdString + importDate;
                inputBill.InputRawImage = "abc";

            }
            catch (Exception)
            {
                return 0;

            }

            bool result = InputBillBusiness.AddInputBill(inputBill);
            if (result)
            {
                return 1;
            }
            else
            {
                return 0;
            }

        }
        #endregion
    }
}