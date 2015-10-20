using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class ManageMaterialController : Controller
    {
        BMAEntities db = new BMAEntities();
        //
        // GET: /ManageMaterial/
        public ActionResult Index()
        {
            var material = db.ProductMaterials.ToList();
            return View(material);
        }
        [HttpGet]
        public ActionResult AddMaterial()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddMaterial(FormCollection f)
        {
            string materialName = f["txtName"].ToString();
            string materialUnit = f.Get("txtUnit").ToString();
            int quantity = int.Parse(f.Get("txtQuantity").ToString());
            ProductMaterial productMaterial = new ProductMaterial();
            var materialList = db.ProductMaterials.ToList();
            for (int i = 0; i < materialList.Count; i++)
            {
                if (materialName == materialList[i].ProductMaterialName)
                {
                    TempData["Error"] = String.Format("{0}{1}",materialName," đã tồn tại");
                    return RedirectToAction("AddMaterial");
                }
            }
            if (ModelState.IsValid)
            {
                productMaterial.ProductMaterialName = materialName;
                productMaterial.ProductMaterialUnit = materialUnit;
                productMaterial.CurrentQuantity = quantity;
                db.ProductMaterials.Add(productMaterial);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
    }
}