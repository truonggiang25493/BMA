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

        public ProductMaterial GetMaterialDetail(int materialId)
        {
            var materialDetail = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == materialId);
            return materialDetail;
        }

        public List<Recipe> GetListProduct(int materialId)
        {
            var lstProduct = db.Recipes.Where(p => p.ProductMaterialId == materialId).ToList();
            return lstProduct;
        }
        public bool ChangeStatus(int materialId, bool status)
        {
            var material = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == materialId);
            material.IsActive = status;
            db.SaveChanges();
            return true;
        }

        public bool CheckProductMaterial(int productMaterialId, int standardQuantity)
        {
            ProductMaterial productMaterial = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == productMaterialId);
            if (productMaterial.CurrentQuantity < standardQuantity)
            {
                return true;
            }
            return false;
        }
        public bool EditMaterial(int materialId, string materialName, string materialUnit, int materialSQuantity)
        {
            ProductMaterial productMaterial = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == materialId);
            productMaterial.ProductMaterialName = materialName;
            productMaterial.ProductMaterialUnit = materialUnit;
            productMaterial.StandardQuantity = materialSQuantity;
            db.SaveChanges();
            return true;
        }
        public bool AddMaterial(string materialName, string materialUnit,int materialSQuantity)
        {
            ProductMaterial productMaterial = new ProductMaterial();
            productMaterial.ProductMaterialName = materialName;
            productMaterial.ProductMaterialUnit = materialUnit;
            productMaterial.CurrentQuantity = 0;
            productMaterial.StandardQuantity = materialSQuantity;
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