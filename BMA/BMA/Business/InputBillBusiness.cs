using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using System.Net;
using System.Data;
using System.Globalization;
using BMA.Business;
using System.IO;

namespace BMA.Business
{
    public class InputBillBusiness
    {
        private static BMAEntities db = new BMAEntities();

        #region Get Input Bill List
        public static List<InputBill> GetInputBillList()
        {
            db = new BMAEntities();
            List<InputBill> inputBillList = db.InputBills.OrderByDescending(n => n.InputBillId).ToList();
            return inputBillList;
        }
        #endregion

        #region View input bill detail
        public InputBill GetInputBill(int id)
        {
            InputBill inputBillDetail = db.InputBills.SingleOrDefault(n => n.InputBillId == id);
            return inputBillDetail;
        }
        #endregion

        #region Add new input bill
        public static bool AddInputBill(InputBill inputBill)
        {
            if (inputBill == null)
            {
                return false;
            }
            try
            {
                db.InputBills.Add(inputBill);
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

        #region Edit input bill
        public static bool EditInputBill(int inputBillId, int supplierId, String inputBillCode, int inputBillAmount, int inputTaxAmount, String inputRawImage, String importDate, string formNo, string serial)
        {
            var inputBill = db.InputBills.SingleOrDefault(n => n.InputBillId == inputBillId);
            if (inputBill != null)
            {
                try
                {
                    inputBill.InputBillId = inputBillId;
                    inputBill.SupplierId = supplierId;
                    inputBill.InputBillAmount = inputBillAmount;
                    inputBill.InputTaxAmount = inputTaxAmount;
                    inputBill.ImportDate = DateTime.ParseExact(importDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    inputBill.InputBillCode = inputBillCode.Replace("-", "");
                    inputBill.InputRawImage = inputRawImage;
                    


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

    }
}