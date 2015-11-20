using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Business
{
    public class TaxBusiness
    {
        private BMAEntities db;

        public TaxBusiness()
        {
            db = new BMAEntities();
        }

        #region Get current tax rates


        public List<TaxRate> GetCurrentTaxRate()
        {
            List<TaxRate> result = new List<TaxRate>();
            // Get current VAT
            TaxRate vatTaxRate = db.TaxRates.FirstOrDefault(m => m.TaxType.Abbreviation.Equals("GTGT") && DateTime.Now >= m.BeginDate && DateTime.Now <= m.EndDate);
            result.Add(vatTaxRate);

            // Get current TNDN
            // Temp income

            Int64 previousYearIncome = 1000000000;
            List<TaxRate> tndnTaxRateList = db.TaxRates.Where(m => m.TaxType.Abbreviation.Equals("TNDN") && m.TaxType.Method.Equals("Khấu trừ") && DateTime.Now >= m.BeginDate && DateTime.Now <= m.EndDate).ToList();
            foreach (TaxRate taxRate in tndnTaxRateList)
            {
                string[] condition = taxRate.TaxType.Condition.Split(',');

            }
            //result.Add(tndnTaxRate);
            return result;
        }
        #endregion

        #region GTGT

        public bool ChangeVat(int vatRate, DateTime beginDate)
        {
            TaxRate currentVatTaxRate =
                db.TaxRates.FirstOrDefault(
                    m => m.TaxTypeId == 1 && DateTime.Now >= m.BeginDate && DateTime.Now <= m.EndDate);
            if (currentVatTaxRate == null)
            {
                return false;
            }

            currentVatTaxRate.EndDate = beginDate.AddDays(-1);
            TaxRate taxRate = new TaxRate();
            taxRate.TaxTypeId = 1;
            taxRate.TaxRateValue = vatRate;
            taxRate.BeginDate = beginDate;
            taxRate.EndDate = new DateTime(9999, 12, 31);

            db.TaxRates.Add(taxRate);

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

        public DeclarationVatForm GetListInputBill(int month, int year)
        {
            DeclarationVatForm result = new DeclarationVatForm();

            StoreInfo storeInfo = db.StoreInfoes.FirstOrDefault();

            result.StoreOwnerName = storeInfo.OwnerName;
            result.StoreTaxCode = storeInfo.TaxCode;
            result.Month = month;
            result.Year = year;

            List<InputBill> inputBillList =
                db.InputBills.Where(m => m.ImportDate.Month == month && m.ImportDate.Year == year).ToList();
            List<DeclarationVatCategory> categories1 = new List<DeclarationVatCategory>();
            int totalCategory1 = 0;
            int totalTaxCategory1 = 0;

            foreach (InputBill inputBill in inputBillList)
            {
                DeclarationVatCategory category = new DeclarationVatCategory();

                category.Column4 = inputBill.InputBillCode;
                category.Column5 = inputBill.ImportDate.ToString("dd/MM/yyyy");
                category.Column6 = inputBill.Supplier.SupplierName;
                category.Column7 = inputBill.Supplier.SupplierTaxCode;
                category.Column9 = inputBill.InputBillAmount.ToString();
                category.Column11 = inputBill.InputTaxAmount.ToString();
                category.Column10 = (inputBill.InputTaxAmount / inputBill.InputBillAmount * 100).ToString();

                totalCategory1 += (int)inputBill.InputBillAmount;
                totalTaxCategory1 += (int)inputBill.InputTaxAmount;

                categories1.Add(category);

            }

            result.Categories1 = categories1;
            result.Categories1TotalAmount = totalCategory1.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            result.Categories1TotalTaxAmount = totalTaxCategory1.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            result.TotalAmount = totalCategory1.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            result.TotalTaxAmount = totalTaxCategory1.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            return result;
        }
        #endregion
    }
}