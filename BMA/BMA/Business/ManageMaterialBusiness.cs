using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class ManageMaterialBusiness
    {
        private readonly BMAEntities db;
        public ManageMaterialBusiness()
        {
            db = new BMAEntities();
        }

        public List<ProductMaterial> GetMaterial()
        {
            List<ProductMaterial> productMaterial = db.ProductMaterials.ToList();
            return productMaterial;
        }

        public bool AddMaterial(string materialName, string materialUnit, int quantity)
        {
            ProductMaterial productMaterial = new ProductMaterial();
            productMaterial.ProductMaterialName = materialName;
            productMaterial.ProductMaterialUnit = materialUnit;
            productMaterial.CurrentQuantity = quantity;
            productMaterial.IsActive = true;
            db.ProductMaterials.Add(productMaterial);
            db.SaveChanges();
            return true;
        }

        public List<ProductMaterial> MaterialPartial(int productId)
        {
            var productMaterial = db.Recipes.Where(p => p.ProductId == productId).ToList();
            List<ProductMaterial> material = db.ProductMaterials.ToList();
            for (int i = 0; i < material.Count; i++)
            {
                for (int j = 0; j < productMaterial.Count; j++)
                {
                    if (material[i].ProductMaterialId == productMaterial[j].ProductMaterialId)
                    {
                        material.RemoveAt(i);
                    }
                }
            }
            return material;
        }
    }
}