using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using System.Net;
using System.Data;

namespace BMA.Controllers
{
    public class ManageProductController : Controller
    {
        BMAEntities db = new BMAEntities();
        public ActionResult Index()
        {
            var product = db.Products.ToList();
            return View(product);
        }

        public ActionResult Create()
        {
            return View();
        }
        public ActionResult Detail(int productId)
        {
            var product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            var productMaterial = db.Recipes.Where(p => p.ProductId == product.ProductId).ToList();
            ViewBag.ProductMaterial = productMaterial;
            return View(product);
        }

        public ActionResult Edit(int? productId)
        {
            if (productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(productId);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirm(int? productId, FormCollection f)
        {
            if (productId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var productToUpdate = db.Products.Find(productId);
            string proName = f["productName"].ToString();
            string proDes = f["productDes"].ToString();
            string proNote = f["productNote"].ToString();
            if (TryUpdateModel(productToUpdate))
            {
                try
                {
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (DataException)
                {
                    ModelState.AddModelError("", "Không thể sửa. Xin vui lòng thử lại.");
                }
            }
            return View(productToUpdate);
        }

        public ActionResult ChangeStatus(int productId, bool status, string strURL)
        {
            var product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            var radioButton = Convert.ToBoolean(Request.Form["status"]);
            product.IsActive = radioButton;
            db.SaveChanges();
            return Redirect(strURL);
        }

        public ActionResult ProductMaterial(int productId)
        {
            var productMaterial = db.Recipes.Where(p => p.ProductId == productId).ToList();
            var product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            ViewBag.productName = product.ProductName;
            ViewBag.productId = product.ProductId;
            return View(productMaterial);
        }

        public ActionResult DeleteProductMaterial(int productId, int productMaterialId, string strURL)
        {
            Recipe productRecipes = db.Recipes.SingleOrDefault(n => n.ProductId == productId && n.ProductMaterialId == productMaterialId);
            db.Recipes.Remove(productRecipes);
            db.SaveChanges();
            return Redirect(strURL);
        }

        public ActionResult UpdateProductMaterial(FormCollection f, int productId, string strURL)
        {
            var productMaterial = db.Recipes.Where(p => p.ProductId == productId).ToList();
            String[] lstQuantity = f["txtQuantity"].ToString().Split(',');
            for (int i = 0; i < productMaterial.Count; i++)
            {
                productMaterial[i].RecipeQuantity = int.Parse(lstQuantity[i]);
            }
            db.SaveChanges();
            return Redirect(strURL);
        }

        public ActionResult AddProductMaterial(FormCollection f, int productId, int productMaterialId, string strURL)
        {
            string quantity = f["txtMQuantity"].ToString();
            Recipe recipe = new Recipe();
            recipe.ProductId = productId;
            recipe.ProductMaterialId = productMaterialId;
            recipe.RecipeQuantity = Convert.ToDouble(quantity);
            db.Recipes.Add(recipe);
            db.SaveChanges();
            return Redirect(strURL);
        }

        public ActionResult ListMaterial(int productId)
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
            return PartialView(material);
        }
    }
}