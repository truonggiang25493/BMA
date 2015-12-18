using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Business;
using BMA.DBChangesNotifer;
using Microsoft.AspNet.SignalR;
using BMA.Hubs;

namespace BMA.Controllers
{
    public class ManageMaterialController : Controller
    {
        BMAEntities db = new BMAEntities();
        //
        // GET: /ManageMaterial/
        public ActionResult Index()
        {
            try
            {
                if (!MvcApplication.lowQuantityNotifer.CheckConnection())
                {
                    MvcApplication.lowQuantityNotifer.Start("BMAChangeDB", "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
                    MvcApplication.lowQuantityNotifer.Change += this.OnChange2;
                }
                if (Session["User"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.TreeView = "productMaterial";
                ViewBag.TreeViewMenu = "productMaterialList";
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                var material = mmb.GetMaterial().OrderByDescending(n => n.IsActive).ThenByDescending(n => n.CurrentQuantity < n.StandardQuantity).ToList();
                return View(material);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        public ActionResult Detail(int materialId)
        {
            try
            {
                if (Session["User"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.TreeView = "productMaterial";
                ViewBag.TreeViewMenu = "productMaterialList";
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                var material = mmb.GetMaterialDetail(materialId);
                var product = mmb.GetListProduct(materialId);
                ViewBag.ProductUse = product;
                return View(material);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }

        }

        [HttpGet]
        public ActionResult Edit(int materialId)
        {
            try
            {
                if (Session["User"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.TreeView = "productMaterial";
                ViewBag.TreeViewMenu = "productMaterialList";
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                var material = mmb.GetMaterialDetail(materialId);
                return View(material);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        [HttpPost, ActionName("Edit")]
        public int EditConfirm(int materialId, FormCollection f)
        {
            try
            {
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                var materialToUpdate = mmb.GetMaterialDetail(materialId);
                string materialName = f["txtName"].ToString();
                string materialUnit = f.Get("txtUnit").ToString();
                int materialSQuantity = Convert.ToInt32(f["txtSQuantity"]);
                var materialList = mmb.GetMaterial();
                if (materialName != materialToUpdate.ProductMaterialName)
                {
                    for (int i = 0; i < materialList.Count; i++)
                    {
                        if (materialName == materialList[i].ProductMaterialName)
                        {
                            return -2;
                        }
                    }
                }
                if (!mmb.CheckProductMaterial(materialId, materialSQuantity))
                {
                    MvcApplication.lowQuantityNotifer.Dispose();
                }
                if (ModelState.IsValid)
                {
                    mmb.EditMaterial(materialId, materialName, materialUnit, materialSQuantity);
                    return 1;
                }
                return -1;
            }
            catch
            {
                return -3;
            }
        }
        public int ChangeStatus(int materialId, bool status, string strURL)
        {
            try
            {
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                var radioButton = Convert.ToBoolean(Request.Form["status"]);
                List<Recipe> lstReciple = db.Recipes.Where(r => r.ProductMaterialId == materialId && r.Product.IsActive).ToList();
                ProductMaterial productMaterial = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == materialId);
                if (productMaterial.IsActive == radioButton && productMaterial.IsActive)
                {
                    return -3;
                }
                if (productMaterial.IsActive == radioButton && !productMaterial.IsActive)
                {
                    return -4;
                }
                if (radioButton)
                {
                    mmb.ChangeStatus(materialId, radioButton);
                    return 1;
                }
                else
                {
                    if (lstReciple.Count != 0)
                    {
                        return -1;
                    }
                    else
                    {
                        mmb.ChangeStatus(materialId, radioButton);
                        return 1;
                    }
                }
            }
            catch
            {
                return -2;
            }
        }

        [HttpGet]
        public ActionResult AddMaterial(string strURL)
        {
            try
            {
                ViewBag.TreeView = "productMaterial";
                ViewBag.TreeViewMenu = "addProductMaterial";
                ViewBag.previousURL = strURL;
                return View();
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        [HttpPost]
        public int AddMaterial(FormCollection f)
        {
            try
            {
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                string materialName = f["txtName"].ToString();
                string materialUnit = f.Get("txtUnit").ToString();
                int materialSQuantity = Convert.ToInt32(f["txtSQuantity"]);
                var materialList = mmb.GetMaterial();
                for (int i = 0; i < materialList.Count; i++)
                {
                    if (StringComparer.CurrentCultureIgnoreCase.Equals(materialName, materialList[i].ProductMaterialName))
                    {
                        //TempData["Error"] = String.Format("{0}{1}", materialName, " đã tồn tại");
                        return -2;
                    }
                }
                if (ModelState.IsValid)
                {
                    mmb.AddMaterial(materialName, materialUnit, materialSQuantity);
                    return 1;
                }
                return -1;
            }
            catch
            {
                return -3;
            }
        }

        [HttpPost]
        public ActionResult ListMaterial(int productId)
        {
            try
            {
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                ViewBag.productId = productId;
                var material = mmb.MaterialPartial(productId);
                return PartialView("ListPartial", material);
            }
            catch
            {
                return null;
            }
        }

        private void OnChange2(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange2(e.Info, e.Source, e.Type);
        }

    }
}