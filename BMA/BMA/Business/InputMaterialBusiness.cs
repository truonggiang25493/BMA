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

        #region Get Input Material List

        public static List<InputMaterial> GetInputMaterialList()
        {
            List<InputMaterial> inputMaterialslList = db.InputMaterials.OrderByDescending(n => n.InputMaterialId).ToList();
            return inputMaterialslList;
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
                    int changeInputMaterialQuantity =importQuantity - inputMaterialDetail.RemainQuantity;
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
        public static bool AddInputMaterial(InputMaterial inputMaterial)
        {
            if (inputMaterial == null)
            {
                return false;
            }
            try
            {
                db.InputMaterials.Add(inputMaterial);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return false;
            }
            finally
            {
                db.Dispose();
            }
            return true;
        }
        #endregion
    }
}