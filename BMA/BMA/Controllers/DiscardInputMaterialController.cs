using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;
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


        #region Get discard by input material id

        public ActionResult GetDiscardTable(int id)
        {
            try
            {
                
            List<DiscardedInputMaterial> discardedInputMaterialList =
                db.DiscardedInputMaterials.Where(n => n.InputMaterialId == id).ToList();
            return PartialView("DiscardPartialView", discardedInputMaterialList);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        [HttpPost]
        #region Add discard for input material
        public int DiscardInputMaterial(FormCollection f)
        {
            try
            {
                db = new BMAEntities();
                String discardQuantityString = f["discardQuantity"];
                String discardNote = f["discardNote"];
                String inputMaterialIdString = f["InputMaterialId"];
                String productMaterialIdString = f["ProductMaterialId"];

                int inputMaterialId = Convert.ToInt32(inputMaterialIdString);
                int discardQuantity = Convert.ToInt32(discardQuantityString);
                int productMaterialId = Convert.ToInt32(productMaterialIdString);

                InputMaterial inputMaterial = db.InputMaterials.FirstOrDefault(m => m.InputMaterialId == inputMaterialId);
                int checkQuantity = discardQuantity - inputMaterial.RemainQuantity;

                DiscardedInputMaterial discardedInputMaterial = new DiscardedInputMaterial();
                if (checkQuantity > 0)
                {
                    return -1;
                }
                else
                {
                    try
                    {
                        discardedInputMaterial.DiscardDate = DateTime.Now;
                        discardedInputMaterial.InputMaterialId = inputMaterialId;
                        discardedInputMaterial.DiscardNote = discardNote;
                        discardedInputMaterial.DiscardQuantity = discardQuantity;

                    }
                    catch (Exception)
                    {
                        return 0;

                    }

                    bool result = DiscardInputMaterialBusiness.DiscardInputMaterial(discardedInputMaterial, inputMaterialId,
                        productMaterialId);
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
            catch (Exception)
            {
                return 0;
            }
            
        }
        #endregion

    }
}