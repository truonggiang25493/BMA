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
                            Column3 = inputBill.ImportDate.ToString("dd/MM/yyyy"),
                            Column4 = inputBill.Supplier.SupplierName,
                            Column5 = inputBill.Supplier.SupplierTaxCode,
                            Column6 = inputBill.InputBillAmount.ToString(),
                            Column7 = inputBill.InputTaxAmount.ToString(),
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
                        category4.Column3 = outputBill.ExportDate.ToString("dd/MM/yyyy");
                        category4.Column4 = outputBill.Order.User1.Fullname;
                        category4.Column5 = outputBill.Order.User1.Customers.ElementAt(0).TaxCode;
                        category4.Column6 = outputBill.OutputBillAmount.ToString();
                        category4.Column7 = outputBill.OutputBillTaxAmount.ToString();



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
            }
            return result;
        }
        #endregion

        #region TNDN

        public TndnTaxDeclarationViewModel GeTndnTaxDeclarationViewModel(int year)
        {
            TndnTaxDeclarationViewModel result = new TndnTaxDeclarationViewModel();

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

            // Bug Hard code
            TaxRate taxRate = db.TaxRates.FirstOrDefault(m => m.TaxRateId == 2);
            if (taxRate != null)
            {
                result.TndnTaxRate = taxRate.TaxRateValue;
            }

            ReportBusiness business = new ReportBusiness();
            List<sp_GetIncomeYearly_Result> incomeYearly = business.GetIncomeYearlyStore(year, year);

            var income = incomeYearly.ElementAt(0).Income;
            if (income != null)
            {
                result.ValueA1 = income.Value;
            }
            else
            {
                result.ValueA1 = 0;
            }

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
            result.ValueB13 = result.ValueB12 - result.ValueB13;


            result.ValueC1 = result.ValueB13;
            result.ValueC2 = 0;
            result.ValueC3 = 0;
            result.ValueC3a = 0;
            result.ValueC3b = 0;
            result.ValueC4 = result.ValueC1;
            result.ValueC5 = 0;
            result.ValueC6 = result.ValueC4 - result.ValueC5;
            result.ValueC7 = 0;
            result.ValueC8 = result.ValueC4;
            result.ValueC9 = 0;
            result.ValueC9a = 0;
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

            return result;
        }

        #endregion
    }
}