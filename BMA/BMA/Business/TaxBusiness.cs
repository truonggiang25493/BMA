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

            List<TaxRate> tndnTaxRateList = db.TaxRates.Where(m => DateTime.Now >= m.BeginDate && DateTime.Now <= m.EndDate).ToList();
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



        public DeclarationVatForm GetVatTaxDeclaration(int quarter, int year)
        {
            DeclarationVatForm result = new DeclarationVatForm();

            VatTaxDeclaration vatTaxDeclaration =
                db.VatTaxDeclarations.FirstOrDefault(m => m.Quarter == quarter && m.Year == year);
            if (vatTaxDeclaration == null)
            {

                result.Quarter = quarter;
                result.Year = year;
                result.CreateMonth = quarter * 3;
                // Get StoreInfo
                StoreInfo storeInfo = db.StoreInfoes.FirstOrDefault();
                if (storeInfo != null)
                {
                    result.StoreOwnerName = storeInfo.OwnerName;
                    result.StoreTaxCode = storeInfo.TaxCode;
                    result.StoreAddress = storeInfo.Address;
                    result.StoreDistrict = storeInfo.District;
                    result.StoreProvince = storeInfo.Province;
                    result.StorePhone = storeInfo.Phonenumber;
                    result.StoreFax = storeInfo.Fax;
                    result.StoreEmail = storeInfo.Email;
                }

                int startMonth = quarter * 3 - 2;
                int endMonth = quarter * 3;

                // Get Input Bill
                List<InputBill> inputBillList =
                    db.InputBills.Where(
                        m => m.ImportDate.Month >= startMonth && m.ImportDate.Month <= endMonth && m.ImportDate.Year == year)
                        .ToList();
                List<DeclarationVatCategory> inputCategories1 = new List<DeclarationVatCategory>();
                int totalInputCategory1 = 0;
                int totalTaxInputCategory1 = 0;
                if (inputBillList.Count > 0)
                {
                    foreach (InputBill inputBill in inputBillList)
                    {
                        DeclarationVatCategory category1 = new DeclarationVatCategory
                        {
                            Column2 = inputBill.InputBillCode,
                            Column3 = inputBill.ImportDate,
                            Column4 = inputBill.Supplier.SupplierName,
                            Column5 = inputBill.Supplier.SupplierTaxCode,
                            Column6 = inputBill.InputBillAmount,
                            Column7 = inputBill.InputTaxAmount
                        };


                        totalInputCategory1 += (int)inputBill.InputBillAmount;
                        totalTaxInputCategory1 += (int)inputBill.InputTaxAmount;

                        inputCategories1.Add(category1);
                    }
                }

                result.InputCategories1 = inputCategories1;
                result.InputCategories1TotalAmount = totalInputCategory1;
                result.InputCategories1TotalTaxAmount = totalTaxInputCategory1;

                result.InputTotalAmount = totalInputCategory1;
                result.InputTotalTaxAmount = totalTaxInputCategory1;

                // Get Output bill
                List<OutputBill> outputBillList =
                    db.OutputBills.Where(
                        m =>
                            m.ExportDate.Month >= startMonth && m.ExportDate.Month <= endMonth &&
                            m.ExportDate.Year == year).ToList();
                List<DeclarationVatCategory> outputCategories4 = new List<DeclarationVatCategory>();
                int totalOutputCategory4 = 0;
                int totalTaxOutputCategory4 = 0;

                if (outputBillList.Count > 0)
                {
                    foreach (OutputBill outputBill in outputBillList)
                    {
                        DeclarationVatCategory category4 = new DeclarationVatCategory();

                        category4.Column2 = outputBill.OutputBillCode;
                        category4.Column3 = outputBill.ExportDate;
                        category4.Column4 = outputBill.Order.User.Fullname;
                        category4.Column5 = outputBill.Order.User.Customers.ElementAt(0).TaxCode;
                        category4.Column6 = outputBill.OutputBillAmount;
                        category4.Column7 = outputBill.OutputBillTaxAmount;



                        totalOutputCategory4 += (int)outputBill.OutputBillAmount;
                        totalTaxOutputCategory4 += (int)outputBill.OutputBillTaxAmount;

                        outputCategories4.Add(category4);
                    }
                }

                result.OutputCategories4 = outputCategories4;
                result.OutputCategories4TotalAmount = totalOutputCategory4;
                result.OutputCategories4TotalTaxAmount = totalTaxOutputCategory4;

                result.OutputTotalAmount = totalOutputCategory4;
                result.HaveTaxOutputAmount = totalOutputCategory4;
                result.OutputTotalTaxAmount = totalTaxOutputCategory4;

                result.Value36 = result.OutputTotalAmount;
                result.Value40A = result.Value36;
                result.Value40 = result.Value40A;
            }
            return result;
        }
        #endregion

        #region TNDN



        public TndnTaxDeclaration GeTndnTaxDeclaration(int year)
        {
            TndnTaxDeclaration result = db.TndnTaxDeclarations.FirstOrDefault(m => m.Year == year);
            {
                if (result == null)
                {
                    result = new TndnTaxDeclaration();
                    StoreInfo storeInfo = db.StoreInfoes.FirstOrDefault();
                    if (storeInfo != null)
                    {
                        result.StoreOwnerName = storeInfo.OwnerName;
                        result.StoreTaxCode = storeInfo.TaxCode;
                        result.StoreAddress = storeInfo.Address;
                        result.StoreDistrict = storeInfo.District;
                        result.StoreProvince = storeInfo.Province;
                        result.StorePhone = storeInfo.Phonenumber;
                        result.StoreFax = storeInfo.Fax;
                        result.StoreEmail = storeInfo.Email;

                        result.SignName = storeInfo.OwnerName;
                        result.Year = year;
                    }



                    ReportBusiness business = new ReportBusiness();
                    List<sp_GetIncomeYearly_Result> incomeYearly = business.GetIncomeYearlyStore(year, year);

                    var revenue = incomeYearly.ElementAt(0).Revenue;
                    result.Value1 = revenue ?? 0;
                    result.Value2 = 0;
                    result.Value4 = 0;
                    result.Value5 = 0;
                    result.Value6 = 0;
                    result.Value7 = 0;
                    result.Value3 = result.Value4 + result.Value5 + result.Value6 + result.Value7;
                    result.Value8 = 0;

                    var materialExpense = incomeYearly.ElementAt(0).MaterialExpense;
                    result.Value10 = materialExpense ?? 0;

                    List<OtherExpense> value11List =
                        db.OtherExpenses.Where(m => m.Type == 11 && m.OtherExpenseYearTime == year).ToList();
                    result.Value11 = 0;
                    foreach (OtherExpense otherExpense in value11List)
                    {
                        result.Value11 += otherExpense.OtherExpenseAmount;
                    }

                    List<OtherExpense> value12List = db.OtherExpenses.Where(m => m.Type == 12 && m.OtherExpenseYearTime == year).ToList();
                    result.Value12 = 0;
                    foreach (OtherExpense otherExpense in value12List)
                    {
                        result.Value12 += otherExpense.OtherExpenseAmount;
                    }

                    result.Value9 = result.Value10 + result.Value11 + result.Value12;

                    result.Value13 = 0;
                    result.Value14 = 0;
                    result.Value15 = result.Value1 - result.Value3 + result.Value8 - result.Value9 - result.Value13;
                    result.Value16 = 0;
                    result.Value17 = 0;
                    result.Value18 = result.Value16 - result.Value17;
                    result.Value19 = result.Value15 + result.Value18;

                    result.ValueA1 = result.Value19;
                    result.ValueB1 = 0;
                    result.ValueB2 = 0;
                    result.ValueB3 = 0;
                    result.ValueB4 = 0;
                    result.ValueB5 = 0;
                    result.ValueB6 = 0;
                    result.ValueB7 = 0;
                    result.ValueB8 = 0;
                    result.ValueB9 = 0;
                    result.ValueB10 = 0;
                    result.ValueB11 = 0;
                    result.ValueB12 = result.ValueA1;
                    result.ValueB14 = 0;
                    result.ValueB13 = result.ValueB12 - result.ValueB14;


                    result.ValueC1 = result.ValueB13;
                    result.ValueC2 = 0;
                    result.ValueC3 = 0;
                    result.ValueC3a = 0;
                    result.ValueC3b = 0;
                    result.ValueC4 = result.ValueC1;
                    result.ValueC5 = 0;
                    result.ValueC6 = result.ValueC4 - result.ValueC5;

                    // Bug Hard code
                    int tndnTaxRate = 0;
                    TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.TaxRateId == 2);
                    if (taxRate != null)
                    {
                        tndnTaxRate = taxRate.TaxRateValue;
                    }

                    if (tndnTaxRate == 22)
                    {
                        result.ValueC7 = result.ValueC4;
                    }
                    else
                    {
                        result.ValueC7 = 0;
                    }

                    if (tndnTaxRate == 20)
                    {
                        result.ValueC8 = result.ValueC4;
                    }
                    else
                    {
                        result.ValueC8 = 0;
                    }

                    if (tndnTaxRate != 22 && tndnTaxRate != 20)
                    {
                        result.ValueC9 = result.ValueC4;
                        result.ValueC9a = tndnTaxRate;
                    }
                    else
                    {
                        result.ValueC9 = 0;
                        result.ValueC9a = 0;
                    }

                    result.ValueC10 = (int)(result.ValueC7 / 5 + result.ValueC8 * 0.22 + result.ValueC9 * result.ValueC9a / 100);
                    result.ValueC11 = 0;
                    result.ValueC12 = 0;
                    result.ValueC13 = 0;
                    result.ValueC14 = 0;
                    result.ValueC15 = 0;
                    result.ValueC16 = result.ValueC10 - result.ValueC11 - result.ValueC12 - result.ValueC15;

                    result.ValueD1 = result.ValueC16;
                    result.ValueD2 = 0;
                    result.ValueD3 = 0;
                    result.ValueD = result.ValueD1 + result.ValueD2 + result.ValueD3;

                    result.ValueE = 0;
                    result.ValueE2 = 0;
                    result.ValueE3 = 0;
                    result.ValueE1 = 0;

                    result.ValueG1 = result.ValueD1 - result.ValueE1;
                    result.ValueG2 = result.ValueD2 - result.ValueE2;
                    result.ValueG3 = result.ValueD3 - result.ValueE3;
                    result.ValueG = result.ValueG1 + result.ValueG2 + result.ValueG3;

                    result.ValueH = result.ValueD / 5;

                    result.ValueI = result.ValueG - result.ValueH;
                    result.ValueL1 = false;
                }
            }




            return result;
        }

        public bool SaveTndnTaxDeclaration(TndnTaxDeclaration declaration)
        {
            int year = declaration.Year;

            TndnTaxDeclaration check = db.TndnTaxDeclarations.FirstOrDefault(m => m.Year == year);

            if (check != null)
            {
                db.TndnTaxDeclarations.Remove(check);
            }
            db.TndnTaxDeclarations.Add(declaration);
            db.SaveChanges();
            
            return true;
        }

        #endregion
    }
}