using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class DiscardInputMaterialController : Controller
    {
        private static BMAEntities db = new BMAEntities();
        //
        // GET: /DiscardInputMaterial/
        public ActionResult DiscardInputMaterialIndex()
        {
            return View();
        }
        [HttpPost]
        public ActionResult DiscardInputMaterial(int inputMaterialId, int discardQuantity, string discardNote)
        {
            DiscardedInputMaterial discardInputMaterial = db.DiscardedInputMaterials.SingleOrDefault(m => m.InputMaterialId == inputMaterialId);
            try
            {
                discardInputMaterial.InputMaterialId = inputMaterialId;
                discardInputMaterial.DiscardQuantity = discardQuantity;
                discardInputMaterial.DiscardNote = discardNote;
                discardInputMaterial.DiscardDate = DateTime.Now;
                db.SaveChanges();
            }
            catch (Exception e)
            {

                return View(@Url.Action("ManageError","Error"));
            }

            return RedirectToAction("DiscardInputMaterial");
        }

        #region Get discard by input material id
        public ActionResult GetDiscardTable(int id)
        {
            List<DiscardedInputMaterial> discardedInputMaterialList = db.DiscardedInputMaterials.Where(n => n.InputMaterialId == id).ToList();

            if (discardedInputMaterialList!=null)
            {
                return PartialView("DiscardPartialView", discardedInputMaterialList);
            }
            else
            {
                return null;
            }
        }
        #endregion

	}
}