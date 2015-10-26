using System;
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
        [HttpGet]
        public ActionResult AddMaterial()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddMaterial(FormCollection f)
        {
            ManageMaterialBusiness mmb = new ManageMaterialBusiness();
            string materialName = f["txtName"].ToString();
            string materialUnit = f.Get("txtUnit").ToString();
            int quantity = int.Parse(f.Get("txtQuantity").ToString());
            var materialList = mmb.GetMaterial();
            for (int i = 0; i < materialList.Count; i++)
            {
                if (materialName == materialList[i].ProductMaterialName)
                {
                    TempData["Error"] = String.Format("{0}{1}", materialName, " đã tồn tại");
                    return RedirectToAction("AddMaterial");
                }
            }
            if (ModelState.IsValid)
            {
                mmb.AddMaterial(materialName, materialUnit, quantity);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
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