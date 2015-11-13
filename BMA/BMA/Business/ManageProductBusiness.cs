using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;
using System.Data.Entity;

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
            var category = db.Categories.Where(n => n.CategoryId != 1).ToList();
            return category;
        }

        public bool AddProduct(string productName, string productUnit, double productWeight, string productDes, string productNote, int productPrice, int dropCate, string productCode, string productImage, int[] materialId, int[] materialQuantity)
        {
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
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
            int productId = GetProductId();
            for (int i = 0; i < materialId.Length; i++)
            {
                Recipe recipe = new Recipe();
                recipe.ProductId = productId;
                recipe.ProductMaterialId = materialId[i];
                recipe.RecipeQuantity = materialQuantity[i];
                db.Recipes.Add(recipe);
                db.SaveChanges();
            }
            try
            {
                contextTransaction.Commit();
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
            }
            finally
            {
                contextTransaction.Dispose();
            }
            return true;
        }

        public int GetProductId()
        {
            List<Product> lstProduct = db.Products.OrderBy(n => n.ProductId).ToList();
            int productId = lstProduct.LastOrDefault().ProductId;
            return productId;
        }

        public bool CheckEditInformation(int productId, int productPrice, int[] materialId, int[] materialQuantity)
        {
            Product product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            int checkChangeMaterial = 0;
            if (product.ProductStandardPrice != productPrice)
            {
                return true;
            }
            List<Recipe> recipe = db.Recipes.Where(n => n.ProductId == productId).ToList();
            if (recipe.Count != materialId.Length)
            {
                return true;
            }
            for (int i = 0; i < recipe.Count; i++)
            {
                for (int j = 0; j < materialId.Length; j++)
                {
                    if (recipe[i].ProductMaterialId == materialId[j])
                    {
                        checkChangeMaterial++;
                        if (recipe[i].RecipeQuantity != materialQuantity[j])
                        {
                            return true;
                        }
                    }
                }
            }
            if (checkChangeMaterial != recipe.Count)
            {
                return true;
            }
            return false;
        }
        public bool EditProduct(int productId, string productName, string productUnit, double productWeight, string productDes, string productNote, int productPrice, int dropCate, string productCode, string productImage, int[] materialId, int[] materialQuantity)
        {
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            bool checkEdit = CheckEditInformation(productId, productPrice, materialId, materialQuantity);
            if (checkEdit)
            {
                Product productOld = db.Products.SingleOrDefault(n => n.ProductId == productId);
                productOld.IsActive = false;
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
                int newProductId = GetProductId();
                for (int i = 0; i < materialId.Length; i++)
                {
                    Recipe recipe = new Recipe();
                    recipe.ProductId = newProductId;
                    recipe.ProductMaterialId = materialId[i];
                    recipe.RecipeQuantity = materialQuantity[i];
                    db.Recipes.Add(recipe);
                    db.SaveChanges();
                }
                db.SaveChanges();
            }
            else
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
                product.CategoryId = dropCate;
                product.ProductCode = productCode;
                if (productImage != null)
                {
                    product.ProductImage = productImage;
                }
                product.IsActive = true;
                db.SaveChanges();
            }
            try
            {
                contextTransaction.Commit();
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
            }
            finally
            {
                contextTransaction.Dispose();
            }
            return true;
        }

        //public bool AddMaterialToProduct(int productId, int[] materialId, int[] quantity)
        //{
        //    DbContextTransaction contextTransaction = db.Database.BeginTransaction();
        //    for (int i = 0; i < materialId.Length; i++)
        //    {
        //        Recipe recipe = new Recipe();
        //        recipe.ProductId = productId;
        //        recipe.ProductMaterialId = materialId[i];
        //        recipe.RecipeQuantity = quantity[i];
        //        db.Recipes.Add(recipe);
        //        db.SaveChanges();
        //    }
        //    try
        //    {
        //        contextTransaction.Commit();
        //    }
        //    catch (Exception)
        //    {
        //        contextTransaction.Rollback();
        //    }
        //    finally
        //    {
        //        contextTransaction.Dispose();
        //    }
        //    return true;
        //}

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

        public bool UpdateProductMaterial(int productId, string[] lstQuantity)
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