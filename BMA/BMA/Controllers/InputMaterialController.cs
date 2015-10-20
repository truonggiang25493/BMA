using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class InputMaterialController : Controller
    {
        private BMAEntities db = new BMAEntities();
        // GET: /InputMaterial/
        public ActionResult Index()
        {
            List<InputMaterial> InputMaterialList = db.InputMaterials.OrderBy(n=>n.InputMaterialExpiryDate).ToList();
            return View(InputMaterialList);
        }
        // GET: Detail
        public ActionResult Detail(int id)
        {
            var inputMaterialDetail = db.InputMaterials.SingleOrDefault(n => n.InputMaterialId == id);
            ViewBag.InputMaterialDetail = inputMaterialDetail;           
            if (inputMaterialDetail == null)
            {
                RedirectToAction("Index", "Error");
            }
            return View(inputMaterialDetail);
        }

	}
}