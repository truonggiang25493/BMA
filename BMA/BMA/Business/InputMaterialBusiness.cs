using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using BMA.Models;
using BMA.Models.ViewModel;
using Microsoft.Owin;

namespace BMA.Business
{
    public class InputMaterialBusiness
    {
        private static BMAEntities db;

        public InputMaterialBusiness()
        {
            db = new BMAEntities();
        }

        public static bool CheckProductMaterial(int productMaterialId, int importQuantity, int inputMaterialId)
        {
            ProductMaterial productMaterial = new ProductMaterial();
            if (inputMaterialId == 0)
            {
                productMaterial = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == productMaterialId && (n.CurrentQuantity + importQuantity) < n.StandardQuantity);
            }
            else
            {
                InputMaterial inputMaterialDetail = db.InputMaterials.SingleOrDefault(n => n.InputMaterialId == inputMaterialId);
                int changeInputMaterialQuantity = importQuantity - inputMaterialDetail.RemainQuantity;
                productMaterial = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == productMaterialId && (n.CurrentQuantity + changeInputMaterialQuantity) < n.StandardQuantity);
            }
            if (productMaterial != null)
            {
                return true;
            }
            return false;
        }

        #region Get Input Material List
        public static List<InputMaterial> GetInputMaterialList()
        {
            List<InputMaterial> inputMaterialslList = db.InputMaterials.OrderByDescending(n => n.ImportDate).ThenByDescending(n => n.IsActive).ToList();
            return inputMaterialslList;
        }
        #endregion

        #region Get Input Material List
        public void CheckInputMaterialListStartup()
        {
            List<InputMaterial> inputMaterialslList = db.InputMaterials.ToList();

            foreach (var inputMaterial in inputMaterialslList)
            {
                if (DateTime.Compare(inputMaterial.InputMaterialExpiryDate, DateTime.Today) < 1)
                {
                    inputMaterial.IsActive = false;
                }
            }
            db.SaveChanges();
        }
        #endregion

        #region View input material detail

        public InputMaterial GetInputMaterial(int id)
        {
            InputMaterial inputMaterialDetail = db.InputMaterials.SingleOrDefault(n => n.InputMaterialId == id);
            return inputMaterialDetail;

        }
        #endregion

        #region Change input material stage
        public static bool ChangeInputMaterialStage(int id)
        {
            var inputMaterialDetail = db.InputMaterials.SingleOrDefault(n => n.InputMaterialId == id);
            if (inputMaterialDetail != null)
            {
                if (inputMaterialDetail.IsActive)
                {
                    inputMaterialDetail.IsActive = false;

                }
                else
                {
                    inputMaterialDetail.IsActive = true;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return false;
                }
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region Edit input material
        public static bool EditInputMaterial(int inputMaterialId, int importQuantity, int productMaterialId, int inputMaterialPrice, DateTime importDate,
            DateTime inputMaterialExpiryDate, int inputBillId, String inputMaterialNote)
        {
            var inputMaterialDetail = db.InputMaterials.SingleOrDefault(n => n.InputMaterialId == inputMaterialId);
            var productMaterial = db.ProductMaterials.SingleOrDefault(n => n.ProductMaterialId == productMaterialId);

            if (inputMaterialDetail != null)
            {
                try
                {
                    inputMaterialDetail.ProductMaterial.ProductMaterialId = productMaterialId;
                    inputMaterialDetail.ImportQuantity = importQuantity;
                    inputMaterialDetail.InputMaterialPrice = inputMaterialPrice;
                    inputMaterialDetail.ImportDate = importDate;
                    inputMaterialDetail.InputMaterialExpiryDate = inputMaterialExpiryDate;
                    inputMaterialDetail.InputBillId = inputBillId;
                    inputMaterialDetail.InputMaterialNote = inputMaterialNote;
                    int changeInputMaterialQuantity = importQuantity - inputMaterialDetail.RemainQuantity;
                    inputMaterialDetail.RemainQuantity = importQuantity;
                    productMaterial.CurrentQuantity = productMaterial.CurrentQuantity + changeInputMaterialQuantity;
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return false;
                }
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region Add new input material
        public static bool AddInputMaterial(int productMaterialId, InputMaterial inputMaterial, int importQuantity)
        {
            if (inputMaterial == null)
            {
                return false;
            }
            try
            {
                ProductMaterial productMaterial = db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == productMaterialId);
                productMaterial.CurrentQuantity = productMaterial.CurrentQuantity + importQuantity;
                db.InputMaterials.Add(inputMaterial);
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