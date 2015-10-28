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

        public List<Product> GetCookie()
        {
            var lstCookies = db.Products.Where(n => n.CategoryId == 2 && n.IsActive).ToList();
            return lstCookies;
        }

        public List<Product> GetSaltine()
        {
            var lstSaltine = db.Products.Where(n => n.CategoryId == 3 && n.IsActive).ToList();
            return lstSaltine;
        }

        public Product GetProductDetail(int ProductId)
        {
            var productDetail = db.Products.SingleOrDefault(n => n.ProductId == ProductId);
            return productDetail;
        }

        public static List<Recipe> GetProductMaterial(int ProductId)
        {
            BMAEntities db = new BMAEntities();
            var lstMaterial = db.Recipes.Where(p => p.ProductId == ProductId).ToList();
            return lstMaterial;
        }
    }
}