using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.Services.Description;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class InputMaterialController : Controller
    {
        private BMAEntities db = new BMAEntities();
        private InputMaterialBusiness inputMaterialBusiness = new InputMaterialBusiness();

        #region Get Input Material Index

        public ActionResult InputMaterialIndex()
        {
            ViewBag.Title = "Danh sách nguyên liệu đầu vào";
            ViewBag.TreeView = "inputMaterial";
            ViewBag.TreeViewMenu = "listInputMaterial";
            var inputMaterialslList = InputMaterialBusiness.GetInputMaterialList();
            if (inputMaterialslList == null)
            {
                RedirectToAction("InputMaterialIndex", "InputMaterial");
            }
            return View(inputMaterialslList);
        }

        #endregion

        #region Get Input Material Detail
        public ActionResult InputMaterialDetail(int id)
        {
            ViewBag.TreeView = "inputMaterial";
            InputMaterial inputMaterialDetail = inputMaterialBusiness.GetInputMaterial(id);
            if (inputMaterialDetail == null)
            {
                RedirectToAction("InputMaterialIndex", "InputMaterial");

            }
            return View(inputMaterialDetail);

        }
        #endregion

        #region Change Input Material Status
        [HttpPost]
        public int ChangeInputMaterialStatus(int id)
        {
            InputMaterialBusiness inputMaterialBusiness = new InputMaterialBusiness();
            Boolean result = InputMaterialBusiness.ChangeInputMaterialStage(id);
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

        #region Add New Input Material View
        public ActionResult AddInputMaterial()
        {
            ViewBag.TreeView = "inputMaterial";
            ViewBag.TreeViewMenu = "addInputMaterial";
            List<ProductMaterial> productMaterials = db.ProductMaterials.ToList();
            return View(productMaterials);
        }
        #endregion
        [HttpPost]
        #region Add New Input Material
        public int AddInputMaterial(FormCollection f)
        {

            String productMaterialIdString = f["productMaterialId"];
            String importQuantity = f["txtImportQuantity"];
            String inputMaterialPrice = f["txtInputMaterialPrice"];
            String importDate = f["txtImportDate"];
            String inputMaterialExpiryDate = f["txtInputMaterialExpiryDate"];
            String inputMaterialNote = f["txtInputMaterialNote"];
            String inputBillId = f["inputBillId"];
            InputMaterial inputMaterial = new InputMaterial();
            try
            {
                inputMaterial.ImportQuantity = int.Parse(importQuantity);
                inputMaterial.InputMaterialPrice = int.Parse(inputMaterialPrice);
                inputMaterial.ImportDate = Convert.ToDateTime(importDate);
                inputMaterial.InputMaterialExpiryDate = Convert.ToDateTime(inputMaterialExpiryDate);
                inputMaterial.InputMaterialNote = inputMaterialNote;
                inputMaterial.InputBillId = int.Parse(inputBillId);
                int productMaterialId = int.Parse(productMaterialIdString);
                inputMaterial.ProductMaterialId = productMaterialId;
                inputMaterial.RemainQuantity = int.Parse(importQuantity);
                inputMaterial.IsActive = true;
            }
            catch (Exception)
            {
                return 0;

            }

            bool result = InputMaterialBusiness.AddInputMaterial(inputMaterial);
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

        [HttpPost]
        #region Get Popup Input Bill
        public ActionResult GetInputBillList()
        {
            List<InputBill> inputBillList = db.InputBills.ToList();
            return PartialView("InputListPartialView", inputBillList);
        }
        #endregion

        #region Get Popup Product Material
        public ActionResult GetProductMaterialList()
        {
            List<ProductMaterial> productMaterialList = db.ProductMaterials.ToList();
            return PartialView("ProductMaterialPartialView", productMaterialList);
        }
        #endregion

        #region Get Table Input Material by Input Bill
        public ActionResult GetInputMaterialTable(int id)
        {
            List<InputMaterial> inputMaterialList = db.InputMaterials.Where(n => n.InputBillId == id).ToList();
            return PartialView("InputMaterialByBillIdPartialView", inputMaterialList);
        }
        #endregion

        #region Edit Input Material View
        public ActionResult EditInputMaterial(int id)
        {
            InputMaterial inputMaterials = db.InputMaterials.SingleOrDefault(m => m.InputMaterialId == id);
            var productMaterial = db.ProductMaterials.ToList();
            ViewBag.productMaterial = productMaterial;
            return View(inputMaterials);
        }
        #endregion
        [HttpPost]
        #region Edit Input Material
        public int EditInputMaterial(FormCollection f)
        {
            String productMaterialIdString = f["productMaterialId"];
            String importQuantityString = f["txtImportQuantity"];
            String inputMaterialPriceString = f["txtInputMaterialPrice"];
            String importDateString = f["txtImportDate"];
            String inputMaterialExpiryDateString = f["txtInputMaterialExpiryDate"];
            String inputMaterialNote = f["txtInputMaterialNote"];
            String inputBillIdString = f["inputBillId"];
            String inputMaterialIdString = f["InputMaterialId"];

            if (
                !(productMaterialIdString.IsEmpty() || importQuantityString.IsEmpty() ||
                  inputMaterialPriceString.IsEmpty() || importDateString.IsEmpty() ||
                  inputMaterialExpiryDateString.IsEmpty() || inputMaterialNote.IsEmpty() || inputBillIdString.IsEmpty() ||
                  inputMaterialIdString.IsEmpty()))
            {
                int inputMaterialId = Convert.ToInt32(inputMaterialIdString);
                int importQuantity = Convert.ToInt32(importQuantityString);
                int productMaterialId = Convert.ToInt32(productMaterialIdString);
                int inputMaterialPrice = Convert.ToInt32(inputMaterialPriceString);
                DateTime importDate = Convert.ToDateTime(importDateString);
                DateTime inputMaterialExpiryDate = Convert.ToDateTime(inputMaterialExpiryDateString);
                int inputBillId = Convert.ToInt32(inputBillIdString);

                bool result = InputMaterialBusiness.EditInputMaterial(inputMaterialId, importQuantity, productMaterialId, inputMaterialPrice, importDate, inputMaterialExpiryDate, inputBillId, inputMaterialNote);
                return result ? 1 : 0;
            }
            return 0;
        }
        #endregion
    }
}

