using System;
using System.Collections.Generic;
using System.Globalization;
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
using BMA.DBChangesNotifer;
using Microsoft.AspNet.SignalR;
using BMA.Hubs;

namespace BMA.Controllers
{
    public class InputMaterialController : Controller
    {
        private BMAEntities db = new BMAEntities();
        private InputMaterialBusiness inputMaterialBusiness = new InputMaterialBusiness();

        #region Get Input Material Index

        public ActionResult InputMaterialIndex()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Title = "Danh sách nguyên liệu đầu vào";
                ViewBag.TreeView = "inputMaterial";
                ViewBag.TreeViewMenu = "listInputMaterial";
                db = new BMAEntities();
                inputMaterialBusiness = new InputMaterialBusiness();
                var inputMaterialslList = InputMaterialBusiness.GetInputMaterialList();
                if (inputMaterialslList == null)
                {
                    return RedirectToAction("Index", "StoreInfor");
                }
                return View(inputMaterialslList);
            }
        }

        #endregion

        #region Get Input Material Detail
        public ActionResult InputMaterialDetail(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    db = new BMAEntities();
                    ViewBag.TreeView = "inputMaterial";
                    InputMaterial inputMaterialDetail = inputMaterialBusiness.GetInputMaterial(id);
                    if (inputMaterialDetail == null)
                    {
                        return RedirectToAction("InputMaterialIndex", "InputMaterial");

                    }

                    return View(inputMaterialDetail);

                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "StoreInfor"); ;
                }
            }
        }
        #endregion

        #region Change Input Material Status

        [HttpPost]
        public int ChangeInputMaterialStatus(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
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
        }

        #endregion

        #region Add New Input Material View

        public ActionResult AddInputMaterial()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                db = new BMAEntities();
                ViewBag.TreeView = "inputMaterial";
                ViewBag.TreeViewMenu = "addInputMaterial";
                List<ProductMaterial> productMaterials = db.ProductMaterials.ToList();
                return View(productMaterials);
            }
        }

        #endregion
        [HttpPost]
        #region Add New Input Material
        public int AddInputMaterial(FormCollection f)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                InputMaterial inputMaterial = new InputMaterial();
                String productMaterialIdString = f["productMaterialId"];
                int productMaterialId = Convert.ToInt32(productMaterialIdString);
                String importQuantityString = f["txtImportQuantity"];
                int importQuantity = Convert.ToInt32(importQuantityString);
                String inputMaterialUnitPriceString = f["txtUnitPrice"];
                String importDateString = f["txtImportDate"];
                String inputMaterialExpiryDateString = f["txtInputMaterialExpiryDate"];
                String inputMaterialNote = f["txtInputMaterialNote"];
                String inputBillId = f["inputBillId"];
                
                var importDate =DateTime.ParseExact(importDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                var expireDate = DateTime.ParseExact(inputMaterialExpiryDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                if (DateTime.Compare(expireDate,importDate)>=1)
                {
                    try
                    {
                        int inputMaterialUnitPrice = Convert.ToInt32(inputMaterialUnitPriceString);
                        
                        inputMaterial.ImportQuantity = importQuantity;
                        inputMaterial.InputMaterialPrice = inputMaterialUnitPrice;
                        inputMaterial.ImportDate = DateTime.ParseExact(importDateString, "dd/MM/yyyy",
                            CultureInfo.InvariantCulture);
                        inputMaterial.InputMaterialExpiryDate = DateTime.ParseExact(inputMaterialExpiryDateString,
                            "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        inputMaterial.InputMaterialNote = inputMaterialNote;
                        inputMaterial.InputBillId = Convert.ToInt32(inputBillId);
                        //Change product material quantity
                        inputMaterial.ProductMaterialId = productMaterialId;
                        inputMaterial.RemainQuantity = Convert.ToInt32(importQuantity);
                        inputMaterial.IsActive = true;
                    }
                    catch (Exception)
                    {
                        return 0;

                    }
                }
                else
                {
                    return -1;
                }
                //Close connection with hub
                MvcApplication.lowQuantityNotifer.Dispose();
                bool result = InputMaterialBusiness.AddInputMaterial(productMaterialId, inputMaterial, importQuantity);

                //Connection with hub
                MvcApplication.lowQuantityNotifer.Start("BMAChangeDB", "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
                MvcApplication.lowQuantityNotifer.Change += this.OnChange2;
                if (result)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        [HttpPost]
        #region Get Popup Input Bill
        public ActionResult GetInputBillList()
        {
            List<InputBill> inputBillList = db.InputBills.OrderByDescending(m=>m.ImportDate).ToList();
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
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int) Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                try
                {
                    InputMaterial inputMaterials = db.InputMaterials.SingleOrDefault(m => m.InputMaterialId == id);
                    if (inputMaterials == null || (inputMaterials.RemainQuantity) != inputMaterials.ImportQuantity || (DateTime.Compare(inputMaterials.InputMaterialExpiryDate, DateTime.Today) < 1))
                    {
                        return RedirectToAction("InputMaterialIndex", "InputMaterial");
                    }
                    else{
                        var productMaterial = db.ProductMaterials.ToList();
                        ViewBag.productMaterial = productMaterial;
                        return View(inputMaterials);
                    }
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "StoreInfor");
                    ;
                }
            }
            
        }
        #endregion

        [HttpPost]
        #region Edit Input Material
        public int EditInputMaterial(FormCollection f)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                String productMaterialIdString = f["productMaterialId"];
                String importQuantityString = f["txtImportQuantity"];
                String inputMaterialPriceString = f["txtUnitPrice"];
                String importDateString = f["txtImportDate"];
                String inputMaterialExpiryDateString = f["txtInputMaterialExpiryDate"];
                String inputMaterialNote = f["txtInputMaterialNote"];
                String inputBillIdString = f["inputBillId"];
                String inputMaterialIdString = f["InputMaterialId"];

                if (
                    !(productMaterialIdString.IsEmpty() || importQuantityString.IsEmpty() ||
                      inputMaterialPriceString.IsEmpty() || importDateString.IsEmpty() ||
                      inputMaterialExpiryDateString.IsEmpty() || inputBillIdString.IsEmpty() ||
                      inputMaterialIdString.IsEmpty()))
                {
                    var checkimportDate = DateTime.ParseExact(importDateString, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var checkExpireDate = DateTime.ParseExact(inputMaterialExpiryDateString, "dd/MM/yyyy",
                        CultureInfo.InvariantCulture);
                    if (DateTime.Compare(checkExpireDate, checkimportDate) >= 1)
                    {
                        try
                        {
                            int inputMaterialId = Convert.ToInt32(inputMaterialIdString);
                            int importQuantity = Convert.ToInt32(importQuantityString);
                            int productMaterialId = Convert.ToInt32(productMaterialIdString);
                            int inputMaterialPrice = Convert.ToInt32(inputMaterialPriceString);
                            DateTime importDate = DateTime.ParseExact(importDateString, "dd/MM/yyyy",
                                CultureInfo.InvariantCulture);
                            DateTime inputMaterialExpiryDate = DateTime.ParseExact(inputMaterialExpiryDateString,
                                "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            int inputBillId = Convert.ToInt32(inputBillIdString);
                            //Close connection with hub
                            MvcApplication.lowQuantityNotifer.Dispose();
                            bool result = InputMaterialBusiness.EditInputMaterial(inputMaterialId, importQuantity,
                                productMaterialId, inputMaterialPrice, importDate, inputMaterialExpiryDate, inputBillId,
                                inputMaterialNote);
                            //Connection with hub
                            MvcApplication.lowQuantityNotifer.Start("BMAChangeDB",
                                "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
                            MvcApplication.lowQuantityNotifer.Change += this.OnChange2;
                            return result ? 1 : 0;
                        }
                        catch (Exception)
                        {
                            return 0;
                        }
                    }
                    return -1;
                }
                return 0;
            }
        }

        #endregion

        #region Add Onchange when material low
        private void OnChange2(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange2(e.Info, e.Source, e.Type);
        }
        #endregion
    }
}

