using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class TaxController : Controller
    {
        // GET: Tax
        public ActionResult Index()
        {
            ViewBag.Title = "Báo cáo thuế";
            ViewBag.TreeView = "report";
            ViewBag.TreeViewMenu = "taxRepory";
            TaxBusiness taxBusiness = new TaxBusiness();
            List<TaxRate> taxRateList = taxBusiness.GetCurrentTaxRate();
            return View(taxRateList);
        }

        // Change VAT
        [HttpPost]
        public int ChangeVat(FormCollection form)
        {
            string vatRateString = form["vatRate"];
            string beginDateString = form["beginDate"];
            if (!(vatRateString.IsEmpty() || beginDateString.IsEmpty()))
            {
                int vatRate;

                if (!(int.TryParse(vatRateString, out vatRate)))
                {
                    return 0;
                }
                DateTime beginDate;
                DateTime.TryParse(beginDateString, out beginDate);
                if (!(DateTime.TryParse(beginDateString, out beginDate)))
                {
                    return 0;
                }
                TaxBusiness taxBusiness = new TaxBusiness();
                bool rs = taxBusiness.ChangeVat(vatRate, beginDate);
                return rs ? 1 : 0;
            }
            return 0;
        }

        #region VAT Tax report

        public ActionResult OutputVatList()
        {
            return View();
        }
        public ActionResult InputVatList(string inputVatTime)
        {
            DateTime date = DateTime.ParseExact(inputVatTime, "MM/yyyy", CultureInfo.InvariantCulture);
            TaxBusiness business = new TaxBusiness();
            DeclarationVatForm result = business.GetListInputBill(date.Month, date.Year);

            return View(result);
        }
        public ActionResult TaxDeclaration()
        {
            return View();
        }

        public int SaveTaxDeclaration(FormCollection form)
        {

            string taxDeclarationType = form["taxDeclarationType"];
            string taxMonth = form["taxMonth"];
            string taxYear = form["taxYear"];
            string storeOwnerTaxCode = form["storeOwnerTaxCode"];
            string storeOwnerName = form["storeOwnerName"];
            string agentOwnerName = form["agentOwnerName"];
            string agentTaxCode = form["agentTaxCode"];

            string category1Column2 = form["category1_column2"];
            string category1Column3 = form["category1_column3"];
            string category1Column4 = form["category1_column4"];
            string category1Column5 = form["category1_column5"];
            string category1Column6 = form["category1_column6"];
            string category1Column7 = form["category1_column7"];
            string category1Column8 = form["category1_column8"];
            string category1Column9 = form["category1_column9"];
            string category1Column10 = form["category1_column10"];
            string category1Column11 = form["category1_column11"];
            string category1Column12 = form["category1_column12"];

            string category2Column2 = form["category2_column2"];
            string category2Column3 = form["category2_column3"];
            string category2Column4 = form["category2_column4"];
            string category2Column5 = form["category2_column5"];
            string category2Column6 = form["category2_column6"];
            string category2Column7 = form["category2_column7"];
            string category2Column8 = form["category2_column8"];
            string category2Column9 = form["category2_column9"];
            string category2Column10 = form["category2_column10"];
            string category2Column11 = form["category2_column11"];
            string category2Column12 = form["category2_column12"];

            string category3Column2 = form["category3_column2"];
            string category3Column3 = form["category3_column3"];
            string category3Column4 = form["category3_column4"];
            string category3Column5 = form["category3_column5"];
            string category3Column6 = form["category3_column6"];
            string category3Column7 = form["category3_column7"];
            string category3Column8 = form["category3_column8"];
            string category3Column9 = form["category3_column9"];
            string category3Column10 = form["category3_column10"];
            string category3Column11 = form["category3_column11"];
            string category3Column12 = form["category3_column12"];

            string category4Column2 = form["category4_column2"];
            string category4Column3 = form["category4_column3"];
            string category4Column4 = form["category4_column4"];
            string category4Column5 = form["category4_column5"];
            string category4Column6 = form["category4_column6"];
            string category4Column7 = form["category4_column7"];
            string category4Column8 = form["category4_column8"];
            string category4Column9 = form["category4_column9"];
            string category4Column10 = form["category4_column10"];
            string category4Column11 = form["category4_column11"];
            string category4Column12 = form["category4_column12"];

            string category5Column2 = form["category5_column2"];
            string category5Column3 = form["category5_column3"];
            string category5Column4 = form["category5_column4"];
            string category5Column5 = form["category5_column5"];
            string category5Column6 = form["category5_column6"];
            string category5Column7 = form["category5_column7"];
            string category5Column8 = form["category5_column8"];
            string category5Column9 = form["category5_column9"];
            string category5Column10 = form["category5_column10"];
            string category5Column11 = form["category5_column11"];
            string category5Column12 = form["category5_column12"];

            string signName = form["signName"];

            string createLocation = form["createLocation"];
            string createDay = form["createDay"];


            string agentName = form["agentName"];
            string agentNo = form["agentNo"];

            return 1;
        }

        public ActionResult ExportTaxDeclaration(FormCollection form)
        {

            string taxDeclarationType = form["taxDeclarationType"];
            string taxMonth = form["taxMonth"];
            string taxYear = form["taxYear"];
            string storeOwnerTaxCode = form["storeOwnerTaxCode"];
            string storeOwnerName = form["storeOwnerName"];
            string agentOwnerName = form["agentOwnerName"];
            string agentTaxCode = form["agentTaxCode"];

            string category1Column2 = form["category1_column2"];
            string category1Column3 = form["category1_column3"];
            string category1Column4 = form["category1_column4"];
            string category1Column5 = form["category1_column5"];
            string category1Column6 = form["category1_column6"];
            string category1Column7 = form["category1_column7"];
            string category1Column8 = form["category1_column8"];
            string category1Column9 = form["category1_column9"];
            string category1Column10 = form["category1_column10"];
            string category1Column11 = form["category1_column11"];
            string category1Column12 = form["category1_column12"];

            string category2Column2 = form["category2_column2"];
            string category2Column3 = form["category2_column3"];
            string category2Column4 = form["category2_column4"];
            string category2Column5 = form["category2_column5"];
            string category2Column6 = form["category2_column6"];
            string category2Column7 = form["category2_column7"];
            string category2Column8 = form["category2_column8"];
            string category2Column9 = form["category2_column9"];
            string category2Column10 = form["category2_column10"];
            string category2Column11 = form["category2_column11"];
            string category2Column12 = form["category2_column12"];

            string category3Column2 = form["category3_column2"];
            string category3Column3 = form["category3_column3"];
            string category3Column4 = form["category3_column4"];
            string category3Column5 = form["category3_column5"];
            string category3Column6 = form["category3_column6"];
            string category3Column7 = form["category3_column7"];
            string category3Column8 = form["category3_column8"];
            string category3Column9 = form["category3_column9"];
            string category3Column10 = form["category3_column10"];
            string category3Column11 = form["category3_column11"];
            string category3Column12 = form["category3_column12"];

            string category4Column2 = form["category4_column2"];
            string category4Column3 = form["category4_column3"];
            string category4Column4 = form["category4_column4"];
            string category4Column5 = form["category4_column5"];
            string category4Column6 = form["category4_column6"];
            string category4Column7 = form["category4_column7"];
            string category4Column8 = form["category4_column8"];
            string category4Column9 = form["category4_column9"];
            string category4Column10 = form["category4_column10"];
            string category4Column11 = form["category4_column11"];
            string category4Column12 = form["category4_column12"];

            string category5Column2 = form["category5_column2"];
            string category5Column3 = form["category5_column3"];
            string category5Column4 = form["category5_column4"];
            string category5Column5 = form["category5_column5"];
            string category5Column6 = form["category5_column6"];
            string category5Column7 = form["category5_column7"];
            string category5Column8 = form["category5_column8"];
            string category5Column9 = form["category5_column9"];
            string category5Column10 = form["category5_column10"];
            string category5Column11 = form["category5_column11"];
            string category5Column12 = form["category5_column12"];

            string signName = form["signName"];

            string createLocation = form["createLocation"];
            string createDay = form["createDay"];


            string agentName = form["agentName"];
            string agentNo = form["agentNo"];

            DeclarationVatForm declarationVat = new DeclarationVatForm();

            declarationVat.Type = Convert.ToInt32(taxDeclarationType);
            declarationVat.Month = Convert.ToInt32(taxMonth);
            declarationVat.Year = Convert.ToInt32(taxYear);
            declarationVat.StoreOwnerName = storeOwnerName;
            declarationVat.StoreTaxCode = storeOwnerTaxCode;
            declarationVat.VatAgentOwnerName = agentOwnerName;
            if (agentTaxCode != null)
            {
                declarationVat.VatAgentTaxCode = agentTaxCode.Replace(",", "");
            }

            declarationVat.VatAgentName = agentName;
            declarationVat.VatAgentNo = agentNo;
            declarationVat.SignName = signName;
            declarationVat.CreateLocation = createLocation;
            declarationVat.CreateDay = createDay;

            int totalAmount = 0;
            int totalTaxAmount = 0;


            if (category1Column4.Trim().Length > 0)
            {
                string[] category1Column2Array = category1Column2.Split(',');
                string[] category1Column3Array = category1Column3.Split(',');
                string[] category1Column4Array = category1Column4.Split(',');
                string[] category1Column5Array = category1Column5.Split(',');
                string[] category1Column6Array = category1Column6.Split(',');
                string[] category1Column7Array = category1Column7.Split(',');
                string[] category1Column8Array = category1Column8.Split(',');
                string[] category1Column9Array = category1Column9.Split(',');
                string[] category1Column10Array = category1Column10.Split(',');
                string[] category1Column11Array = category1Column11.Split(',');
                string[] category1Column12Array = category1Column12.Split(',');

                List<DeclarationVatCategory> category1List = new List<DeclarationVatCategory>();
                int totalCategory1 = 0;
                int totalTaxCategory1 = 0;

                for (int i = 0; i < category1Column2Array.Length; i++)
                {
                    DeclarationVatCategory category1 = new DeclarationVatCategory();

                    category1.Column2 = category1Column2Array[i];
                    category1.Column3 = category1Column3Array[i];
                    category1.Column4 = category1Column4Array[i];
                    category1.Column5 = category1Column5Array[i];
                    category1.Column6 = category1Column6Array[i];
                    category1.Column7 = category1Column7Array[i];
                    category1.Column8 = category1Column8Array[i];

                    int categoryColumn9Number = Convert.ToInt32(category1Column9Array[i]);
                    totalCategory1 += categoryColumn9Number;
                    category1.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

                    category1.Column10 = category1Column10Array[i];

                    int categoryColumn11Number = Convert.ToInt32(category1Column11Array[i]);
                    totalTaxCategory1 += categoryColumn11Number;
                    category1.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
                    category1.Column12 = category1Column12Array[i];

                    category1List.Add(category1);
                }

                declarationVat.Categories1 = category1List;
                declarationVat.Categories1TotalAmount = totalCategory1.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));
                declarationVat.Categories1TotalTaxAmount = totalTaxCategory1.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));

                totalAmount += totalCategory1;
                totalTaxAmount += totalTaxCategory1;

            }


            if (category2Column4.Trim().Length > 0)
            {
                string[] category2Column2Array = category2Column2.Split(',');
                string[] category2Column3Array = category2Column3.Split(',');
                string[] category2Column4Array = category2Column4.Split(',');
                string[] category2Column5Array = category2Column5.Split(',');
                string[] category2Column6Array = category2Column6.Split(',');
                string[] category2Column7Array = category2Column7.Split(',');
                string[] category2Column8Array = category2Column8.Split(',');
                string[] category2Column9Array = category2Column9.Split(',');
                string[] category2Column10Array = category2Column10.Split(',');
                string[] category2Column11Array = category2Column11.Split(',');
                string[] category2Column12Array = category2Column12.Split(',');

                List<DeclarationVatCategory> category2List = new List<DeclarationVatCategory>();
                int totalcategory2 = 0;
                int totalTaxcategory2 = 0;

                for (int i = 0; i < category2Column4Array.Length; i++)
                {
                    DeclarationVatCategory category2 = new DeclarationVatCategory();

                    category2.Column2 = category2Column2Array[i];
                    category2.Column3 = category2Column3Array[i];
                    category2.Column4 = category2Column4Array[i];
                    category2.Column5 = category2Column5Array[i];
                    category2.Column6 = category2Column6Array[i];
                    category2.Column7 = category2Column7Array[i];
                    category2.Column8 = category2Column8Array[i];

                    int categoryColumn9Number = Convert.ToInt32(category2Column9Array[i]);
                    totalcategory2 += categoryColumn9Number;
                    category2.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

                    category2.Column10 = category2Column10Array[i];

                    int categoryColumn11Number = Convert.ToInt32(category2Column11Array[i]);
                    totalTaxcategory2 += categoryColumn11Number;
                    category2.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
                    category2.Column12 = category2Column12Array[i];

                    category2List.Add(category2);
                }

                declarationVat.Categories2 = category2List;
                declarationVat.Categories2TotalAmount = totalcategory2.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));
                declarationVat.Categories2TotalTaxAmount = totalTaxcategory2.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));

                totalAmount += totalcategory2;
                totalTaxAmount += totalTaxcategory2;

            }

            if (category3Column4.Trim().Length > 0)
            {
                string[] category3Column2Array = category3Column2.Split(',');
                string[] category3Column3Array = category3Column3.Split(',');
                string[] category3Column4Array = category3Column4.Split(',');
                string[] category3Column5Array = category3Column5.Split(',');
                string[] category3Column6Array = category3Column6.Split(',');
                string[] category3Column7Array = category3Column7.Split(',');
                string[] category3Column8Array = category3Column8.Split(',');
                string[] category3Column9Array = category3Column9.Split(',');
                string[] category3Column10Array = category3Column10.Split(',');
                string[] category3Column11Array = category3Column11.Split(',');
                string[] category3Column12Array = category3Column12.Split(',');

                List<DeclarationVatCategory> category3List = new List<DeclarationVatCategory>();
                int totalcategory3 = 0;
                int totalTaxcategory3 = 0;

                for (int i = 0; i < category3Column4Array.Length; i++)
                {
                    DeclarationVatCategory category3 = new DeclarationVatCategory();

                    category3.Column2 = category3Column2Array[i];
                    category3.Column3 = category3Column3Array[i];
                    category3.Column4 = category3Column4Array[i];
                    category3.Column5 = category3Column5Array[i];
                    category3.Column6 = category3Column6Array[i];
                    category3.Column7 = category3Column7Array[i];
                    category3.Column8 = category3Column8Array[i];

                    int categoryColumn9Number = Convert.ToInt32(category3Column9Array[i]);
                    totalcategory3 += categoryColumn9Number;
                    category3.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

                    category3.Column10 = category3Column10Array[i];

                    int categoryColumn11Number = Convert.ToInt32(category3Column11Array[i]);
                    totalTaxcategory3 += categoryColumn11Number;
                    category3.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
                    category3.Column12 = category3Column12Array[i];

                    category3List.Add(category3);
                }

                declarationVat.Categories3 = category3List;
                declarationVat.Categories3TotalAmount = totalcategory3.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));
                declarationVat.Categories3TotalTaxAmount = totalTaxcategory3.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));

                totalAmount += totalcategory3;
                totalTaxAmount += totalTaxcategory3;

            }

            if (category4Column4.Trim().Length > 0)
            {
                string[] category4Column2Array = category4Column2.Split(',');
                string[] category4Column3Array = category4Column3.Split(',');
                string[] category4Column4Array = category4Column4.Split(',');
                string[] category4Column5Array = category4Column5.Split(',');
                string[] category4Column6Array = category4Column6.Split(',');
                string[] category4Column7Array = category4Column7.Split(',');
                string[] category4Column8Array = category4Column8.Split(',');
                string[] category4Column9Array = category4Column9.Split(',');
                string[] category4Column10Array = category4Column10.Split(',');
                string[] category4Column11Array = category4Column11.Split(',');
                string[] category4Column12Array = category4Column12.Split(',');

                List<DeclarationVatCategory> category4List = new List<DeclarationVatCategory>();
                int totalcategory4 = 0;
                int totalTaxcategory4 = 0;

                for (int i = 0; i < category4Column4Array.Length; i++)
                {
                    DeclarationVatCategory category4 = new DeclarationVatCategory();

                    category4.Column2 = category4Column2Array[i];
                    category4.Column3 = category4Column3Array[i];
                    category4.Column4 = category4Column4Array[i];
                    category4.Column5 = category4Column5Array[i];
                    category4.Column6 = category4Column6Array[i];
                    category4.Column7 = category4Column7Array[i];
                    category4.Column8 = category4Column8Array[i];

                    int categoryColumn9Number = Convert.ToInt32(category4Column9Array[i]);
                    totalcategory4 += categoryColumn9Number;
                    category4.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

                    category4.Column10 = category4Column10Array[i];

                    int categoryColumn11Number = Convert.ToInt32(category4Column11Array[i]);
                    totalTaxcategory4 += categoryColumn11Number;
                    category4.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
                    category4.Column12 = category4Column12Array[i];

                    category4List.Add(category4);
                }

                declarationVat.Categories4 = category4List;
                declarationVat.Categories4TotalAmount = totalcategory4.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));
                declarationVat.Categories4TotalTaxAmount = totalTaxcategory4.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));

                totalAmount += totalcategory4;
                totalTaxAmount += totalTaxcategory4;

            }

            if (category5Column4.Trim().Length > 0)
            {
                string[] category5Column2Array = category5Column2.Split(',');
                string[] category5Column3Array = category5Column3.Split(',');
                string[] category5Column4Array = category5Column4.Split(',');
                string[] category5Column5Array = category5Column5.Split(',');
                string[] category5Column6Array = category5Column6.Split(',');
                string[] category5Column7Array = category5Column7.Split(',');
                string[] category5Column8Array = category5Column8.Split(',');
                string[] category5Column9Array = category5Column9.Split(',');
                string[] category5Column10Array = category5Column10.Split(',');
                string[] category5Column11Array = category5Column11.Split(',');
                string[] category5Column12Array = category5Column12.Split(',');

                List<DeclarationVatCategory> category5List = new List<DeclarationVatCategory>();
                int totalcategory5 = 0;
                int totalTaxcategory5 = 0;

                for (int i = 0; i < category5Column4Array.Length; i++)
                {
                    DeclarationVatCategory category5 = new DeclarationVatCategory();

                    category5.Column2 = category5Column2Array[i];
                    category5.Column3 = category5Column3Array[i];
                    category5.Column4 = category5Column4Array[i];
                    category5.Column5 = category5Column5Array[i];
                    category5.Column6 = category5Column6Array[i];
                    category5.Column7 = category5Column7Array[i];
                    category5.Column8 = category5Column8Array[i];

                    int categoryColumn9Number = Convert.ToInt32(category5Column9Array[i]);
                    totalcategory5 += categoryColumn9Number;
                    category5.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

                    category5.Column10 = category5Column10Array[i];

                    int categoryColumn11Number = Convert.ToInt32(category5Column11Array[i]);
                    totalTaxcategory5 += categoryColumn11Number;
                    category5.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
                    category5.Column12 = category5Column12Array[i];

                    category5List.Add(category5);
                }

                declarationVat.Categories5 = category5List;
                declarationVat.Categories5TotalAmount = totalcategory5.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));
                declarationVat.Categories5TotalTaxAmount = totalTaxcategory5.ToString("N0",
                    CultureInfo.CreateSpecificCulture("vi-VN"));

                totalAmount += totalcategory5;
                totalTaxAmount += totalTaxcategory5;

            }


            declarationVat.TotalAmount = (totalAmount).ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            declarationVat.TotalTaxAmount = (totalTaxAmount).ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));


            return View("InputVatListExport", declarationVat);
        }
        #endregion
    }
}