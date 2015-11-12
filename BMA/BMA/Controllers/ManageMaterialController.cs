﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Business;

namespace BMA.Controllers
{
    public class ManageMaterialController : Controller
    {
        BMAEntities db = new BMAEntities();
        //
        // GET: /ManageMaterial/
        public ActionResult Index()
        {
            ManageMaterialBusiness mmb = new ManageMaterialBusiness();
            var material = mmb.GetMaterial();
            return View(material);
        }

        public ActionResult Detail(int materialId)
        {
            ManageMaterialBusiness mmb = new ManageMaterialBusiness();
            var material = mmb.GetMaterialDetail(materialId);
            var product = mmb.GetListProduct(materialId);
            ViewBag.ProductUse = product;
            return View(material);
        }

        [HttpGet]
        public ActionResult Edit(int materialId)
        {
            ManageMaterialBusiness mmb = new ManageMaterialBusiness();
            var material = mmb.GetMaterialDetail(materialId);
            return View(material);
        }

        [HttpPost, ActionName("Edit")]
        public int EditConfirm(int materialId, FormCollection f)
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
                        //TempData["Error"] = String.Format("{0}{1}", materialName, " đã tồn tại");
                        return -2;
                    }
                }
            }
            if (ModelState.IsValid)
            {
                mmb.Edit(materialId, materialName, materialUnit, materialSQuantity);
                return 1;
            }
            return -1;
        }
        public ActionResult ChangeStatus(int materialId, bool status, string strURL)
        {
            try
            {
                ManageMaterialBusiness mmb = new ManageMaterialBusiness();
                var radioButton = Convert.ToBoolean(Request.Form["status"]);
                mmb.ChangeStatus(materialId, radioButton);
                return Redirect(strURL);
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpGet]
        public ActionResult AddMaterial()
        {
            return View();
        }

        [HttpPost]
        public int AddMaterial(FormCollection f)
        {
            ManageMaterialBusiness mmb = new ManageMaterialBusiness();
            string materialName = f["txtName"].ToString();
            string materialUnit = f.Get("txtUnit").ToString();
            int materialSQuantity = Convert.ToInt32(f["txtSQuantity"]);
            var materialList = mmb.GetMaterial();
            for (int i = 0; i < materialList.Count; i++)
            {
                if (materialName == materialList[i].ProductMaterialName)
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

        [HttpPost]
        public ActionResult ListMaterial(int productId)
        {
            ManageMaterialBusiness mmb = new ManageMaterialBusiness();
            ViewBag.productId = productId;
            var material = mmb.MaterialPartial(productId);
            return PartialView("ListPartial", material);
        }
    }
}