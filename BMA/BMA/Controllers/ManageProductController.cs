using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Models.ViewModel;
using System.Net;
using System.Data;
using BMA.Business;
using System.IO;

namespace BMA.Controllers
{
    public class ManageProductController : Controller
    {
        BMAEntities db = new BMAEntities();
        public ActionResult Index()
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var product = mpb.GetProduct();
            return View(product);
        }

        [HttpGet]
        public ActionResult AddProduct()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] == 3)
            {
                return RedirectToAction("Index", "Home");
            }
            if ((int)Session["UserRole"] == 1)
            {
                return RedirectToAction("Index", "StoreInfor");
            }
            ManageProductBusiness mpb = new ManageProductBusiness();
            Policy maxPricePolicy = db.Policies.SingleOrDefault(n => n.PolicyId == 2);
            int maxPrice = maxPricePolicy.PolicyBound;
            ViewBag.maxPrice = maxPrice;
            var category = mpb.GetCategory();
            ViewBag.category = category;
            InitiateMaterialList(null);
            return View();
        }

        [HttpPost]
        public int AddImage(HttpPostedFileBase file, FormCollection f)
        {
            var allowedExtensions = new[] {  
            ".Jpg", ".png", ".jpg", "jpeg", ".JPG", ".PNG", ".JPEG"  
            };
            var maxSize = 1048576;
            var fileName = "";
            if (file != null)
            {
                var productSize = file.ContentLength;
                fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                if (allowedExtensions.Contains(ext))
                {
                    if (productSize <= maxSize)
                    {
                        var comparePath = Server.MapPath(string.Format("{0}{1}", "~/Content/Images/BakeryImages", fileName));
                        if (!System.IO.File.Exists(comparePath))
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/Images/BakeryImages"), fileName);
                            file.SaveAs(path);
                        }
                        return 1;
                    }
                    else
                    {
                        //TempData["Message"] = "Kích thước hình ảnh quá lớn";
                        string strURL = Request.UrlReferrer.AbsolutePath;
                        return -2;
                    }
                }
                else
                {
                    //TempData["Message"] = "Xin chọn file hình ảnh";
                    string strURL = Request.UrlReferrer.AbsolutePath;
                    return -3;
                }
            }
            else
            {
                string fileBackup = f["fileBackup"];
                if (fileBackup != null || fileBackup != "")
                {
                    return 1;
                }
                return -1;
            }
        }

        [HttpPost]
        public int EditImage(HttpPostedFileBase file)
        {
            var allowedExtensions = new[] {  
            ".Jpg", ".png", ".jpg", "jpeg", ".JPG", ".PNG", ".JPEG"  
            };
            var maxSize = 1048576;
            var fileName = "";
            if (file != null)
            {
                var productSize = file.ContentLength;
                fileName = Path.GetFileName(file.FileName);
                var ext = Path.GetExtension(file.FileName);
                if (allowedExtensions.Contains(ext))
                {
                    if (productSize <= maxSize)
                    {
                        var comparePath = Server.MapPath(string.Format("{0}{1}", "~/Content/Images/BakeryImages", fileName));
                        if (!System.IO.File.Exists(comparePath))
                        {
                            var path = Path.Combine(Server.MapPath("~/Content/Images/BakeryImages"), fileName);
                            file.SaveAs(path);
                        }
                        return 1;
                    }
                    else
                    {
                        //TempData["Message"] = "Kích thước hình ảnh quá lớn";
                        string strURL = Request.UrlReferrer.AbsolutePath;
                        return -2;
                    }
                }
                else
                {
                    //TempData["Message"] = "Xin chọn file hình ảnh";
                    string strURL = Request.UrlReferrer.AbsolutePath;
                    return -3;
                }
            }
            else
            {
                return 1;
            }
        }

        public string CreateProductCode(string productName)
        {
            try
            {
                var lstProductCode = db.Products.Select(n => n.ProductCode).ToList();
                string[] productCodeArray = productName.Split(' ');
                string productWord = "";
                string productNumber = "001";
                string productCode = "";
                string upperLetter = "";
                try
                {
                    for (int i = 0; i < productCodeArray.Length; i++)
                    {
                        upperLetter = productCodeArray[i].Substring(0, 1).ToUpper();
                        productWord += upperLetter;
                    }
                }
                catch
                {
                    productWord = "APEA";
                }

                if (productWord.Length < 4)
                {
                    for (int j = productWord.Length; j < 4; j++)
                    {
                        productWord += "X";
                    }
                }
                if (productWord.Length > 4)
                {
                    productWord = productWord.Substring(0, 4);
                }
                for (int i = 0; i < lstProductCode.Count; i++)
                {
                    if (productWord.Equals(lstProductCode[i].Substring(0, 4)))
                    {
                        int subNumber = Convert.ToInt32(lstProductCode[i].Substring(4, 3));
                        productNumber = (subNumber + 1).ToString();
                        if (productNumber.Length < 3)
                        {
                            if (productNumber.Length < 2)
                            {
                                productNumber = string.Format("{0}{1}{2}", "0", "0", productNumber);
                            }
                            else
                            {
                                productNumber = string.Format("{0}{1}", "0", productNumber);
                            }
                        }
                    }
                }
                productCode = string.Format("{0}{1}", productWord, productNumber);

                return productCode;
            }
            catch
            {
                return "Error";
            }
        }

        [HttpPost]
        public int AddProduct(string productName, string productUnit, double productWeight, string productDes, string productNote, int productPrice, int dropCate, string fileName, int[] materialId, int[] materialQuantity)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            //Check that product name had existed
            var productList = mpb.GetActiveProduct();
            string productCode = CreateProductCode(productName);
            for (int i = 0; i < productList.Count; i++)
            {
                if (StringComparer.CurrentCultureIgnoreCase.Equals(productName, productList[i].ProductName))
                {
                    //string strURL = Request.UrlReferrer.AbsolutePath;
                    //TempData["Error"] = String.Format("{0}{1}", productName, " đã tồn tại");
                    return -4;
                }
            }
            try
            {
                productPrice = Convert.ToInt32(productPrice.ToString().Replace(".", ""));
                mpb.AddProduct(productName, productUnit, productWeight, productDes, productNote, productPrice, dropCate, productCode, fileName, materialId, materialQuantity);
                Session["ProductInfor"] = null;
                Session["Material"] = null;
                return 2;
            }
            catch (Exception)
            {
                return -5;
            }
        }
        public ActionResult Detail(int productId)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var product = mpb.GetProductDetail(productId);
            var productMaterial = mpb.GetListMaterial(productId);
            ViewBag.ProductMaterial = productMaterial;
            return View(product);
        }

        public ActionResult Edit(int productId)
        {
            try
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
                if ((int)Session["UserRole"] == 1)
                {
                    return RedirectToAction("Index", "StoreInfor");
                }
                ManageProductBusiness mpb = new ManageProductBusiness();
                var product = mpb.GetProductDetail(productId);
                var materialList = mpb.GetListMaterial(productId);
                var category = mpb.GetCategory();
                Policy maxPricePolicy = db.Policies.SingleOrDefault(n => n.PolicyId == 2);
                int maxPrice = maxPricePolicy.PolicyBound;
                ViewBag.maxPrice = maxPrice;
                ViewBag.materialList = materialList;
                ViewBag.category = category;
                if (product == null)
                {
                    return HttpNotFound();
                }
                InitiateMaterialList(productId);
                return View(product);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        [HttpPost, ActionName("Edit")]
        public int EditConfirm(int productId, string productName, string productUnit, double productWeight, string productDes, string productNote, int productPrice, int dropCate, string fileName, int[] materialId, int[] materialQuantity)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var productToUpdate = mpb.GetProductDetail(productId);
            string productCode = CreateProductCode(productName);
            var productList = mpb.GetActiveProduct();
            if (!StringComparer.CurrentCultureIgnoreCase.Equals(productName, productToUpdate.ProductName))
            {
                for (int i = 0; i < productList.Count; i++)
                {
                    if (StringComparer.CurrentCultureIgnoreCase.Equals(productName, productList[i].ProductName))
                    {
                        //string strURL = Request.UrlReferrer.AbsolutePath;
                        //string URL = String.Format("{0}{1}{2}", strURL, "?ProductId=", productId);
                        //TempData["Error"] = String.Format("{0}{1}", productName, " đã tồn tại");
                        return -4;
                    }
                }
            }
            try
            {
                productPrice = Convert.ToInt32(productPrice.ToString().Replace(".", ""));
                mpb.EditProduct(productId, productName, productUnit, productWeight, productDes, productNote, productPrice, dropCate, productCode, fileName, materialId, materialQuantity);
                return 2;
            }
            catch (DataException)
            {
                return -5;
            }
        }

        public int ChangeStatus(int productId, bool status, string strURL)
        {
            try
            {
                ManageProductBusiness mpb = new ManageProductBusiness();
                Product product = db.Products.SingleOrDefault(n => n.ProductId == productId);
                var radioButton = Convert.ToBoolean(Request.Form["status"]);
                if (product.IsActive == radioButton && product.IsActive)
                {
                    return -2;
                }
                if (product.IsActive == radioButton && !product.IsActive)
                {
                    return -3;
                }

                mpb.ChangeStatus(productId, radioButton);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public ActionResult ProductMaterial(int productId)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            var productMaterial = mpb.GetListMaterial(productId);
            var product = mpb.GetProductDetail(productId);
            ViewBag.productName = product.ProductName;
            ViewBag.productId = product.ProductId;
            return View(productMaterial);
        }

        public ActionResult DeleteProductMaterial(int productId, int productMaterialId, string strURL)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            mpb.DeleteProductMaterial(productId, productMaterialId);
            return Redirect(strURL);
        }

        public ActionResult UpdateProductMaterial(FormCollection f, int productId, string strURL)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            String[] lstQuantity = f["txtQuantity"].ToString().Split(',');
            mpb.UpdateProductMaterial(productId, lstQuantity);
            return Redirect(strURL);
        }

        [HttpPost]
        public ActionResult AddProductMaterial(FormCollection form)
        {
            ManageProductBusiness mpb = new ManageProductBusiness();
            int productId = int.Parse(form["productId"]);
            int materialId = int.Parse(form["productMaterialId"]);
            string strURL = String.Format("{0}{1}", "/ManageProduct/ProductMaterial?ProductId=", productId);
            mpb.AddProductMaterial(productId, materialId);
            return Redirect(strURL);
        }

        public int AddTempProductInfor(string productName, string productUnit, double? productWeight, string productDes, string productNote, int? productPrice, int dropCate, string fileName, int[] materialId, int[] materialQuantity)
        {
            try
            {
                Product product = new Product();
                product.ProductName = productName;
                product.Unit = productUnit;
                if (productWeight == null)
                {
                    product.ProductWeight = -1;
                }
                else
                {
                    product.ProductWeight = productWeight.Value;
                }

                product.Descriptions = productDes;
                product.Note = productNote;
                if (productPrice == null)
                {
                    product.ProductStandardPrice = -1;
                }
                else
                {
                    product.ProductStandardPrice = productPrice.Value;
                }

                product.CategoryId = dropCate;
                product.ProductImage = fileName;

                Session["ProductInfor"] = product;

                ManageProductBusiness mpb = new ManageProductBusiness();
                List<AllProductInfoViewModel> apivList = new List<AllProductInfoViewModel>();
                if (materialId != null && materialQuantity != null && materialQuantity.Length > 0 && materialId.Length > 0)
                {
                    for (int i = 0; i < materialId.Length; i++)
                    {
                        var pm = mpb.GetMaterialById(materialId[i]);
                        AllProductInfoViewModel apiv = new AllProductInfoViewModel
                        {
                            MaterialId = pm.ProductMaterialId,
                            MaterialName = pm.ProductMaterialName,
                            MaterialUnit = pm.ProductMaterialUnit,
                            MaterialQuantity = materialQuantity[i]
                        };
                        apivList.Add(apiv);
                    }
                }

                if (apivList.Count > 0)
                {
                    Session["Material"] = apivList;
                }
                return 1;
            }
            catch
            {
                return -1;
            }
        }
        private void InitiateMaterialList(int? productId)
        {
            List<ProductMaterial> materialList = db.ProductMaterials.Where(n => n.IsActive).ToList();
            if (productId == null)
            {
                List<AllProductInfoViewModel> materialToProductList = Session["Material"] as List<AllProductInfoViewModel>;
                if (materialToProductList != null)
                {
                    foreach (var item in materialToProductList)
                    {
                        materialList.Remove(materialList.FirstOrDefault(n => n.ProductMaterialId == item.MaterialId));
                    }
                }
                Session["MaterialListToAdd"] = materialList;
            }
            else
            {
                List<ProductMaterial> resultList = new List<ProductMaterial>();
                List<Recipe> recipe = db.Recipes.Where(n => n.ProductId == productId).ToList();
                if (materialList.Count > 0 && recipe != null)
                {
                    foreach (ProductMaterial pm in materialList)
                    {
                        bool check = true;
                        foreach (var item in recipe)
                        {
                            if (item.ProductMaterialId == pm.ProductMaterialId)
                            {
                                check = false;
                            }
                        }
                        if (check)
                        {
                            resultList.Add(pm);
                        }
                    }
                }
                Session["MaterialList"] = resultList;
            }
        }

        [HttpPost]
        public ActionResult GetListMaterialToAdd(int? productId)
        {
            // Check autherization
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                if (productId != null)
                {
                    List<ProductMaterial> materialList = Session["MaterialList"] as List<ProductMaterial>;
                    //materialList = Session["MaterialList"] as List<ProductMaterial>;

                    return PartialView(materialList);
                }
                else
                {
                    List<ProductMaterial> materialList = new List<ProductMaterial>();
                    materialList = Session["MaterialListToAdd"] as List<ProductMaterial>;

                    return PartialView(materialList);
                }

            }
        }

        public int RemoveMaterialFromListToAdd(int[] materialId)
        {
            List<ProductMaterial> lstMaterial = Session["MaterialListToAdd"] as List<ProductMaterial>;
            if (materialId != null)
            {
                if (materialId.Length > 0)
                {
                    foreach (int index in materialId)
                    {
                        if (lstMaterial != null && lstMaterial.Count > 0)
                        {
                            lstMaterial.Remove(lstMaterial.SingleOrDefault(n => n.ProductMaterialId == index));
                        }
                    }
                    Session["MaterialListToAdd"] = lstMaterial;
                    return 1;
                }
                return 0;
            }
            else
            {
                return -1;
            }
        }

        public int AddMaterialInMaterialListToAdd(int materialId)
        {
            List<ProductMaterial> materialList = Session["MaterialListToAdd"] as List<ProductMaterial>;
            ProductMaterial productMaterial = db.ProductMaterials.FirstOrDefault(n => n.ProductMaterialId == materialId && n.IsActive);
            if (productMaterial != null)
            {
                if (materialList != null && materialList.Count > 0)
                {
                    bool check = true;
                    foreach (ProductMaterial material in materialList)
                    {
                        if (material.ProductMaterialId == materialId)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        materialList.Add(productMaterial);
                    }
                    return 1;
                }
                return 0;
            }
            return 0;
        }

        [HttpPost]
        public int RemoveMaterialInProductList(int[] materialId)
        {
            List<ProductMaterial> materialList = Session["MaterialList"] as List<ProductMaterial>;
            if (materialId != null)
            {
                if (materialId.Length > 0)
                {
                    foreach (int index in materialId)
                    {
                        if (materialList != null && materialList.Count > 0)
                        {
                            materialList.Remove(materialList.FirstOrDefault(m => m.ProductMaterialId == index));
                        }
                    }
                    Session["MaterialList"] = materialList;
                    return 1;
                }
                return 0;
            }
            else
            {
                return -1;
            }
        }

        [HttpPost]
        public int AddMaterialInMaterialList(int materialId)
        {
            List<ProductMaterial> materialList = Session["MaterialList"] as List<ProductMaterial>;
            ProductMaterial productMaterial = db.ProductMaterials.FirstOrDefault(n => n.ProductMaterialId == materialId && n.IsActive);
            if (productMaterial != null)
            {
                if (materialList != null && materialList.Count > 0)
                {
                    bool check = true;
                    foreach (ProductMaterial material in materialList)
                    {
                        if (material.ProductMaterialId == materialId)
                        {
                            check = false;
                        }
                    }
                    if (check)
                    {
                        materialList.Add(productMaterial);
                    }
                    return 1;
                }
                return 0;
            }


            return 0;
        }
    }
}