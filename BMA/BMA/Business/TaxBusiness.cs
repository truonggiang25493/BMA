using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

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

        
        //public List<TaxRate> GetCurrentTaxRate()
        //{
        //    List<TaxRate> result = new List<TaxRate>();
        //    // Get current VAT
        //    TaxRate vatTaxRate = db.TaxRates.FirstOrDefault(m => m.TaxType.Abbreviation.Equals("GTGT") && DateTime.Now >= m.BeginDate && DateTime.Now <= m.EndDate);
        //    result.Add(vatTaxRate);

        //    // Get current TNDN
        //    // Temp income

        //    Int64 previousYearIncome = 1000000000;
        //    List<TaxRate> tndnTaxRateList = db.TaxRates.Where(m => m.TaxType.Abbreviation.Equals("TNDN") && m.TaxType.Method.Equals("Khấu trừ") && DateTime.Now >= m.BeginDate && DateTime.Now <= m.EndDate).ToList();
        //    foreach (TaxRate taxRate in tndnTaxRateList)
        //    {
        //        string[] condition = taxRate.TaxType.Condition.Split(',');

        //    }
        //    //result.Add(tndnTaxRate);
        //    return result;
        //}
        #endregion

        #region

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
        #endregion
    }
}