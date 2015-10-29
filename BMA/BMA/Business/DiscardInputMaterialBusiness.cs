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
        public bool AddDiscardInputMaterial(DiscardedInputMaterial discardInputMaterial)
        {
            if (discardInputMaterial == null)
            {
                return false;
            }
            try
            {
                db.DiscardedInputMaterials.Add(discardInputMaterial);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return false;
            }
            return true; 
            
        }
    }
}