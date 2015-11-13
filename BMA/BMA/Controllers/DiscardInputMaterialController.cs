﻿using System;
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
            List<DiscardedInputMaterial> discardedInputMaterialList =
                db.DiscardedInputMaterials.Where(n => n.InputMaterialId == id).ToList();
            return PartialView("DiscardPartialView", discardedInputMaterialList);
        }

        #endregion

        [HttpPost]
        #region Add discard for input material
        public int DiscardInputMaterial(FormCollection f)
        {
            String discardQuantityString = f["discardQuantity"];
            String discardNote = f["discardNote"];
            String inputMaterialIdString = f["InputMaterialId"];
            int inputMaterialId = Convert.ToInt32(inputMaterialIdString);
            int discardQuantity = Convert.ToInt32(discardQuantityString);

            DiscardedInputMaterial discardedInputMaterial = new DiscardedInputMaterial();
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

            bool result = DiscardInputMaterialBusiness.DiscardInputMaterial(discardedInputMaterial,inputMaterialId);
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