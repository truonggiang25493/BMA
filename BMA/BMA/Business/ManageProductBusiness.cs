using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class ManageProductBusiness
    {
        private readonly BMAEntities db;
        public ManageProductBusiness()
        {
            db = new BMAEntities();
        }

        public List<Product> GetProduct()
        {
            var product = db.Products.ToList();
            return product;
        }

        public List<Product> GetActiveProduct()
        {
            var product = db.Products.Where(n => n.IsActive).ToList();
            return product;
        }
        public Product GetProductDetail(int productId)
        {
            var productDetail = db.Products.SingleOrDefault(n => n.ProductId == productId);
            return productDetail;
        }

        public List<Recipe> GetListMaterial(int productId)
        {
            var lstMaterial = db.Recipes.Where(p => p.ProductId == productId).ToList();
            return lstMaterial;
        }

        public List<Category> GetCategory()
        {
            var category = db.Categories.Where(n=>n.CategoryId != 1).ToList();
            return category;
        }

        public bool AddProduct(string productName, string productUnit, double productWeight, string productDes, string productNote, int productPrice, int dropCate, string productCode, string productImage)
        {
            Product product = new Product();
            product.ProductName = productName;
            product.Unit = productUnit;
            product.ProductWeight = productWeight;
            if (productDes != null)
            {
                product.Descriptions = productDes;
            }
            if (productNote != null)
            {
                product.Note = productNote;
            }
            product.ProductStandardPrice = productPrice;
            product.CategoryId = dropCate;
            product.ProductCode = productCode;
            if (productImage != null)
            {
                product.ProductImage = productImage;
            }
            product.IsActive = true;
            db.Products.Add(product);
            db.SaveChanges();
            return true;
        }

        public int GetProductId()
        {
            List<Product> lstProduct = db.Products.OrderBy(n => n.ProductId).ToList();
            int orderCode = lstProduct.LastOrDefault().ProductId;
            return orderCode;
        }

        public bool EditProduct(int productId,string productName, string productUnit, double productWeight, string productDes, string productNote, int productPrice, int dropCate, string productCode, string productImage)
        {
            Product product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            product.ProductName = productName;
            product.Unit = productUnit;
            product.ProductWeight = productWeight;
            if (productDes != null)
            {
                product.Descriptions = productDes;
            }
            if (productNote != null)
            {
                product.Note = productNote;
            }
            product.ProductStandardPrice = productPrice;
            product.CategoryId = dropCate;
            product.ProductCode = productCode;
            if (productImage != null)
            {
                product.ProductImage = productImage;
            }
            db.SaveChanges();
            return true;
        }

        public bool ChangeStatus(int productId, bool status)
        {
            var product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            product.IsActive = status;
            db.SaveChanges();
            return true;
        }

        public bool DeleteProductMaterial(int productId, int productMaterialId)
        {
            Recipe productRecipes = db.Recipes.SingleOrDefault(n => n.ProductId == productId && n.ProductMaterialId == productMaterialId);
            db.Recipes.Remove(productRecipes);
            db.SaveChanges();
            return true;
        }

        public bool UpdateProductMaterial(int productId,string[] lstQuantity)
        {
            var productMaterial = db.Recipes.Where(p => p.ProductId == productId).ToList();
            for (int i = 0; i < productMaterial.Count; i++)
            {
                productMaterial[i].RecipeQuantity = int.Parse(lstQuantity[i]);
            }
            db.SaveChanges();
            return true;
        }

        public bool AddProductMaterial(int productId, int materialId)
        {
            Recipe recipe = new Recipe();
            recipe.ProductId = productId;
            recipe.ProductMaterialId = materialId;
            recipe.RecipeQuantity = 1;
            db.Recipes.Add(recipe);
            db.SaveChanges();
            return true;
        }
    }
}