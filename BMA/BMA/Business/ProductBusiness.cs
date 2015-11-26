using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class ProductBusiness
    {
        private readonly BMAEntities db;
        public ProductBusiness()
        {
            db = new BMAEntities();
        }
        public List<Product> GetProduct()
        {
            var lstProducts = db.Products.Where(n => n.IsActive).ToList();
            return lstProducts;
        }

        public List<Product> SearchProduct(string searchString)
        {
            var lstProducts = db.Products.Where(n => n.ProductName.Contains(searchString) && n.IsActive).ToList();
            return lstProducts;
        }

        public List<Product> SearchMaterial(string searchString)
        {
            var material = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialName.Contains(searchString));
            var lstProductIds = db.Recipes.Where(n => n.ProductMaterialId == material.ProductMaterialId).Select(n => n.ProductId).ToList();
            Product product;
            List<Product> lstProducts = new List<Product>();
            foreach (var item in lstProductIds)
            {
                product = db.Products.SingleOrDefault(n => n.ProductId == item && n.IsActive);
                lstProducts.Add(product);
            }
            return lstProducts;
        }
        public List<Product> GetOtherProduct(int productId)
        {
            List<Product> lstProducts = db.Products.Where(n => n.IsActive && n.ProductId != productId).ToList();
            return lstProducts;
        }

        public List<Product> GetProductByCategory(int? categoryId)
        {
            var lstProducts = db.Products.Where(n => n.CategoryId == categoryId && n.IsActive).ToList();
            return lstProducts;
        }

        public Product GetProductDetail(int ProductId)
        {
            var productDetail = db.Products.SingleOrDefault(n => n.ProductId == ProductId);
            return productDetail;
        }

        public List<Recipe> GetProductMaterial(int ProductId)
        {
            BMAEntities db = new BMAEntities();
            var lstMaterial = db.Recipes.Where(p => p.ProductId == ProductId).ToList();
            return lstMaterial;
        }
    }
}