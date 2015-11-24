using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class DiscardInputMaterialBusiness
    {
        private static BMAEntities db = new BMAEntities();
        #region Add discard for input material
        public static bool DiscardInputMaterial(DiscardedInputMaterial discardedInputMaterial, int inputMaterialId, int productMaterialId)
        {
            InputMaterial inputMaterial = db.InputMaterials.FirstOrDefault(m => m.InputMaterialId == inputMaterialId);
            ProductMaterial productMaterial = db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == productMaterialId);
            if (discardedInputMaterial == null)
            {
                return false;
            }
            try
            {
                db.DiscardedInputMaterials.Add(discardedInputMaterial);
                inputMaterial.RemainQuantity = inputMaterial.RemainQuantity - discardedInputMaterial.DiscardQuantity;
                productMaterial.CurrentQuantity = productMaterial.CurrentQuantity - discardedInputMaterial.DiscardQuantity;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return false;
            }
            return true;
        }
        #endregion
    }
}