using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.WebPages;
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
                result.CreateDate = new DateTime(year, quarter * 3, 1).AddMonths(1).AddDays(-1);
                result.Value20Date = new DateTime(2013, 1, 1);
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
                        m =>
                            m.ImportDate.Month >= startMonth && m.ImportDate.Month <= endMonth &&
                            m.ImportDate.Year == year)
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

                result.Value25 = result.InputTotalTaxAmount;

                result.Value36 = result.OutputTotalAmount - result.Value25;
                if (result.Value36 >= 0)
                {
                    result.Value40A = result.Value36;
                    result.Value40 = result.Value40A;
                }
                else
                {
                    result.Value41 = -result.Value36;
                    result.Value42 = result.Value41;
                }

                result.Value40 = result.Value40A;
                if (result.InputTotalAmount == 0 && result.OutputCategories1TotalAmount == 0 &&
                    result.HaveTaxOutputAmount == 0)
                {
                    result.Value21 = true;
                }
                else
                {
                    result.Value21 = false;
                }
            }
            else
            {
                result.Quarter = quarter;
                result.Year = year;
                if (vatTaxDeclaration.CreateDate == null)
                {
                    result.CreateDate = new DateTime(year, quarter * 3, 1).AddMonths(1).AddDays(-1);
                }
                else
                {
                    result.CreateDate = vatTaxDeclaration.CreateDate;
                }

                if (vatTaxDeclaration.Value20Date == null)
                {
                    result.Value20Date = new DateTime(2013, 1, 1);
                }
                else
                {
                    result.Value20Date = vatTaxDeclaration.Value20Date.Value;
                }

                result.Value20No = vatTaxDeclaration.Value20No;

                // Get StoreInfo
                StoreInfo storeInfo = db.StoreInfoes.FirstOrDefault();

                if (vatTaxDeclaration.StoreOwnerName.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreOwnerName = storeInfo.OwnerName;
                    }
                }
                else
                {
                    result.StoreOwnerName = vatTaxDeclaration.StoreOwnerName;
                }

                if (vatTaxDeclaration.StoreTaxCode.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreTaxCode = storeInfo.TaxCode;
                    }
                }
                else
                {
                    result.StoreTaxCode = vatTaxDeclaration.StoreTaxCode;
                }

                if (vatTaxDeclaration.StoreAddress.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreAddress = storeInfo.Address;
                    }
                }
                else
                {
                    result.StoreAddress = vatTaxDeclaration.StoreAddress;
                }

                if (vatTaxDeclaration.StoreDistrict.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreDistrict = storeInfo.District;
                    }
                }
                else
                {
                    result.StoreDistrict = vatTaxDeclaration.StoreDistrict;
                }

                if (vatTaxDeclaration.StoreProvince.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreProvince = storeInfo.Province;
                    }
                }
                else
                {
                    result.StoreProvince = vatTaxDeclaration.StoreProvince;
                }

                if (vatTaxDeclaration.StorePhone.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StorePhone = storeInfo.Phonenumber;
                    }
                }
                else
                {
                    result.StorePhone = vatTaxDeclaration.StorePhone;
                }

                if (vatTaxDeclaration.StoreFax.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreFax = storeInfo.Fax;
                    }
                }
                else
                {
                    result.StoreFax = vatTaxDeclaration.StoreFax;
                }

                if (vatTaxDeclaration.StoreEmail.Trim() == "")
                {
                    if (storeInfo != null)
                    {
                        result.StoreEmail = storeInfo.Email;
                    }
                }
                else
                {
                    result.StoreEmail = vatTaxDeclaration.StoreEmail;
                }

                // Tax agent info
                result.VatAgentOwnerName = vatTaxDeclaration.TaxAgentOwnerName;
                result.VatAgentTaxCode = vatTaxDeclaration.TaxAgentTaxCode;
                result.VatAgentName = vatTaxDeclaration.TaxAgentName;
                result.VatAgentNo = vatTaxDeclaration.TaxAgentNo;
                result.VatAgentAddress = vatTaxDeclaration.TaxAgentAddress;
                result.VatAgentDistrict = vatTaxDeclaration.TaxAgentDistrict;
                result.VatAgentProvince = vatTaxDeclaration.TaxAgentProvince;
                result.VatAgentPhone = vatTaxDeclaration.TaxAgentPhone;
                result.VatAgentFax = vatTaxDeclaration.TaxAgentFax;
                result.VatAgentEmail = vatTaxDeclaration.TaxAgentEmail;

                result.Value21 = (vatTaxDeclaration.Value21 == 1);
                result.Value22 = vatTaxDeclaration.Value22.Value;
                result.Value25 = vatTaxDeclaration.Value25.Value;
                result.Value36 = vatTaxDeclaration.Value36.Value;
                result.Value37 = vatTaxDeclaration.Value37.Value;
                result.Value38 = vatTaxDeclaration.Value38.Value;
                result.Value39 = vatTaxDeclaration.Value39.Value;
                result.Value40 = vatTaxDeclaration.Value40.Value;
                result.Value40A = vatTaxDeclaration.Value40a.Value;
                result.Value40B = vatTaxDeclaration.Value40b.Value;
                result.Value41 = vatTaxDeclaration.Value41.Value;
                result.Value42 = vatTaxDeclaration.Value42.Value;
                result.Value43 = vatTaxDeclaration.Value43.Value;

                #region Input

                if (!vatTaxDeclaration.InputCategory1.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> inputCategory1List = new List<DeclarationVatCategory>();
                    string[] inputCategory1ListString = vatTaxDeclaration.InputCategory1.Split(';');
                    foreach (string s in inputCategory1ListString)
                    {
                        string[] inputCategory1String = s.Split(',');
                        if (inputCategory1String.Length == 7)
                        {
                            DeclarationVatCategory inputCategory1 = new DeclarationVatCategory();
                            inputCategory1.Column2 = inputCategory1String[0];
                            inputCategory1.Column3 = DateTime.ParseExact(inputCategory1String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            inputCategory1.Column4 = inputCategory1String[2];
                            inputCategory1.Column5 = inputCategory1String[3];
                            inputCategory1.Column6 = Convert.ToInt32(inputCategory1String[4]);
                            inputCategory1.Column7 = Convert.ToInt32(inputCategory1String[5]);
                            inputCategory1.Column8 = inputCategory1String[6];

                            inputCategory1List.Add(inputCategory1);
                        }
                    }
                    result.InputCategories1 = inputCategory1List;
                }

                if (!vatTaxDeclaration.InputCategory2.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> inputCategory2List = new List<DeclarationVatCategory>();
                    string[] inputCategory2ListString = vatTaxDeclaration.InputCategory2.Split(';');
                    foreach (string s in inputCategory2ListString)
                    {
                        string[] inputCategory2String = s.Split(',');
                        if (inputCategory2String.Length == 7)
                        {
                            DeclarationVatCategory inputCategory2 = new DeclarationVatCategory();
                            inputCategory2.Column2 = inputCategory2String[0];
                            inputCategory2.Column3 = DateTime.ParseExact(inputCategory2String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            inputCategory2.Column4 = inputCategory2String[2];
                            inputCategory2.Column5 = inputCategory2String[3];
                            inputCategory2.Column6 = Convert.ToInt32(inputCategory2String[4]);
                            inputCategory2.Column7 = Convert.ToInt32(inputCategory2String[5]);
                            inputCategory2.Column8 = inputCategory2String[6];

                            inputCategory2List.Add(inputCategory2);
                        }
                    }
                    result.InputCategories2 = inputCategory2List;
                }

                if (!vatTaxDeclaration.InputCategory3.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> inputCategory3List = new List<DeclarationVatCategory>();
                    string[] inputCategory3ListString = vatTaxDeclaration.InputCategory3.Split(';');
                    foreach (string s in inputCategory3ListString)
                    {
                        string[] inputCategory3String = s.Split(',');
                        if (inputCategory3String.Length == 7)
                        {
                            DeclarationVatCategory inputCategory3 = new DeclarationVatCategory();
                            inputCategory3.Column2 = inputCategory3String[0];
                            inputCategory3.Column3 = DateTime.ParseExact(inputCategory3String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            inputCategory3.Column4 = inputCategory3String[2];
                            inputCategory3.Column5 = inputCategory3String[3];
                            inputCategory3.Column6 = Convert.ToInt32(inputCategory3String[4]);
                            inputCategory3.Column7 = Convert.ToInt32(inputCategory3String[5]);
                            inputCategory3.Column8 = inputCategory3String[6];

                            inputCategory3List.Add(inputCategory3);
                        }
                    }
                    result.InputCategories3 = inputCategory3List;
                }

                #endregion

                #region Output

                if (!vatTaxDeclaration.OutputCategory1.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> outputCategory1List = new List<DeclarationVatCategory>();
                    string[] outputCategory1ListString = vatTaxDeclaration.OutputCategory1.Split(';');
                    foreach (string s in outputCategory1ListString)
                    {
                        string[] outputCategory1String = s.Split(',');
                        if (outputCategory1String.Length == 7)
                        {
                            DeclarationVatCategory outputCategory1 = new DeclarationVatCategory();
                            outputCategory1.Column2 = outputCategory1String[0];
                            outputCategory1.Column3 = DateTime.ParseExact(outputCategory1String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            outputCategory1.Column4 = outputCategory1String[2];
                            outputCategory1.Column5 = outputCategory1String[3];
                            outputCategory1.Column6 = Convert.ToInt32(outputCategory1String[4]);
                            outputCategory1.Column7 = Convert.ToInt32(outputCategory1String[5]);
                            outputCategory1.Column8 = outputCategory1String[6];

                            outputCategory1List.Add(outputCategory1);
                        }
                    }
                    result.OutputCategories1 = outputCategory1List;
                }

                if (!vatTaxDeclaration.OutputCategory2.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> outputCategory2List = new List<DeclarationVatCategory>();
                    string[] outputCategory2ListString = vatTaxDeclaration.OutputCategory2.Split(';');
                    foreach (string s in outputCategory2ListString)
                    {
                        string[] outputCategory2String = s.Split(',');
                        if (outputCategory2String.Length == 7)
                        {
                            DeclarationVatCategory outputCategory2 = new DeclarationVatCategory();
                            outputCategory2.Column2 = outputCategory2String[0];
                            outputCategory2.Column3 = DateTime.ParseExact(outputCategory2String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            outputCategory2.Column4 = outputCategory2String[2];
                            outputCategory2.Column5 = outputCategory2String[3];
                            outputCategory2.Column6 = Convert.ToInt32(outputCategory2String[4]);
                            outputCategory2.Column7 = Convert.ToInt32(outputCategory2String[5]);
                            outputCategory2.Column8 = outputCategory2String[6];

                            outputCategory2List.Add(outputCategory2);
                        }
                    }
                    result.OutputCategories2 = outputCategory2List;
                }

                if (!vatTaxDeclaration.OutputCategory3.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> outputCategory3List = new List<DeclarationVatCategory>();
                    string[] outputCategory3ListString = vatTaxDeclaration.OutputCategory3.Split(';');
                    foreach (string s in outputCategory3ListString)
                    {
                        string[] outputCategory3String = s.Split(',');
                        if (outputCategory3String.Length == 7)
                        {
                            DeclarationVatCategory outputCategory3 = new DeclarationVatCategory();
                            outputCategory3.Column2 = outputCategory3String[0];
                            outputCategory3.Column3 = DateTime.ParseExact(outputCategory3String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            outputCategory3.Column4 = outputCategory3String[2];
                            outputCategory3.Column5 = outputCategory3String[3];
                            outputCategory3.Column6 = Convert.ToInt32(outputCategory3String[4]);
                            outputCategory3.Column7 = Convert.ToInt32(outputCategory3String[5]);
                            outputCategory3.Column8 = outputCategory3String[6];

                            outputCategory3List.Add(outputCategory3);
                        }
                    }
                    result.OutputCategories3 = outputCategory3List;
                }

                if (!vatTaxDeclaration.OutputCategory4.Trim().IsEmpty())
                {
                    List<DeclarationVatCategory> outputCategory4List = new List<DeclarationVatCategory>();
                    string[] outputCategory4ListString = vatTaxDeclaration.OutputCategory4.Split(';');
                    foreach (string s in outputCategory4ListString)
                    {
                        string[] outputCategory4String = s.Split(',');
                        if (outputCategory4String.Length == 7)
                        {
                            DeclarationVatCategory outputCategory4 = new DeclarationVatCategory();
                            outputCategory4.Column2 = outputCategory4String[0];
                            outputCategory4.Column3 = DateTime.ParseExact(outputCategory4String[1], "yyyy-MM-dd", CultureInfo.InvariantCulture);
                            outputCategory4.Column4 = outputCategory4String[2];
                            outputCategory4.Column5 = outputCategory4String[3];
                            outputCategory4.Column6 = Convert.ToInt32(outputCategory4String[4]);
                            outputCategory4.Column7 = Convert.ToInt32(outputCategory4String[5]);
                            outputCategory4.Column8 = outputCategory4String[6];

                            outputCategory4List.Add(outputCategory4);
                        }
                    }
                    result.OutputCategories4 = outputCategory4List;
                }

                #endregion

                result.InputCategories1TotalAmount = vatTaxDeclaration.InputCategory1TotalAmount ?? 0;
                result.InputCategories2TotalAmount = vatTaxDeclaration.InputCategory2TotalAmount ?? 0;
                result.InputCategories3TotalAmount = vatTaxDeclaration.InputCategory3TotalAmount ?? 0;
                result.InputCategories1TotalTaxAmount = vatTaxDeclaration.InputCategory1TotalTaxAmount ?? 0;
                result.InputCategories2TotalTaxAmount = vatTaxDeclaration.InputCategory2TotalTaxAmount ?? 0;
                result.InputCategories3TotalTaxAmount = vatTaxDeclaration.InputCategory3TotalTaxAmount ?? 0;
                result.InputTotalAmount = vatTaxDeclaration.InputTotalAmountValue23;
                result.InputTotalTaxAmount = vatTaxDeclaration.InputTotalTaxAmountValue24;

                result.OutputCategories1TotalAmount = vatTaxDeclaration.OutputCategory1TotalAmountValue26 ?? 0;
                result.OutputCategories2TotalAmount = vatTaxDeclaration.OutputCategory2TotalAmountValue29 ?? 0;
                result.OutputCategories3TotalAmount = vatTaxDeclaration.OutputCategory3TotalAmountValue30 ?? 0;
                result.OutputCategories4TotalAmount = vatTaxDeclaration.OutputCategory4TotalAmountValue32 ?? 0;

                result.OutputCategories3TotalTaxAmount = vatTaxDeclaration.OutputCategory3TotalTaxAmountValue31 ?? 0;
                result.OutputCategories4TotalTaxAmount = vatTaxDeclaration.OutputCategory4TotalTaxAmountValue33 ?? 0;
                result.OutputTotalAmount = vatTaxDeclaration.OutputTotalAmountValue34;
                result.HaveTaxOutputAmount = vatTaxDeclaration.HaveTaxOutputTotalAmountValue27;
                result.OutputTotalTaxAmount = vatTaxDeclaration.OutputTotalTaxAmountValue28;

                if (result.InputTotalAmount == 0 && result.OutputCategories1TotalAmount == 0 &&
                    result.HaveTaxOutputAmount == 0)
                {
                    result.Value21 = true;
                }
                else
                {
                    result.Value21 = false;
                }
            }
            return result;
        }

        public bool SaveVatTaxDeclaration(VatTaxDeclaration declaration)
        {
            int year = declaration.Year;
            int quarter = declaration.Quarter;

            VatTaxDeclaration check = db.VatTaxDeclarations.FirstOrDefault(m => m.Year == year && m.Quarter == quarter);

            if (check != null)
            {
                db.VatTaxDeclarations.Remove(check);
            }
            db.VatTaxDeclarations.Add(declaration);

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