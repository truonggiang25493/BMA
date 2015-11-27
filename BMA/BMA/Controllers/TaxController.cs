﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Web.WebSockets;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using BMA.Business;
using BMA.Common;
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


            return View();
        }
        public ActionResult TaxDeclaration(int quarter, int year)
        {
            TaxBusiness business = new TaxBusiness();
            DeclarationVatForm result = business.GetVatTaxDeclaration(quarter, year);

            return View(result);
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

            string category2Column2 = form["category2_column2"];
            string category2Column3 = form["category2_column3"];
            string category2Column4 = form["category2_column4"];
            string category2Column5 = form["category2_column5"];
            string category2Column6 = form["category2_column6"];
            string category2Column7 = form["category2_column7"];
            string category2Column8 = form["category2_column8"];

            string category3Column2 = form["category3_column2"];
            string category3Column3 = form["category3_column3"];
            string category3Column4 = form["category3_column4"];
            string category3Column5 = form["category3_column5"];
            string category3Column6 = form["category3_column6"];
            string category3Column7 = form["category3_column7"];
            string category3Column8 = form["category3_column8"];

            string category4Column2 = form["category4_column2"];
            string category4Column3 = form["category4_column3"];
            string category4Column4 = form["category4_column4"];
            string category4Column5 = form["category4_column5"];
            string category4Column6 = form["category4_column6"];
            string category4Column7 = form["category4_column7"];
            string category4Column8 = form["category4_column8"];

            string category5Column2 = form["category5_column2"];
            string category5Column3 = form["category5_column3"];
            string category5Column4 = form["category5_column4"];
            string category5Column5 = form["category5_column5"];
            string category5Column6 = form["category5_column6"];
            string category5Column7 = form["category5_column7"];
            string category5Column8 = form["category5_column8"];

            string signName = form["signName"];

            string createLocation = form["createLocation"];
            string createDay = form["createDay"];


            string agentName = form["agentName"];
            string agentNo = form["agentNo"];

            return 1;
        }

        public ActionResult ExportTaxDeclaration(FormCollection form)
        {

            string taxQuarter = form["taxQuarter"];
            string taxYear = form["taxYear"];

            //Store Info Not get from form; Get from DB

            string vatAgentOwnerName = form["vatAgentOwnerName"];
            string vatAgentName = form["agentName"];
            string vatAgentTaxCode = form["agentTaxCode"];
            string vatAgentNo = form["agentNo"];
            string vatAgentAddress = form["vatAgentAddress"];
            string vatAgentDistrict = form["vatAgentDistrict"];
            string vatAgentProvince = form["vatAgentProvince"];
            string vatAgentPhone = form["vatAgentPhone"];
            string vatAgentFax = form["vatAgentFax"];
            string vatAgentEmail = form["vatAgentEmail"];

            string value20No = form["value20No"];
            string value20Date = form["value20Date"];
            string value21 = form["value21"];
            string value22 = form["value22"];
            string value25 = form["value25"];
            string value36 = form["value36"];
            string value37 = form["value37"];
            string value38 = form["value38"];
            string value39 = form["value39"];
            string value40a = form["value40a"];
            string value40b = form["value40b"];
            string value40 = form["value40"];
            string value41 = form["value41"];
            string value42 = form["value42"];
            string value43 = form["value43"];


            string signName = form["signName"];
            string createLocation = form["createLocation"];
            string createDay = form["createDay"];
            string createMonth = form["createMonth"];

            string outputCategory1Column2 = form["output_category1_column2"];
            string outputCategory1Column3 = form["output_category1_column3"];
            string outputCategory1Column4 = form["output_category1_column4"];
            string outputCategory1Column5 = form["output_category1_column5"];
            string outputCategory1Column6 = form["output_category1_column6"];
            string outputCategory1Column7 = form["output_category1_column7"];
            string outputCategory1Column8 = form["output_category1_column8"];

            string outputCategory2Column2 = form["output_category2_column2"];
            string outputCategory2Column3 = form["output_category2_column3"];
            string outputCategory2Column4 = form["output_category2_column4"];
            string outputCategory2Column5 = form["output_category2_column5"];
            string outputCategory2Column6 = form["output_category2_column6"];
            string outputCategory2Column7 = form["output_category2_column7"];
            string outputCategory2Column8 = form["output_category2_column8"];

            string outputCategory3Column2 = form["output_category3_column2"];
            string outputCategory3Column3 = form["output_category3_column3"];
            string outputCategory3Column4 = form["output_category3_column4"];
            string outputCategory3Column5 = form["output_category3_column5"];
            string outputCategory3Column6 = form["output_category3_column6"];
            string outputCategory3Column7 = form["output_category3_column7"];
            string outputCategory3Column8 = form["output_category3_column8"];

            string outputCategory4Column2 = form["output_category4_column2"];
            string outputCategory4Column3 = form["output_category4_column3"];
            string outputCategory4Column4 = form["output_category4_column4"];
            string outputCategory4Column5 = form["output_category4_column5"];
            string outputCategory4Column6 = form["output_category4_column6"];
            string outputCategory4Column7 = form["output_category4_column7"];
            string outputCategory4Column8 = form["output_category4_column8"];

            string inputCategory1Column2 = form["input_category1_column2"];
            string inputCategory1Column3 = form["input_category1_column3"];
            string inputCategory1Column4 = form["input_category1_column4"];
            string inputCategory1Column5 = form["input_category1_column5"];
            string inputCategory1Column6 = form["input_category1_column6"];
            string inputCategory1Column7 = form["input_category1_column7"];
            string inputCategory1Column8 = form["input_category1_column8"];

            string inputCategory2Column2 = form["input_category2_column2"];
            string inputCategory2Column3 = form["input_category2_column3"];
            string inputCategory2Column4 = form["input_category2_column4"];
            string inputCategory2Column5 = form["input_category2_column5"];
            string inputCategory2Column6 = form["input_category2_column6"];
            string inputCategory2Column7 = form["input_category2_column7"];
            string inputCategory2Column8 = form["input_category2_column8"];

            string inputCategory3Column2 = form["input_category3_column2"];
            string inputCategory3Column3 = form["input_category3_column3"];
            string inputCategory3Column4 = form["input_category3_column4"];
            string inputCategory3Column5 = form["input_category3_column5"];
            string inputCategory3Column6 = form["input_category3_column6"];
            string inputCategory3Column7 = form["input_category3_column7"];
            string inputCategory3Column8 = form["input_category3_column8"];


            BMAEntities db = new BMAEntities();

            StoreInfo storeInfo = db.StoreInfoes.FirstOrDefault();

            #region Process Data

            DeclarationVatForm vatForm = new DeclarationVatForm();

            #region Output

            int totalOutputAmount = 0;
            int haveTaxTotalOutputAmount = 0;
            int totalTaxOutputAmount = 0;

            #region OutputCategory1

            List<DeclarationVatCategory> outputCategory1List = new List<DeclarationVatCategory>();

            if (outputCategory1Column4.Trim().Length > 0)
            {
                string[] outputCategory1Column2Array = outputCategory1Column2.Split(',');
                string[] outputCategory1Column3Array = outputCategory1Column3.Split(',');
                string[] outputCategory1Column4Array = outputCategory1Column4.Split(',');
                string[] outputCategory1Column5Array = outputCategory1Column5.Split(',');
                string[] outputCategory1Column6Array = outputCategory1Column6.Split(',');
                string[] outputCategory1Column7Array = outputCategory1Column7.Split(',');
                string[] outputCategory1Column8Array = outputCategory1Column8.Split(',');

                int totalOutputCategory1 = 0;

                for (int i = 0; i < outputCategory1Column4Array.Length; i++)
                {
                    DeclarationVatCategory outputCategory1 = new DeclarationVatCategory();

                    outputCategory1.Column2 = outputCategory1Column2Array[i];
                    outputCategory1.Column3 = outputCategory1Column3Array[i];
                    outputCategory1.Column4 = outputCategory1Column4Array[i];
                    outputCategory1.Column5 = outputCategory1Column5Array[i];
                    outputCategory1.Column6 = outputCategory1Column6Array[i];
                    outputCategory1.Column7 = outputCategory1Column7Array[i];
                    outputCategory1.Column8 = outputCategory1Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(outputCategory1Column6Array[i]);
                    totalOutputCategory1 += categoryColumn6Number;


                    outputCategory1List.Add(outputCategory1);
                }

                vatForm.OutputCategories1TotalAmount = totalOutputCategory1;

                totalOutputAmount += totalOutputCategory1;
            }

            vatForm.OutputCategories1 = outputCategory1List;

            #endregion

            #region OutputCategory2

            List<DeclarationVatCategory> outputCategory2List = new List<DeclarationVatCategory>();

            if (outputCategory2Column4.Trim().Length > 0)
            {
                string[] outputCategory2Column2Array = outputCategory2Column2.Split(',');
                string[] outputCategory2Column3Array = outputCategory2Column3.Split(',');
                string[] outputCategory2Column4Array = outputCategory2Column4.Split(',');
                string[] outputCategory2Column5Array = outputCategory2Column5.Split(',');
                string[] outputCategory2Column6Array = outputCategory2Column6.Split(',');
                string[] outputCategory2Column7Array = outputCategory2Column7.Split(',');
                string[] outputCategory2Column8Array = outputCategory2Column8.Split(',');

                int totalOutputCategory2 = 0;

                for (int i = 0; i < outputCategory2Column4Array.Length; i++)
                {
                    DeclarationVatCategory outputCategory2 = new DeclarationVatCategory();

                    outputCategory2.Column2 = outputCategory2Column2Array[i];
                    outputCategory2.Column3 = outputCategory2Column3Array[i];
                    outputCategory2.Column4 = outputCategory2Column4Array[i];
                    outputCategory2.Column5 = outputCategory2Column5Array[i];
                    outputCategory2.Column6 = outputCategory2Column6Array[i];
                    outputCategory2.Column7 = outputCategory2Column7Array[i];
                    outputCategory2.Column8 = outputCategory2Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(outputCategory2Column6Array[i]);
                    totalOutputCategory2 += categoryColumn6Number;

                    outputCategory2List.Add(outputCategory2);
                }

                vatForm.OutputCategories2TotalAmount = totalOutputCategory2;

                haveTaxTotalOutputAmount += totalOutputCategory2;
                totalOutputAmount += totalOutputCategory2;
            }

            vatForm.OutputCategories2 = outputCategory2List;

            #endregion

            #region OutputCategory3

            List<DeclarationVatCategory> outputCategory3List = new List<DeclarationVatCategory>();

            if (outputCategory3Column4.Trim().Length > 0)
            {
                string[] outputCategory3Column2Array = outputCategory3Column2.Split(',');
                string[] outputCategory3Column3Array = outputCategory3Column3.Split(',');
                string[] outputCategory3Column4Array = outputCategory3Column4.Split(',');
                string[] outputCategory3Column5Array = outputCategory3Column5.Split(',');
                string[] outputCategory3Column6Array = outputCategory3Column6.Split(',');
                string[] outputCategory3Column7Array = outputCategory3Column7.Split(',');
                string[] outputCategory3Column8Array = outputCategory3Column8.Split(',');


                int totalOutputCategory3 = 0;
                int totalTaxOutputCategory3 = 0;

                for (int i = 0; i < outputCategory3Column4Array.Length; i++)
                {
                    DeclarationVatCategory outputCategory3 = new DeclarationVatCategory();

                    outputCategory3.Column2 = outputCategory3Column2Array[i];
                    outputCategory3.Column3 = outputCategory3Column3Array[i];
                    outputCategory3.Column4 = outputCategory3Column4Array[i];
                    outputCategory3.Column5 = outputCategory3Column5Array[i];
                    outputCategory3.Column6 = outputCategory3Column6Array[i];
                    outputCategory3.Column7 = outputCategory3Column7Array[i];
                    outputCategory3.Column8 = outputCategory3Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(outputCategory3Column6Array[i]);
                    totalOutputCategory3 += categoryColumn6Number;


                    int categoryColumn7Number = Convert.ToInt32(outputCategory3Column7Array[i]);
                    totalTaxOutputCategory3 += categoryColumn7Number;

                    outputCategory3List.Add(outputCategory3);
                }

                vatForm.OutputCategories3TotalAmount = totalOutputCategory3;
                vatForm.OutputCategories3TotalTaxAmount = totalTaxOutputCategory3;

                totalOutputAmount += totalOutputCategory3;
                haveTaxTotalOutputAmount += totalOutputCategory3;
                totalTaxOutputAmount += totalTaxOutputCategory3;
            }

            vatForm.OutputCategories3 = outputCategory3List;

            #endregion

            #region OutputCategory4

            List<DeclarationVatCategory> outputCategory4List = new List<DeclarationVatCategory>();

            if (outputCategory4Column4.Trim().Length > 0)
            {
                string[] outputCategory4Column2Array = outputCategory4Column2.Split(',');
                string[] outputCategory4Column3Array = outputCategory4Column3.Split(',');
                string[] outputCategory4Column4Array = outputCategory4Column4.Split(',');
                string[] outputCategory4Column5Array = outputCategory4Column5.Split(',');
                string[] outputCategory4Column6Array = outputCategory4Column6.Split(',');
                string[] outputCategory4Column7Array = outputCategory4Column7.Split(',');
                string[] outputCategory4Column8Array = outputCategory4Column8.Split(',');

                int totalOutputCategory4 = 0;
                int totalTaxOutputCategory4 = 0;

                for (int i = 0; i < outputCategory4Column4Array.Length; i++)
                {
                    DeclarationVatCategory outputCategory4 = new DeclarationVatCategory();

                    outputCategory4.Column2 = outputCategory4Column2Array[i];
                    outputCategory4.Column3 = outputCategory4Column3Array[i];
                    outputCategory4.Column4 = outputCategory4Column4Array[i];
                    outputCategory4.Column5 = outputCategory4Column5Array[i];
                    outputCategory4.Column6 = outputCategory4Column6Array[i];
                    outputCategory4.Column7 = outputCategory4Column7Array[i];
                    outputCategory4.Column8 = outputCategory4Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(outputCategory4Column6Array[i]);
                    totalOutputCategory4 += categoryColumn6Number;

                    int categoryColumn7Number = Convert.ToInt32(outputCategory4Column7Array[i]);
                    totalTaxOutputCategory4 += categoryColumn7Number;

                    outputCategory4List.Add(outputCategory4);
                }

                vatForm.OutputCategories4TotalAmount = totalOutputCategory4;
                vatForm.OutputCategories4TotalTaxAmount = totalTaxOutputCategory4;

                totalOutputAmount += totalOutputCategory4;
                haveTaxTotalOutputAmount += totalOutputCategory4;
                totalTaxOutputAmount += totalTaxOutputCategory4;
            }

            vatForm.OutputCategories4 = outputCategory4List;

            #endregion


            vatForm.OutputTotalAmount = totalOutputAmount;
            vatForm.OutputTotalTaxAmount = totalTaxOutputAmount;
            vatForm.HaveTaxOutputAmount = haveTaxTotalOutputAmount;

            #endregion

            #region Input

            int totalInputAmount = 0;
            int totalTaxInputAmount = 0;

            #region InputCategory1

            List<DeclarationVatCategory> inputCategory1List = new List<DeclarationVatCategory>();

            if (inputCategory1Column4.Trim().Length > 0)
            {
                string[] inputCategory1Column2Array = inputCategory1Column2.Split(',');
                string[] inputCategory1Column3Array = inputCategory1Column3.Split(',');
                string[] inputCategory1Column4Array = inputCategory1Column4.Split(',');
                string[] inputCategory1Column5Array = inputCategory1Column5.Split(',');
                string[] inputCategory1Column6Array = inputCategory1Column6.Split(',');
                string[] inputCategory1Column7Array = inputCategory1Column7.Split(',');
                string[] inputCategory1Column8Array = inputCategory1Column8.Split(',');

                int totalInputCategory1 = 0;
                int totalTaxInputCategory1 = 0;

                for (int i = 0; i < inputCategory1Column4Array.Length; i++)
                {
                    DeclarationVatCategory inputCategory1 = new DeclarationVatCategory();

                    inputCategory1.Column2 = inputCategory1Column2Array[i];
                    inputCategory1.Column3 = inputCategory1Column3Array[i];
                    inputCategory1.Column4 = inputCategory1Column4Array[i];
                    inputCategory1.Column5 = inputCategory1Column5Array[i];
                    inputCategory1.Column6 = inputCategory1Column6Array[i];
                    inputCategory1.Column7 = inputCategory1Column7Array[i];
                    inputCategory1.Column8 = inputCategory1Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(inputCategory1Column6Array[i]);
                    totalInputCategory1 += categoryColumn6Number;

                    int categoryColumn7Number = Convert.ToInt32(inputCategory1Column7Array[i]);
                    totalTaxInputCategory1 += categoryColumn7Number;

                    inputCategory1List.Add(inputCategory1);
                }

                vatForm.InputCategories1TotalAmount = totalInputCategory1;
                vatForm.InputCategories1TotalTaxAmount = totalTaxInputCategory1;

                totalInputAmount += totalInputCategory1;
                totalTaxInputAmount += totalTaxInputCategory1;
            }

            vatForm.InputCategories1 = inputCategory1List;

            #endregion

            #region InputCategory2

            List<DeclarationVatCategory> inputCategory2List = new List<DeclarationVatCategory>();

            if (inputCategory2Column4.Trim().Length > 0)
            {
                string[] inputCategory2Column2Array = inputCategory2Column2.Split(',');
                string[] inputCategory2Column3Array = inputCategory2Column3.Split(',');
                string[] inputCategory2Column4Array = inputCategory2Column4.Split(',');
                string[] inputCategory2Column5Array = inputCategory2Column5.Split(',');
                string[] inputCategory2Column6Array = inputCategory2Column6.Split(',');
                string[] inputCategory2Column7Array = inputCategory2Column7.Split(',');
                string[] inputCategory2Column8Array = inputCategory2Column8.Split(',');

                int totalInputCategory2 = 0;
                int totalTaxInputCategory2 = 0;

                for (int i = 0; i < inputCategory2Column4Array.Length; i++)
                {
                    DeclarationVatCategory inputCategory2 = new DeclarationVatCategory();

                    inputCategory2.Column2 = inputCategory2Column2Array[i];
                    inputCategory2.Column3 = inputCategory2Column3Array[i];
                    inputCategory2.Column4 = inputCategory2Column4Array[i];
                    inputCategory2.Column5 = inputCategory2Column5Array[i];
                    inputCategory2.Column6 = inputCategory2Column6Array[i];
                    inputCategory2.Column7 = inputCategory2Column7Array[i];
                    inputCategory2.Column8 = inputCategory2Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(inputCategory2Column6Array[i]);
                    totalInputCategory2 += categoryColumn6Number;

                    int categoryColumn7Number = Convert.ToInt32(inputCategory2Column7Array[i]);
                    totalTaxInputCategory2 += categoryColumn7Number;

                    inputCategory2List.Add(inputCategory2);
                }

                vatForm.InputCategories2TotalAmount = totalInputCategory2;
                vatForm.InputCategories2TotalTaxAmount = totalTaxInputCategory2;

                totalInputAmount += totalInputCategory2;
                totalTaxInputAmount += totalTaxInputCategory2;
            }

            vatForm.InputCategories2 = inputCategory2List;

            #endregion

            #region InputCategory3

            List<DeclarationVatCategory> inputCategory3List = new List<DeclarationVatCategory>();

            if (inputCategory3Column4.Trim().Length > 0)
            {
                string[] inputCategory3Column2Array = inputCategory3Column2.Split(',');
                string[] inputCategory3Column3Array = inputCategory3Column3.Split(',');
                string[] inputCategory3Column4Array = inputCategory3Column4.Split(',');
                string[] inputCategory3Column5Array = inputCategory3Column5.Split(',');
                string[] inputCategory3Column6Array = inputCategory3Column6.Split(',');
                string[] inputCategory3Column7Array = inputCategory3Column7.Split(',');
                string[] inputCategory3Column8Array = inputCategory3Column8.Split(',');


                int totalInputCategory3 = 0;
                int totalTaxInputCategory3 = 0;

                for (int i = 0; i < inputCategory3Column4Array.Length; i++)
                {
                    DeclarationVatCategory inputCategory3 = new DeclarationVatCategory();

                    inputCategory3.Column2 = inputCategory3Column2Array[i];
                    inputCategory3.Column3 = inputCategory3Column3Array[i];
                    inputCategory3.Column4 = inputCategory3Column4Array[i];
                    inputCategory3.Column5 = inputCategory3Column5Array[i];
                    inputCategory3.Column6 = inputCategory3Column6Array[i];
                    inputCategory3.Column7 = inputCategory3Column7Array[i];
                    inputCategory3.Column8 = inputCategory3Column8Array[i];

                    int categoryColumn6Number = Convert.ToInt32(inputCategory3Column6Array[i]);
                    totalInputCategory3 += categoryColumn6Number;

                    int categoryColumn7Number = Convert.ToInt32(inputCategory3Column7Array[i]);
                    totalTaxInputCategory3 += categoryColumn7Number;

                    inputCategory3List.Add(inputCategory3);
                }

                vatForm.InputCategories3TotalAmount = totalInputCategory3;
                vatForm.InputCategories3TotalTaxAmount = totalTaxInputCategory3;

            }

            vatForm.InputCategories3 = inputCategory3List;

            #endregion

            vatForm.InputTotalAmount = totalInputAmount;
            vatForm.InputTotalTaxAmount = totalTaxInputAmount;

            #endregion

            #endregion

            #region Create XML File

            XElement root = new XElement("HSoThueDTu");

            XElement HSoKhaiThue = new XElement("HSoKhaiThue");
            HSoKhaiThue.Add(new XAttribute("id", "ID_18"));

            XElement TTinChung = new XElement("TTinChung");

            XElement TTinDVu = new XElement("TTinDVu");

            XElement maDVu = new XElement("maDVu", "HTKK");
            TTinDVu.Add(maDVu);
            XElement tenDVu = new XElement("tenDVu", "HTKK");
            TTinDVu.Add(tenDVu);
            XElement pbanDVu = new XElement("pbanDVu", "3.3.4");
            TTinDVu.Add(pbanDVu);

            TTinChung.Add(TTinDVu);

            XElement TTinTKhaiThue = new XElement("TTinTKhaiThue");

            XElement TKhaiThue = new XElement("TKhaiThue");

            XElement maTKhai = new XElement("maTKhai", "01");
            TKhaiThue.Add(maTKhai);

            XElement tenTKhai = new XElement("tenTKhai", "TỜ KHAI THUẾ GIÁ TRỊ GIA TĂNG (Mẫu số 01/GTGT)");
            TKhaiThue.Add(tenTKhai);

            XElement moTaBMau = new XElement("moTaBMau", "(Ban hành kèm theo Thông tư số 119/2014/TT-BTC ngày 25/8/2014 của Bộ Tài chính)");
            TKhaiThue.Add(moTaBMau);

            XElement pbanTKhaiXML = new XElement("pbanTKhaiXML", "2.0.7");
            TKhaiThue.Add(pbanTKhaiXML);

            XElement loaiTKhai = new XElement("loaiTKhai", "C");
            TKhaiThue.Add(loaiTKhai);

            XElement soLan = new XElement("soLan", "0");
            TKhaiThue.Add(soLan);

            XElement KyKKhaiThue = new XElement("KyKKhaiThue");

            XElement kieuKy = new XElement("kieuKy", "Q");
            KyKKhaiThue.Add(kieuKy);

            XElement kyKKhai = new XElement("kyKKhai", "4/2015");
            KyKKhaiThue.Add(kyKKhai);

            XElement kyKKhaiTuNgay = new XElement("kyKKhaiTuNgay", "01/10/2015");
            KyKKhaiThue.Add(kyKKhaiTuNgay);

            XElement kyKKhaiDenNgay = new XElement("kyKKhaiDenNgay", "31/12/2015");
            KyKKhaiThue.Add(kyKKhaiDenNgay);

            XElement kyKKhaiTuThang = new XElement("kyKKhaiTuThang");
            KyKKhaiThue.Add(kyKKhaiTuThang);

            XElement kyKKhaiDenThang = new XElement("kyKKhaiDenThang");
            KyKKhaiThue.Add(kyKKhaiDenThang);


            TKhaiThue.Add(KyKKhaiThue);

            XElement maCQTNoiNop = new XElement("maCQTNoiNop");
            TKhaiThue.Add(maCQTNoiNop);

            XElement tenCQTNoiNop = new XElement("tenCQTNoiNop", "Chi cục Thuế Quận 12");
            TKhaiThue.Add(tenCQTNoiNop);

            XElement ngayLapTKhai = new XElement("ngayLapTKhai", taxYear + "-" + createMonth + "-" + createDay);
            TKhaiThue.Add(ngayLapTKhai);

            XElement GiaHan = new XElement("GiaHan");

            XElement maLyDoGiaHan = new XElement("maLyDoGiaHan");
            GiaHan.Add(maLyDoGiaHan);

            XElement lyDoGiaHan = new XElement("lyDoGiaHan");
            GiaHan.Add(lyDoGiaHan);

            TKhaiThue.Add(GiaHan);

            XElement nguoiKy = new XElement("nguoiKy", signName);
            TKhaiThue.Add(nguoiKy);

            XElement ngayKy = new XElement("ngayKy", taxYear + "-" + createMonth + "-" + createDay);
            TKhaiThue.Add(ngayKy);

            XElement nganhNgheKD = new XElement("nganhNgheKD");
            TKhaiThue.Add(nganhNgheKD);


            TTinTKhaiThue.Add(TKhaiThue);

            XElement NNT = new XElement("NNT");

            XElement mst = new XElement("mst", storeInfo.TaxCode);
            NNT.Add(mst);

            XElement tenNNT = new XElement("tenNNT", storeInfo.OwnerName);
            NNT.Add(tenNNT);

            XElement dchiNNT = new XElement("dchiNNT", storeInfo.Address);
            NNT.Add(dchiNNT);

            XElement phuongXa = new XElement("phuongXa");
            NNT.Add(phuongXa);

            XElement maHuyenNNT = new XElement("maHuyenNNT");
            NNT.Add(maHuyenNNT);

            XElement tenHuyenNNT = new XElement("tenHuyenNNT", storeInfo.District);
            NNT.Add(tenHuyenNNT);

            XElement maTinhNNT = new XElement("maTinhNNT");
            NNT.Add(maTinhNNT);

            XElement tenTinhNNT = new XElement("tenTinhNNT", storeInfo.Province);
            NNT.Add(tenTinhNNT);

            XElement dthoaiNNT = new XElement("dthoaiNNT", storeInfo.Phonenumber);
            NNT.Add(dthoaiNNT);

            XElement faxNNT = new XElement("faxNNT", storeInfo.Fax);
            NNT.Add(faxNNT);

            XElement emailNNT = new XElement("emailNNT", storeInfo.Email);
            NNT.Add(emailNNT);

            TTinTKhaiThue.Add(NNT);

            XElement DLyThue = new XElement("DLyThue");

            XElement mstDLyThue = new XElement("mstDLyThue", vatAgentTaxCode.Replace(",", ""));
            DLyThue.Add(mstDLyThue);

            XElement tenDLyThue = new XElement("tenDLyThue", vatAgentOwnerName);
            DLyThue.Add(tenDLyThue);

            XElement dchiDLyThue = new XElement("dchiDLyThue", vatAgentAddress);
            DLyThue.Add(dchiDLyThue);

            XElement maHuyenDLyThue = new XElement("maHuyenDLyThue");
            DLyThue.Add(maHuyenDLyThue);

            XElement tenHuyenDLyThue = new XElement("tenHuyenDLyThue", vatAgentDistrict);
            DLyThue.Add(tenHuyenDLyThue);

            XElement maTinhDLyThue = new XElement("maTinhDLyThue");
            DLyThue.Add(maTinhDLyThue);

            XElement tenTinhDLyThue = new XElement("tenTinhDLyThue", vatAgentProvince);
            DLyThue.Add(tenTinhDLyThue);

            XElement dthoaiDLyThue = new XElement("dthoaiDLyThue", vatAgentPhone);
            DLyThue.Add(dthoaiDLyThue);

            XElement faxDLyThue = new XElement("faxDLyThue", vatAgentFax);
            DLyThue.Add(faxDLyThue);

            XElement emailDLyThue = new XElement("emailDLyThue", vatAgentEmail);
            DLyThue.Add(emailDLyThue);

            XElement soHDongDLyThue = new XElement("soHDongDLyThue", value20No);
            DLyThue.Add(soHDongDLyThue);

            XElement ngayKyHDDLyThue = new XElement("ngayKyHDDLyThue", value20Date);
            DLyThue.Add(ngayKyHDDLyThue);

            XElement NVienDLy = new XElement("NVienDLy");

            XElement tenNVienDLyThue = new XElement("tenNVienDLyThue", vatAgentName);
            NVienDLy.Add(tenNVienDLyThue);

            XElement cchiHNghe = new XElement("cchiHNghe", vatAgentNo);
            NVienDLy.Add(cchiHNghe);


            DLyThue.Add(NVienDLy);

            TTinTKhaiThue.Add(DLyThue);

            TTinChung.Add(TTinTKhaiThue);

            HSoKhaiThue.Add(TTinChung);

            XElement CTieuTKhaiChinh = new XElement("CTieuTKhaiChinh");

            XElement tieuMucHachToan = new XElement("tieuMucHachToan", "1701");
            CTieuTKhaiChinh.Add(tieuMucHachToan);

            XElement ct21 = new XElement("ct21", value21);
            CTieuTKhaiChinh.Add(ct21);

            XElement ct22 = new XElement("ct22", value22);
            CTieuTKhaiChinh.Add(ct22);

            XElement GiaTriVaThueGTGTHHDVMuaVao = new XElement("GiaTriVaThueGTGTHHDVMuaVao");


            XElement ct23 = new XElement("ct23", vatForm.InputTotalAmount);
            GiaTriVaThueGTGTHHDVMuaVao.Add(ct23);

            XElement ct24 = new XElement("ct24", vatForm.InputTotalTaxAmount);
            GiaTriVaThueGTGTHHDVMuaVao.Add(ct24);


            CTieuTKhaiChinh.Add(GiaTriVaThueGTGTHHDVMuaVao);

            XElement ct25 = new XElement("ct25", value25);
            CTieuTKhaiChinh.Add(ct25);

            XElement ct26 = new XElement("ct26", vatForm.OutputCategories1TotalAmount);
            CTieuTKhaiChinh.Add(ct26);

            XElement HHDVBRaChiuThueGTGT = new XElement("HHDVBRaChiuThueGTGT");

            XElement ct27 = new XElement("ct27", vatForm.HaveTaxOutputAmount);
            HHDVBRaChiuThueGTGT.Add(ct27);

            XElement ct28 = new XElement("ct28", vatForm.OutputTotalTaxAmount);
            HHDVBRaChiuThueGTGT.Add(ct28);

            CTieuTKhaiChinh.Add(HHDVBRaChiuThueGTGT);

            XElement ct29 = new XElement("ct29", vatForm.OutputCategories2TotalAmount);
            CTieuTKhaiChinh.Add(ct29);

            XElement HHDVBRaChiuTSuat5 = new XElement("HHDVBRaChiuTSuat5");

            XElement ct30 = new XElement("ct30", vatForm.OutputCategories3TotalTaxAmount);
            HHDVBRaChiuTSuat5.Add(ct30);

            XElement ct31 = new XElement("ct31", vatForm.OutputCategories3TotalTaxAmount);
            HHDVBRaChiuTSuat5.Add(ct31);

            CTieuTKhaiChinh.Add(HHDVBRaChiuTSuat5);

            XElement HHDVBRaChiuTSuat10 = new XElement("HHDVBRaChiuTSuat10");

            XElement ct32 = new XElement("ct32", vatForm.OutputCategories4TotalAmount);
            HHDVBRaChiuTSuat10.Add(ct32);

            XElement ct33 = new XElement("ct33", vatForm.OutputCategories4TotalTaxAmount);
            HHDVBRaChiuTSuat10.Add(ct33);

            CTieuTKhaiChinh.Add(HHDVBRaChiuTSuat10);

            XElement TongDThuVaThueGTGTHHDVBRa = new XElement("TongDThuVaThueGTGTHHDVBRa");

            XElement ct34 = new XElement("ct34", vatForm.OutputTotalAmount);
            TongDThuVaThueGTGTHHDVBRa.Add(ct34);

            XElement ct35 = new XElement("ct35", vatForm.OutputTotalTaxAmount);
            TongDThuVaThueGTGTHHDVBRa.Add(ct35);

            CTieuTKhaiChinh.Add(TongDThuVaThueGTGTHHDVBRa);

            XElement ct36 = new XElement("ct36", value36);
            CTieuTKhaiChinh.Add(ct36);

            XElement ct37 = new XElement("ct37", value37);
            CTieuTKhaiChinh.Add(ct37);

            XElement ct38 = new XElement("ct38", value38);
            CTieuTKhaiChinh.Add(ct38);

            XElement ct39 = new XElement("ct39", value39);
            CTieuTKhaiChinh.Add(ct39);

            XElement ct40 = new XElement("ct40", value40);
            CTieuTKhaiChinh.Add(ct40);

            XElement ct40a = new XElement("ct40a", value40a);
            CTieuTKhaiChinh.Add(ct40a);

            XElement ct40b = new XElement("ct40b", value40b);
            CTieuTKhaiChinh.Add(ct40b);

            XElement ct41 = new XElement("ct41", value41);
            CTieuTKhaiChinh.Add(ct41);

            XElement ct42 = new XElement("ct42", value42);
            CTieuTKhaiChinh.Add(ct42);

            XElement ct43 = new XElement("ct43", value43);
            CTieuTKhaiChinh.Add(ct43);

            HSoKhaiThue.Add(CTieuTKhaiChinh);

            XElement PLuc = new XElement("PLuc");

            #region PL01_1_GTGT
            XElement PL01_1_GTGT = new XElement("PL01_1_GTGT");

            XElement HHDVKChiuThue = new XElement("HHDVKChiuThue");

            XElement ChiTietHHDVKChiuThue = new XElement("ChiTietHHDVKChiuThue");
            if (vatForm.OutputCategories1 != null && vatForm.OutputCategories1.Count > 0)
            {
                for (int i = 0; i < vatForm.OutputCategories1.Count; i++)
                {
                    XElement HDonBRa = new XElement("HDonBRa");
                    HDonBRa.Add(new XAttribute("id", "ID_" + (i + 1)));

                    XElement kyHieuMauHDon = new XElement("kyHieuMauHDon");
                    HDonBRa.Add(kyHieuMauHDon);

                    XElement kyHieuHDon = new XElement("kyHieuHDon");
                    HDonBRa.Add(kyHieuHDon);

                    XElement soHDon = new XElement("soHDon", vatForm.OutputCategories1.ElementAt(i).Column2);
                    HDonBRa.Add(soHDon);

                    XElement ngayPHanh = new XElement("ngayPHanh", DateTime.ParseExact(vatForm.OutputCategories1.ElementAt(i).Column3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                    HDonBRa.Add(ngayPHanh);

                    XElement tenNMUA = new XElement("tenNMUA", vatForm.OutputCategories1.ElementAt(i).Column4);
                    HDonBRa.Add(tenNMUA);

                    XElement mstNMUA = new XElement("mstNMUA", vatForm.OutputCategories1.ElementAt(i).Column5);
                    HDonBRa.Add(mstNMUA);

                    XElement matHang = new XElement("matHang");
                    HDonBRa.Add(matHang);

                    XElement dsoBanChuaThue = new XElement("dsoBanChuaThue", vatForm.OutputCategories1.ElementAt(i).Column6);
                    HDonBRa.Add(dsoBanChuaThue);

                    XElement thueGTGT = new XElement("thueGTGT", vatForm.OutputCategories1.ElementAt(i).Column7);
                    HDonBRa.Add(thueGTGT);

                    XElement ghiChu = new XElement("ghiChu", vatForm.OutputCategories1.ElementAt(i).Column8);

                    ChiTietHHDVKChiuThue.Add(HDonBRa);
                }
            }


            HHDVKChiuThue.Add(ChiTietHHDVKChiuThue);

            XElement tongDThuBRaKChiuThue = new XElement("tongDThuBRaKChiuThue", vatForm.OutputCategories1TotalAmount);
            HHDVKChiuThue.Add(tongDThuBRaKChiuThue);

            PL01_1_GTGT.Add(HHDVKChiuThue);

            #region 0%

            XElement HHDVThue0 = new XElement("HHDVThue0");

            XElement ChiTietHHDVThue0 = new XElement("ChiTietHHDVThue0");
            if (vatForm.OutputCategories2 != null && vatForm.OutputCategories2.Count > 0)
            {
                for (int i = 0; i < vatForm.OutputCategories2.Count; i++)
                {
                    XElement HDonBRa = new XElement("HDonBRa");
                    HDonBRa.Add(new XAttribute("id", "ID_" + (i + 1)));

                    XElement kyHieuMauHDon = new XElement("kyHieuMauHDon");
                    HDonBRa.Add(kyHieuMauHDon);

                    XElement kyHieuHDon = new XElement("kyHieuHDon");
                    HDonBRa.Add(kyHieuHDon);

                    XElement soHDon = new XElement("soHDon", vatForm.OutputCategories2.ElementAt(i).Column2);
                    HDonBRa.Add(soHDon);

                    XElement ngayPHanh = new XElement("ngayPHanh", DateTime.ParseExact(vatForm.OutputCategories2.ElementAt(i).Column3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                    HDonBRa.Add(ngayPHanh);

                    XElement tenNMUA = new XElement("tenNMUA", vatForm.OutputCategories2.ElementAt(i).Column4);
                    HDonBRa.Add(tenNMUA);

                    XElement mstNMUA = new XElement("mstNMUA", vatForm.OutputCategories2.ElementAt(i).Column5);
                    HDonBRa.Add(mstNMUA);

                    XElement matHang = new XElement("matHang");
                    HDonBRa.Add(matHang);

                    XElement dsoBanChuaThue = new XElement("dsoBanChuaThue", vatForm.OutputCategories2.ElementAt(i).Column6);
                    HDonBRa.Add(dsoBanChuaThue);

                    XElement thueGTGT = new XElement("thueGTGT", vatForm.OutputCategories2.ElementAt(i).Column7);
                    HDonBRa.Add(thueGTGT);

                    XElement ghiChu = new XElement("ghiChu", vatForm.OutputCategories2.ElementAt(i).Column8);

                    ChiTietHHDVThue0.Add(HDonBRa);
                }
            }


            HHDVThue0.Add(ChiTietHHDVThue0);

            XElement tongDThuBRaThue0 = new XElement("tongDThuBRaKChiuThue", vatForm.OutputCategories1TotalAmount);
            HHDVThue0.Add(tongDThuBRaThue0);

            XElement tongThueBRaThue0 = new XElement("tongThueBRaThue0", "0");
            HHDVThue0.Add(tongThueBRaThue0);

            PL01_1_GTGT.Add(HHDVThue0);

            #endregion


            #region 5%

            XElement HHDVThue5 = new XElement("HHDVThue5");

            XElement ChiTietHHDVThue5 = new XElement("ChiTietHHDVThue5");
            if (vatForm.OutputCategories3 != null && vatForm.OutputCategories3.Count > 0)
            {
                for (int i = 0; i < vatForm.OutputCategories3.Count; i++)
                {
                    XElement HDonBRa = new XElement("HDonBRa");
                    HDonBRa.Add(new XAttribute("id", "ID_" + (i + 1)));

                    XElement kyHieuMauHDon = new XElement("kyHieuMauHDon");
                    HDonBRa.Add(kyHieuMauHDon);

                    XElement kyHieuHDon = new XElement("kyHieuHDon");
                    HDonBRa.Add(kyHieuHDon);

                    XElement soHDon = new XElement("soHDon", vatForm.OutputCategories3.ElementAt(i).Column2);
                    HDonBRa.Add(soHDon);

                    XElement ngayPHanh = new XElement("ngayPHanh", DateTime.ParseExact(vatForm.OutputCategories3.ElementAt(i).Column3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                    HDonBRa.Add(ngayPHanh);

                    XElement tenNMUA = new XElement("tenNMUA", vatForm.OutputCategories3.ElementAt(i).Column4);
                    HDonBRa.Add(tenNMUA);

                    XElement mstNMUA = new XElement("mstNMUA", vatForm.OutputCategories3.ElementAt(i).Column5);
                    HDonBRa.Add(mstNMUA);

                    XElement matHang = new XElement("matHang");
                    HDonBRa.Add(matHang);

                    XElement dsoBanChuaThue = new XElement("dsoBanChuaThue", vatForm.OutputCategories3.ElementAt(i).Column6);
                    HDonBRa.Add(dsoBanChuaThue);

                    XElement thueGTGT = new XElement("thueGTGT", vatForm.OutputCategories3.ElementAt(i).Column7);
                    HDonBRa.Add(thueGTGT);

                    XElement ghiChu = new XElement("ghiChu", vatForm.OutputCategories3.ElementAt(i).Column8);

                    ChiTietHHDVThue5.Add(HDonBRa);
                }
            }


            HHDVThue5.Add(ChiTietHHDVThue5);

            XElement tongDThuBRaThue5 = new XElement("tongDThuBRaThue5", vatForm.OutputCategories3TotalAmount);
            HHDVThue5.Add(tongDThuBRaThue5);

            XElement tongThueBRaThue5 = new XElement("tongThueBRaThue5", vatForm.OutputCategories3TotalTaxAmount);
            HHDVThue5.Add(tongThueBRaThue5);

            PL01_1_GTGT.Add(HHDVThue5);

            #endregion


            #region 10%

            XElement HHDVThue10 = new XElement("HHDVThue10");

            XElement ChiTietHHDVThue10 = new XElement("ChiTietHHDVThue10");
            if (vatForm.OutputCategories4 != null && vatForm.OutputCategories4.Count > 0)
            {
                for (int i = 0; i < vatForm.OutputCategories4.Count; i++)
                {
                    XElement HDonBRa = new XElement("HDonBRa");
                    HDonBRa.Add(new XAttribute("id", "ID_" + (i + 1)));

                    XElement kyHieuMauHDon = new XElement("kyHieuMauHDon");
                    HDonBRa.Add(kyHieuMauHDon);

                    XElement kyHieuHDon = new XElement("kyHieuHDon");
                    HDonBRa.Add(kyHieuHDon);

                    XElement soHDon = new XElement("soHDon", vatForm.OutputCategories4.ElementAt(i).Column2);
                    HDonBRa.Add(soHDon);

                    XElement ngayPHanh = new XElement("ngayPHanh", DateTime.ParseExact(vatForm.OutputCategories4.ElementAt(i).Column3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                    HDonBRa.Add(ngayPHanh);

                    XElement tenNMUA = new XElement("tenNMUA", vatForm.OutputCategories4.ElementAt(i).Column4);
                    HDonBRa.Add(tenNMUA);

                    XElement mstNMUA = new XElement("mstNMUA", vatForm.OutputCategories4.ElementAt(i).Column5);
                    HDonBRa.Add(mstNMUA);

                    XElement matHang = new XElement("matHang");
                    HDonBRa.Add(matHang);

                    XElement dsoBanChuaThue = new XElement("dsoBanChuaThue", vatForm.OutputCategories4.ElementAt(i).Column6);
                    HDonBRa.Add(dsoBanChuaThue);

                    XElement thueGTGT = new XElement("thueGTGT", vatForm.OutputCategories4.ElementAt(i).Column7);
                    HDonBRa.Add(thueGTGT);

                    XElement ghiChu = new XElement("ghiChu", vatForm.OutputCategories4.ElementAt(i).Column8);

                    ChiTietHHDVThue10.Add(HDonBRa);
                }
            }


            HHDVThue10.Add(ChiTietHHDVThue10);

            XElement tongDThuBRaThue10 = new XElement("tongDThuBRaThue10", vatForm.OutputCategories4TotalAmount);
            HHDVThue10.Add(tongDThuBRaThue10);

            XElement tongThueBRaThue10 = new XElement("tongThueBRaThue10", vatForm.OutputCategories4TotalTaxAmount);
            HHDVThue10.Add(tongThueBRaThue10);

            PL01_1_GTGT.Add(HHDVThue10);

            #endregion

            XElement HHDVBRaKhongTHop = new XElement("HHDVBRaKhongTHop");

            XElement ChiTietHHDVBRaKhongTHop = new XElement("ChiTietHHDVBRaKhongTHop");
            HHDVBRaKhongTHop.Add(ChiTietHHDVBRaKhongTHop);

            XElement tongDThuBRaKoKKhai = new XElement("tongDThuBRaKoKKhai", "0");
            HHDVBRaKhongTHop.Add(tongDThuBRaKoKKhai);

            XElement tongThueBRaKoKKhai = new XElement("tongThueBRaKoKKhai", "0");
            HHDVBRaKhongTHop.Add(HHDVBRaKhongTHop);

            PL01_1_GTGT.Add(HHDVBRaKhongTHop);

            XElement tongDThuBRa = new XElement("tongDThuBRa", vatForm.OutputTotalAmount);
            PL01_1_GTGT.Add(tongDThuBRa);

            XElement tongDThuBRaChiuThue = new XElement("tongDThuBRaChiuThue", vatForm.HaveTaxOutputAmount);
            PL01_1_GTGT.Add(tongDThuBRaChiuThue);

            XElement tongThueBRa = new XElement("tongThueBRa", vatForm.OutputTotalTaxAmount);
            PL01_1_GTGT.Add(tongThueBRa);

            PLuc.Add(PL01_1_GTGT);
            #endregion


            #region PL01_2_GTGT
            XElement PL01_2_GTGT = new XElement("PL01_2_GTGT");
            #region HHDVDRieng


            XElement HHDVDRieng = new XElement("HHDVDRieng");

            XElement ChiTietHHDVDRieng = new XElement("ChiTietHHDVDRieng");
            if (vatForm.InputCategories1 != null && vatForm.InputCategories1.Count > 0)
            {
                for (int i = 0; i < vatForm.InputCategories1.Count; i++)
                {
                    XElement HDonMVao = new XElement("HDonMVao");
                    HDonMVao.Add(new XAttribute("id", "ID_" + (i + 1)));

                    XElement kyHieuMauHDon = new XElement("kyHieuMauHDon");
                    HDonMVao.Add(kyHieuMauHDon);

                    XElement kyHieuHDon = new XElement("kyHieuHDon");
                    HDonMVao.Add(kyHieuHDon);

                    XElement soHDon = new XElement("soHDon", vatForm.InputCategories1.ElementAt(i).Column2);
                    HDonMVao.Add(soHDon);

                    XElement ngayPHanh = new XElement("ngayPHanh", DateTime.ParseExact(vatForm.InputCategories1.ElementAt(i).Column3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                    HDonMVao.Add(ngayPHanh);

                    XElement tenNBAN = new XElement("tenNBAN", vatForm.InputCategories1.ElementAt(i).Column4);
                    HDonMVao.Add(tenNBAN);

                    XElement mstNBAN = new XElement("mstNBAN", vatForm.InputCategories1.ElementAt(i).Column5);
                    HDonMVao.Add(mstNBAN);

                    XElement matHang = new XElement("matHang");
                    HDonMVao.Add(matHang);

                    XElement thueSuat = new XElement("thueSuat", "0");
                    HDonMVao.Add(thueSuat);

                    XElement dsoMuaChuaThue = new XElement("dsoMuaChuaThue", vatForm.InputCategories1.ElementAt(i).Column6);
                    HDonMVao.Add(dsoMuaChuaThue);

                    XElement thueGTGT = new XElement("thueGTGT", vatForm.InputCategories1.ElementAt(i).Column7);
                    HDonMVao.Add(thueGTGT);

                    XElement ghiChu = new XElement("ghiChu", vatForm.InputCategories1.ElementAt(i).Column8);

                    ChiTietHHDVDRieng.Add(HDonMVao);
                }
            }


            HHDVDRieng.Add(ChiTietHHDVDRieng);

            XElement tongGTriMVao = new XElement("tongGTriMVao", vatForm.InputCategories1TotalAmount);
            HHDVDRieng.Add(tongGTriMVao);

            XElement tongThueMVao = new XElement("tongThueMVao", vatForm.InputCategories1TotalTaxAmount);
            HHDVDRieng.Add(tongThueMVao);

            PL01_2_GTGT.Add(HHDVDRieng);
            #endregion

            #region HHDVKhongKTru

            XElement HHDVKhongKTru = new XElement("HHDVKhongKTru");

            XElement ChiTietHHDVKhongKTru = new XElement("ChiTietHHDVKhongKTru");
            HHDVKhongKTru.Add(HHDVKhongKTru);

            XElement tongGTriMVao1 = new XElement("tongGTriMVao", "0");
            HHDVKhongKTru.Add(tongGTriMVao1);

            XElement tongThueMVao1 = new XElement("tongThueMVao", "0");
            HHDVKhongKTru.Add(tongThueMVao1);

            PL01_2_GTGT.Add(HHDVKhongKTru);

            #endregion

            #region HHDVDChung


            XElement HHDVDChung = new XElement("HHDVDChung");

            XElement ChiTietHHDVDChung = new XElement("ChiTietHHDVDChung");
            if (vatForm.InputCategories2 != null && vatForm.InputCategories2.Count > 0)
            {
                for (int i = 0; i < vatForm.InputCategories2.Count; i++)
                {
                    XElement HDonMVao = new XElement("HDonMVao");
                    HDonMVao.Add(new XAttribute("id", "ID_" + (i + 1)));

                    XElement kyHieuMauHDon = new XElement("kyHieuMauHDon");
                    HDonMVao.Add(kyHieuMauHDon);

                    XElement kyHieuHDon = new XElement("kyHieuHDon");
                    HDonMVao.Add(kyHieuHDon);

                    XElement soHDon = new XElement("soHDon", vatForm.InputCategories2.ElementAt(i).Column2);
                    HDonMVao.Add(soHDon);

                    XElement ngayPHanh = new XElement("ngayPHanh", DateTime.ParseExact(vatForm.InputCategories2.ElementAt(i).Column3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd"));
                    HDonMVao.Add(ngayPHanh);

                    XElement tenNBAN = new XElement("tenNBAN", vatForm.InputCategories2.ElementAt(i).Column4);
                    HDonMVao.Add(tenNBAN);

                    XElement mstNBAN = new XElement("mstNBAN", vatForm.InputCategories2.ElementAt(i).Column5);
                    HDonMVao.Add(mstNBAN);

                    XElement matHang = new XElement("matHang");
                    HDonMVao.Add(matHang);

                    XElement thueSuat = new XElement("thueSuat", "0");
                    HDonMVao.Add(thueSuat);

                    XElement dsoMuaChuaThue = new XElement("dsoMuaChuaThue", vatForm.InputCategories2.ElementAt(i).Column6);
                    HDonMVao.Add(dsoMuaChuaThue);

                    XElement thueGTGT = new XElement("thueGTGT", vatForm.InputCategories2.ElementAt(i).Column7);
                    HDonMVao.Add(thueGTGT);

                    XElement ghiChu = new XElement("ghiChu", vatForm.InputCategories2.ElementAt(i).Column8);

                    ChiTietHHDVDChung.Add(HDonMVao);
                }
            }


            HHDVDChung.Add(ChiTietHHDVDChung);

            XElement tongGTriMVao3 = new XElement("tongGTriMVao", vatForm.InputCategories2TotalAmount);
            HHDVDChung.Add(tongGTriMVao3);

            XElement tongThueMVao3 = new XElement("tongThueMVao", vatForm.InputCategories2TotalTaxAmount);
            HHDVDChung.Add(tongThueMVao3);

            PL01_2_GTGT.Add(HHDVDChung);
            #endregion

            #region HHDVKTru

            XElement HHDVKTru = new XElement("HHDVKTru");

            XElement ChiTietHHDVKTru = new XElement("ChiTietHHDVKTru");
            HHDVKTru.Add(ChiTietHHDVKTru);

            XElement tongGTriMVao2 = new XElement("tongGTriMVao", "0");
            HHDVKTru.Add(tongGTriMVao2);

            XElement tongThueMVao2 = new XElement("tongThueMVao", "0");
            HHDVKTru.Add(tongThueMVao2);

            PL01_2_GTGT.Add(HHDVKTru);

            #endregion

            #region HHDVMVaoKhongTHop

            XElement HHDVMVaoKhongTHop = new XElement("HHDVMVaoKhongTHop");

            XElement ChiTietHHDVMVaoKhongTHop = new XElement("ChiTietHHDVMVaoKhongTHop");
            HHDVMVaoKhongTHop.Add(ChiTietHHDVKTru);

            XElement tongGTriMVao4 = new XElement("tongGTriMVao", "0");
            HHDVMVaoKhongTHop.Add(tongGTriMVao4);

            XElement tongThueMVao4 = new XElement("tongThueMVao", "0");
            HHDVMVaoKhongTHop.Add(tongThueMVao4);

            PL01_2_GTGT.Add(HHDVMVaoKhongTHop);

            #endregion


            XElement tongGTriMVao5 = new XElement("tongGTriMVao", vatForm.InputTotalAmount);
            PL01_2_GTGT.Add(tongGTriMVao5);


            XElement tongThueMVao5 = new XElement("tongThueMVao", vatForm.InputTotalTaxAmount);
            PL01_2_GTGT.Add(tongThueMVao5);

            PLuc.Add(PL01_2_GTGT);
            #endregion


            HSoKhaiThue.Add(PLuc);

            root.Add(HSoKhaiThue);

            var xs = new XmlSerializer(root.GetType());
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);

            xs.Serialize(streamWriter, root);
            stream.Position = 0;
            return File(stream, "application/xml", "MyXml.xml");

            #endregion


            //DeclarationVatForm declarationVat = new DeclarationVatForm();



            //if (vatAgentTaxCode != null)
            //{
            //    declarationVat.VatAgentTaxCode = vatAgentTaxCode.Replace(",", "");
            //}

            //declarationVat.VatAgentName = vatAgentName;
            //declarationVat.VatAgentNo = vatAgentNo;
            //declarationVat.SignName = signName;
            //declarationVat.CreateLocation = createLocation;
            ////declarationVat.CreateDay = createDay;

            //int totalAmount = 0;
            //int totalTaxAmount = 0;


            //if (category1Column4.Trim().Length > 0)
            //{
            //    string[] category1Column2Array = category1Column2.Split(',');
            //    string[] category1Column3Array = category1Column3.Split(',');
            //    string[] category1Column4Array = category1Column4.Split(',');
            //    string[] category1Column5Array = category1Column5.Split(',');
            //    string[] category1Column6Array = category1Column6.Split(',');
            //    string[] category1Column7Array = category1Column7.Split(',');
            //    string[] category1Column8Array = category1Column8.Split(',');
            //    string[] category1Column9Array = category1Column9.Split(',');
            //    string[] category1Column10Array = category1Column10.Split(',');
            //    string[] category1Column11Array = category1Column11.Split(',');
            //    string[] category1Column12Array = category1Column12.Split(',');

            //    List<DeclarationVatCategory> category1List = new List<DeclarationVatCategory>();
            //    int totalCategory1 = 0;
            //    int totalTaxCategory1 = 0;

            //    for (int i = 0; i < category1Column2Array.Length; i++)
            //    {
            //        DeclarationVatCategory category1 = new DeclarationVatCategory();

            //        category1.Column2 = category1Column2Array[i];
            //        category1.Column3 = category1Column3Array[i];
            //        category1.Column4 = category1Column4Array[i];
            //        category1.Column5 = category1Column5Array[i];
            //        category1.Column6 = category1Column6Array[i];
            //        category1.Column7 = category1Column7Array[i];
            //        category1.Column8 = category1Column8Array[i];

            //        int categoryColumn9Number = Convert.ToInt32(category1Column9Array[i]);
            //        totalCategory1 += categoryColumn9Number;
            //        category1.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            //        category1.Column10 = category1Column10Array[i];

            //        int categoryColumn11Number = Convert.ToInt32(category1Column11Array[i]);
            //        totalTaxCategory1 += categoryColumn11Number;
            //        category1.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            //        category1.Column12 = category1Column12Array[i];

            //        category1List.Add(category1);
            //    }

            //    // declarationVat.Categories1 = category1List;
            //    //declarationVat.Categories1TotalAmount = totalCategory1.ToString("N0",
            //    //    CultureInfo.CreateSpecificCulture("vi-VN"));
            //    //declarationVat.Categories1TotalTaxAmount = totalTaxCategory1.ToString("N0",
            //    //  CultureInfo.CreateSpecificCulture("vi-VN"));

            //    totalAmount += totalCategory1;
            //    totalTaxAmount += totalTaxCategory1;

            //}


            //if (category2Column4.Trim().Length > 0)
            //{
            //    string[] category2Column2Array = category2Column2.Split(',');
            //    string[] category2Column3Array = category2Column3.Split(',');
            //    string[] category2Column4Array = category2Column4.Split(',');
            //    string[] category2Column5Array = category2Column5.Split(',');
            //    string[] category2Column6Array = category2Column6.Split(',');
            //    string[] category2Column7Array = category2Column7.Split(',');
            //    string[] category2Column8Array = category2Column8.Split(',');
            //    string[] category2Column9Array = category2Column9.Split(',');
            //    string[] category2Column10Array = category2Column10.Split(',');
            //    string[] category2Column11Array = category2Column11.Split(',');
            //    string[] category2Column12Array = category2Column12.Split(',');

            //    List<DeclarationVatCategory> category2List = new List<DeclarationVatCategory>();
            //    int totalcategory2 = 0;
            //    int totalTaxcategory2 = 0;

            //    for (int i = 0; i < category2Column4Array.Length; i++)
            //    {
            //        DeclarationVatCategory category2 = new DeclarationVatCategory();

            //        category2.Column2 = category2Column2Array[i];
            //        category2.Column3 = category2Column3Array[i];
            //        category2.Column4 = category2Column4Array[i];
            //        category2.Column5 = category2Column5Array[i];
            //        category2.Column6 = category2Column6Array[i];
            //        category2.Column7 = category2Column7Array[i];
            //        category2.Column8 = category2Column8Array[i];

            //        int categoryColumn9Number = Convert.ToInt32(category2Column9Array[i]);
            //        totalcategory2 += categoryColumn9Number;
            //        category2.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            //        category2.Column10 = category2Column10Array[i];

            //        int categoryColumn11Number = Convert.ToInt32(category2Column11Array[i]);
            //        totalTaxcategory2 += categoryColumn11Number;
            //        category2.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            //        category2.Column12 = category2Column12Array[i];

            //        category2List.Add(category2);
            //    }

            //    //declarationVat.Categories2 = category2List;
            //    //declarationVat.Categories2TotalAmount = totalcategory2.ToString("N0",
            //    //     CultureInfo.CreateSpecificCulture("vi-VN"));
            //    //declarationVat.Categories2TotalTaxAmount = totalTaxcategory2.ToString("N0",
            //    //   CultureInfo.CreateSpecificCulture("vi-VN"));

            //    totalAmount += totalcategory2;
            //    totalTaxAmount += totalTaxcategory2;

            //}

            //if (category3Column4.Trim().Length > 0)
            //{
            //    string[] category3Column2Array = category3Column2.Split(',');
            //    string[] category3Column3Array = category3Column3.Split(',');
            //    string[] category3Column4Array = category3Column4.Split(',');
            //    string[] category3Column5Array = category3Column5.Split(',');
            //    string[] category3Column6Array = category3Column6.Split(',');
            //    string[] category3Column7Array = category3Column7.Split(',');
            //    string[] category3Column8Array = category3Column8.Split(',');
            //    string[] category3Column9Array = category3Column9.Split(',');
            //    string[] category3Column10Array = category3Column10.Split(',');
            //    string[] category3Column11Array = category3Column11.Split(',');
            //    string[] category3Column12Array = category3Column12.Split(',');

            //    List<DeclarationVatCategory> category3List = new List<DeclarationVatCategory>();
            //    int totalcategory3 = 0;
            //    int totalTaxcategory3 = 0;

            //    for (int i = 0; i < category3Column4Array.Length; i++)
            //    {
            //        DeclarationVatCategory category3 = new DeclarationVatCategory();

            //        category3.Column2 = category3Column2Array[i];
            //        category3.Column3 = category3Column3Array[i];
            //        category3.Column4 = category3Column4Array[i];
            //        category3.Column5 = category3Column5Array[i];
            //        category3.Column6 = category3Column6Array[i];
            //        category3.Column7 = category3Column7Array[i];
            //        category3.Column8 = category3Column8Array[i];

            //        int categoryColumn9Number = Convert.ToInt32(category3Column9Array[i]);
            //        totalcategory3 += categoryColumn9Number;
            //        category3.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            //        category3.Column10 = category3Column10Array[i];

            //        int categoryColumn11Number = Convert.ToInt32(category3Column11Array[i]);
            //        totalTaxcategory3 += categoryColumn11Number;
            //        category3.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            //        category3.Column12 = category3Column12Array[i];

            //        category3List.Add(category3);
            //    }

            //    //declarationVat.Categories3 = category3List;
            //    //declarationVat.Categories3TotalAmount = totalcategory3.ToString("N0",
            //    //CultureInfo.CreateSpecificCulture("vi-VN"));
            //    //declarationVat.Categories3TotalTaxAmount = totalTaxcategory3.ToString("N0",
            //    //  CultureInfo.CreateSpecificCulture("vi-VN"));

            //    totalAmount += totalcategory3;
            //    totalTaxAmount += totalTaxcategory3;

            //}

            //if (category4Column4.Trim().Length > 0)
            //{
            //    string[] category4Column2Array = category4Column2.Split(',');
            //    string[] category4Column3Array = category4Column3.Split(',');
            //    string[] category4Column4Array = category4Column4.Split(',');
            //    string[] category4Column5Array = category4Column5.Split(',');
            //    string[] category4Column6Array = category4Column6.Split(',');
            //    string[] category4Column7Array = category4Column7.Split(',');
            //    string[] category4Column8Array = category4Column8.Split(',');
            //    string[] category4Column9Array = category4Column9.Split(',');
            //    string[] category4Column10Array = category4Column10.Split(',');
            //    string[] category4Column11Array = category4Column11.Split(',');
            //    string[] category4Column12Array = category4Column12.Split(',');

            //    List<DeclarationVatCategory> category4List = new List<DeclarationVatCategory>();
            //    int totalcategory4 = 0;
            //    int totalTaxcategory4 = 0;

            //    for (int i = 0; i < category4Column4Array.Length; i++)
            //    {
            //        DeclarationVatCategory category4 = new DeclarationVatCategory();

            //        category4.Column2 = category4Column2Array[i];
            //        category4.Column3 = category4Column3Array[i];
            //        category4.Column4 = category4Column4Array[i];
            //        category4.Column5 = category4Column5Array[i];
            //        category4.Column6 = category4Column6Array[i];
            //        category4.Column7 = category4Column7Array[i];
            //        category4.Column8 = category4Column8Array[i];

            //        int categoryColumn9Number = Convert.ToInt32(category4Column9Array[i]);
            //        totalcategory4 += categoryColumn9Number;
            //        category4.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            //        category4.Column10 = category4Column10Array[i];

            //        int categoryColumn11Number = Convert.ToInt32(category4Column11Array[i]);
            //        totalTaxcategory4 += categoryColumn11Number;
            //        category4.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            //        category4.Column12 = category4Column12Array[i];

            //        category4List.Add(category4);
            //    }

            //    //declarationVat.Categories4 = category4List;
            //    //declarationVat.Categories4TotalAmount = totalcategory4.ToString("N0",
            //    //    CultureInfo.CreateSpecificCulture("vi-VN"));
            //    //declarationVat.Categories4TotalTaxAmount = totalTaxcategory4.ToString("N0",
            //    //    CultureInfo.CreateSpecificCulture("vi-VN"));

            //    totalAmount += totalcategory4;
            //    totalTaxAmount += totalTaxcategory4;

            //}

            //if (category5Column4.Trim().Length > 0)
            //{
            //    string[] category5Column2Array = category5Column2.Split(',');
            //    string[] category5Column3Array = category5Column3.Split(',');
            //    string[] category5Column4Array = category5Column4.Split(',');
            //    string[] category5Column5Array = category5Column5.Split(',');
            //    string[] category5Column6Array = category5Column6.Split(',');
            //    string[] category5Column7Array = category5Column7.Split(',');
            //    string[] category5Column8Array = category5Column8.Split(',');
            //    string[] category5Column9Array = category5Column9.Split(',');
            //    string[] category5Column10Array = category5Column10.Split(',');
            //    string[] category5Column11Array = category5Column11.Split(',');
            //    string[] category5Column12Array = category5Column12.Split(',');

            //    List<DeclarationVatCategory> category5List = new List<DeclarationVatCategory>();
            //    int totalcategory5 = 0;
            //    int totalTaxcategory5 = 0;

            //    for (int i = 0; i < category5Column4Array.Length; i++)
            //    {
            //        DeclarationVatCategory category5 = new DeclarationVatCategory();

            //        category5.Column2 = category5Column2Array[i];
            //        category5.Column3 = category5Column3Array[i];
            //        category5.Column4 = category5Column4Array[i];
            //        category5.Column5 = category5Column5Array[i];
            //        category5.Column6 = category5Column6Array[i];
            //        category5.Column7 = category5Column7Array[i];
            //        category5.Column8 = category5Column8Array[i];

            //        int categoryColumn9Number = Convert.ToInt32(category5Column9Array[i]);
            //        totalcategory5 += categoryColumn9Number;
            //        category5.Column9 = categoryColumn9Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            //        category5.Column10 = category5Column10Array[i];

            //        int categoryColumn11Number = Convert.ToInt32(category5Column11Array[i]);
            //        totalTaxcategory5 += categoryColumn11Number;
            //        category5.Column11 = categoryColumn11Number.ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));
            //        category5.Column12 = category5Column12Array[i];

            //        category5List.Add(category5);
            //    }

            //declarationVat.Categories5 = category5List;
            //declarationVat.Categories5TotalAmount = totalcategory5.ToString("N0",
            //    CultureInfo.CreateSpecificCulture("vi-VN"));
            //declarationVat.Categories5TotalTaxAmount = totalTaxcategory5.ToString("N0",
            //    CultureInfo.CreateSpecificCulture("vi-VN"));

            //totalAmount += totalcategory5;
            //totalTaxAmount += totalTaxcategory5;

            //}


            //declarationVat.TotalAmount = (totalAmount).ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));

            //declarationVat.TotalTaxAmount = (totalTaxAmount).ToString("N0", CultureInfo.CreateSpecificCulture("vi-VN"));



        }
        #endregion

        #region TNDN Tax report

        public ActionResult TndnTaxDeclaration(int year)
        {
            TaxBusiness business = new TaxBusiness();
            TndnTaxDeclarationViewModel result = business.GeTndnTaxDeclarationViewModel(year);


            return View(result);
        }

        [HttpPost]
        public ActionResult ExportXmlTndnTaxDeclaration(FormCollection form)
        {
            #region Create Data
            string taxYear = form["taxYear"];

            //Store Info Not get from form; Get from DB
            BMAEntities db = new BMAEntities();

            StoreInfo storeInfo = db.StoreInfoes.FirstOrDefault();

            string createDay = form["createDay"];
            string createMonth = form["createMonth"];
            string vatAgentOwnerName = form["vatAgentOwnerName"];
            string vatAgentName = form["agentName"];
            string vatAgentTaxCodeString = form["agentTaxCode"];
            string vatAgentTaxCode = "";
            if (vatAgentTaxCodeString != null)
            {
                vatAgentTaxCode = vatAgentTaxCodeString.Replace(",", "");
            }
            string vatAgentNo = form["agentNo"];
            string vatAgentAddress = form["vatAgentAddress"];
            string vatAgentDistrict = form["vatAgentDistrict"];
            string vatAgentProvince = form["vatAgentProvince"];
            string vatAgentPhone = form["vatAgentPhone"];
            string vatAgentFax = form["vatAgentFax"];
            string vatAgentEmail = form["vatAgentEmail"];
            string signName = form["signName"];
            string companyType1 = form["companyType1"];
            string companyType2 = form["companyType2"];
            string companyType3 = form["companyType3"];
            string maxRevenueAreaNo = form["maxRevenueAreaNo"];
            string maxRevenueAreaName = form["maxRevenueAreaName"];


            string value22No = form["value22No"];
            string value22Date = form["value22Date"];

            string valueA1 = form["valueA1"].Replace(".", "");

            string valueB1 = form["valueB1"].Replace(".", "");
            string valueB2 = form["valueB2"].Replace(".", "");
            string valueB3 = form["valueB3"].Replace(".", "");
            string valueB4 = form["valueB4"].Replace(".", "");
            string valueB5 = form["valueB5"].Replace(".", "");
            string valueB6 = form["valueB6"].Replace(".", "");
            string valueB7 = form["valueB7"].Replace(".", "");
            string valueB8 = form["valueB8"].Replace(".", "");
            string valueB9 = form["valueB9"].Replace(".", "");
            string valueB10 = form["valueB10"].Replace(".", "");
            string valueB11 = form["valueB11"].Replace(".", "");
            string valueB12 = form["valueB12"].Replace(".", "");
            string valueB13 = form["valueB13"].Replace(".", "");
            string valueB14 = form["valueB14"].Replace(".", "");

            string valueC1 = form["valueC1"].Replace(".", "");
            string valueC2 = form["valueC2"].Replace(".", "");
            string valueC3 = form["valueC3"].Replace(".", "");
            string valueC3a = form["valueC3a"].Replace(".", "");
            string valueC3b = form["valueC3b"].Replace(".", "");
            string valueC4 = form["valueC4"].Replace(".", "");
            string valueC5 = form["valueC5"].Replace(".", "");
            string valueC6 = form["valueC6"].Replace(".", "");
            string valueC7 = form["valueC7"].Replace(".", "");
            string valueC8 = form["valueC8"].Replace(".", "");
            string valueC9 = form["valueC9"].Replace(".", "");
            string valueC9a = form["valueC9a"].Replace(".", "");
            string valueC10 = form["valueC10"].Replace(".", "");
            string valueC11 = form["valueC11"].Replace(".", "");
            string valueC12 = form["valueC12"].Replace(".", "");
            string valueC13 = form["valueC13"].Replace(".", "");
            string valueC14 = form["valueC14"].Replace(".", "");
            string valueC15 = form["valueC15"].Replace(".", "");
            string valueC16 = form["valueC16"].Replace(".", "");

            string valueD = form["valueD"].Replace(".", "");
            string valueD1 = form["valueD1"].Replace(".", "");
            string valueD2 = form["valueD2"].Replace(".", "");
            string valueD3 = form["valueD3"].Replace(".", "");

            string valueE = form["valueE"].Replace(".", "");
            string valueE1 = form["valueE1"].Replace(".", "");
            string valueE2 = form["valueE2"].Replace(".", "");
            string valueE3 = form["valueE3"].Replace(".", "");

            string valueG = form["valueG"].Replace(".", "");
            string valueG1 = form["valueG1"].Replace(".", "");
            string valueG2 = form["valueG2"].Replace(".", "");
            string valueG3 = form["valueG3"].Replace(".", "");

            string valueH = form["valueH"].Replace(".", "");
            string valueI = form["valueI"].Replace(".", "");

            string valueL1 = form["valueL1"];
            string valueL2Name = form["valueL2Name"];
            string valueL2Code = form["valueL2Code"];
            string valueL3 = form["valueL3"];
            string valueL4 = form["valueL4"].Replace(".", "");
            string valueL5 = form["valueL5"].Replace(".", "");

            string valueM1DayQuantity = form["valueM1DayQuantity"];
            string valueM1FromDate = form["valueM1FromDate"];
            string valueM1ToDate = form["valueM1ToDate"];
            string valueM2 = form["valueM2"].Replace(".", "");
            #endregion

            #region Create XML File

            XElement root = new XElement("HSoThueDTu");

            XElement HSoKhaiThue = new XElement("HSoKhaiThue");
            HSoKhaiThue.Add(new XAttribute("id", "ID_1"));

            XElement TTinChung = new XElement("TTinChung");

            XElement TTinDVu = new XElement("TTinDVu");

            XElement maDVu = new XElement("maDVu", "HTKK");
            TTinDVu.Add(maDVu);
            XElement tenDVu = new XElement("tenDVu", "HTKK");
            TTinDVu.Add(tenDVu);
            XElement pbanDVu = new XElement("pbanDVu", "3.3.4");
            TTinDVu.Add(pbanDVu);

            TTinChung.Add(TTinDVu);

            XElement TTinTKhaiThue = new XElement("TTinTKhaiThue");

            XElement TKhaiThue = new XElement("TKhaiThue");

            XElement maTKhai = new XElement("maTKhai", "03");
            TKhaiThue.Add(maTKhai);

            XElement tenTKhai = new XElement("tenTKhai", "TỜ KHAI QUYẾT TOÁN THUẾ THU NHẬP DOANH NGHIỆP (Mẫu số 03/TNDN)");
            TKhaiThue.Add(tenTKhai);

            XElement moTaBMau = new XElement("moTaBMau", "(Ban hành kèm theo Thông tư số 151/2014/TT-BTC  ngày 10/10/2014 của  Bộ Tài chính)");
            TKhaiThue.Add(moTaBMau);

            XElement pbanTKhaiXML = new XElement("pbanTKhaiXML", "2.0.7");
            TKhaiThue.Add(pbanTKhaiXML);

            XElement loaiTKhai = new XElement("loaiTKhai", "C");
            TKhaiThue.Add(loaiTKhai);

            XElement soLan = new XElement("soLan", "0");
            TKhaiThue.Add(soLan);

            XElement KyKKhaiThue = new XElement("KyKKhaiThue");

            XElement kieuKy = new XElement("kieuKy", "Y");
            KyKKhaiThue.Add(kieuKy);

            XElement kyKKhai = new XElement("kyKKhai", taxYear);
            KyKKhaiThue.Add(kyKKhai);

            XElement kyKKhaiTuNgay = new XElement("kyKKhaiTuNgay", "01/01/" + taxYear);
            KyKKhaiThue.Add(kyKKhaiTuNgay);

            XElement kyKKhaiDenNgay = new XElement("kyKKhaiDenNgay", "31/12/" + taxYear);
            KyKKhaiThue.Add(kyKKhaiDenNgay);

            XElement kyKKhaiTuThang = new XElement("kyKKhaiTuThang");
            KyKKhaiThue.Add(kyKKhaiTuThang);

            XElement kyKKhaiDenThang = new XElement("kyKKhaiDenThang");
            KyKKhaiThue.Add(kyKKhaiDenThang);


            TKhaiThue.Add(KyKKhaiThue);

            XElement maCQTNoiNop = new XElement("maCQTNoiNop");
            TKhaiThue.Add(maCQTNoiNop);

            XElement tenCQTNoiNop = new XElement("tenCQTNoiNop", "Chi cục Thuế Quận 12");
            TKhaiThue.Add(tenCQTNoiNop);

            XElement ngayLapTKhai = new XElement("ngayLapTKhai", taxYear + "-" + createMonth + "-" + createDay);
            TKhaiThue.Add(ngayLapTKhai);

            XElement GiaHan = new XElement("GiaHan");

            XElement maLyDoGiaHan = new XElement("maLyDoGiaHan", valueL2Name);
            GiaHan.Add(maLyDoGiaHan);

            XElement lyDoGiaHan = new XElement("lyDoGiaHan", valueL2Code);
            GiaHan.Add(lyDoGiaHan);

            TKhaiThue.Add(GiaHan);

            XElement nguoiKy = new XElement("nguoiKy", signName);
            TKhaiThue.Add(nguoiKy);

            XElement ngayKy = new XElement("ngayKy", taxYear + "-" + createMonth + "-" + createDay);
            TKhaiThue.Add(ngayKy);

            XElement nganhNgheKD = new XElement("nganhNgheKD");
            TKhaiThue.Add(nganhNgheKD);


            TTinTKhaiThue.Add(TKhaiThue);

            XElement NNT = new XElement("NNT");

            XElement mst = new XElement("mst", storeInfo.TaxCode);
            NNT.Add(mst);

            XElement tenNNT = new XElement("tenNNT", storeInfo.OwnerName);
            NNT.Add(tenNNT);

            XElement dchiNNT = new XElement("dchiNNT", storeInfo.Address);
            NNT.Add(dchiNNT);

            XElement phuongXa = new XElement("phuongXa");
            NNT.Add(phuongXa);

            XElement maHuyenNNT = new XElement("maHuyenNNT");
            NNT.Add(maHuyenNNT);

            XElement tenHuyenNNT = new XElement("tenHuyenNNT", storeInfo.District);
            NNT.Add(tenHuyenNNT);

            XElement maTinhNNT = new XElement("maTinhNNT");
            NNT.Add(maTinhNNT);

            XElement tenTinhNNT = new XElement("tenTinhNNT", storeInfo.Province);
            NNT.Add(tenTinhNNT);

            XElement dthoaiNNT = new XElement("dthoaiNNT", storeInfo.Phonenumber);
            NNT.Add(dthoaiNNT);

            XElement faxNNT = new XElement("faxNNT", storeInfo.Fax);
            NNT.Add(faxNNT);

            XElement emailNNT = new XElement("emailNNT", storeInfo.Email);
            NNT.Add(emailNNT);

            TTinTKhaiThue.Add(NNT);

            XElement DLyThue = new XElement("DLyThue");

            XElement mstDLyThue = new XElement("mstDLyThue", vatAgentTaxCode.Replace(",", ""));
            DLyThue.Add(mstDLyThue);

            XElement tenDLyThue = new XElement("tenDLyThue", vatAgentOwnerName);
            DLyThue.Add(tenDLyThue);

            XElement dchiDLyThue = new XElement("dchiDLyThue", vatAgentAddress);
            DLyThue.Add(dchiDLyThue);

            XElement maHuyenDLyThue = new XElement("maHuyenDLyThue");
            DLyThue.Add(maHuyenDLyThue);

            XElement tenHuyenDLyThue = new XElement("tenHuyenDLyThue", vatAgentDistrict);
            DLyThue.Add(tenHuyenDLyThue);

            XElement maTinhDLyThue = new XElement("maTinhDLyThue");
            DLyThue.Add(maTinhDLyThue);

            XElement tenTinhDLyThue = new XElement("tenTinhDLyThue", vatAgentProvince);
            DLyThue.Add(tenTinhDLyThue);

            XElement dthoaiDLyThue = new XElement("dthoaiDLyThue", vatAgentPhone);
            DLyThue.Add(dthoaiDLyThue);

            XElement faxDLyThue = new XElement("faxDLyThue", vatAgentFax);
            DLyThue.Add(faxDLyThue);

            XElement emailDLyThue = new XElement("emailDLyThue", vatAgentEmail);
            DLyThue.Add(emailDLyThue);

            XElement soHDongDLyThue = new XElement("soHDongDLyThue", value22No);
            DLyThue.Add(soHDongDLyThue);

            XElement ngayKyHDDLyThue = new XElement("ngayKyHDDLyThue", value22Date);
            DLyThue.Add(ngayKyHDDLyThue);

            XElement NVienDLy = new XElement("NVienDLy");

            XElement tenNVienDLyThue = new XElement("tenNVienDLyThue", vatAgentName);
            NVienDLy.Add(tenNVienDLyThue);

            XElement cchiHNghe = new XElement("cchiHNghe", vatAgentNo);
            NVienDLy.Add(cchiHNghe);


            DLyThue.Add(NVienDLy);

            TTinTKhaiThue.Add(DLyThue);

            TTinChung.Add(TTinTKhaiThue);

            HSoKhaiThue.Add(TTinChung);

            XElement CTieuTKhaiChinh = new XElement("CTieuTKhaiChinh");

            XElement tieuMucHachToan = new XElement("tieuMucHachToan", "1052");
            CTieuTKhaiChinh.Add(tieuMucHachToan);

            XElement doanhNghiepCoQuyMoVuaVaNho = new XElement("doanhNghiepCoQuyMoVuaVaNho", (companyType1 != null ? "1" : "0"));
            CTieuTKhaiChinh.Add(doanhNghiepCoQuyMoVuaVaNho);

            XElement doanhNghiepCoCSoHToanPThuoc = new XElement("doanhNghiepCoCSoHToanPThuoc", (companyType2 != null ? "1" : "0"));
            CTieuTKhaiChinh.Add(doanhNghiepCoCSoHToanPThuoc);

            XElement doanhNghiepKeKhaiTTinLienKet = new XElement("doanhNghiepKeKhaiTTinLienKet", (companyType3 != null ? "1" : "0"));
            CTieuTKhaiChinh.Add(doanhNghiepKeKhaiTTinLienKet);

            XElement ct_04_ma = new XElement("ct_04_ma", maxRevenueAreaNo);
            CTieuTKhaiChinh.Add(ct_04_ma);

            XElement ct_04_ten = new XElement("ct_04_ten", maxRevenueAreaName);
            CTieuTKhaiChinh.Add(ct_04_ten);

            XElement ctA1 = new XElement("ctA1", valueA1);
            CTieuTKhaiChinh.Add(ctA1);

            XElement ctB1 = new XElement("ctB1", valueB1);
            CTieuTKhaiChinh.Add(ctB1);
            XElement ctB2 = new XElement("ctB2", valueB2);
            CTieuTKhaiChinh.Add(ctB2);
            XElement ctB3 = new XElement("ctB3", valueB3);
            CTieuTKhaiChinh.Add(ctB3);
            XElement ctB4 = new XElement("ctB4", valueB4);
            CTieuTKhaiChinh.Add(ctB4);
            XElement ctB5 = new XElement("ctB5", valueB5);
            CTieuTKhaiChinh.Add(ctB5);
            XElement ctB6 = new XElement("ctB6", valueB6);
            CTieuTKhaiChinh.Add(ctB6);
            XElement ctB7 = new XElement("ctB7", valueB7);
            CTieuTKhaiChinh.Add(ctB7);
            XElement ctB8 = new XElement("ctB8", valueB8);
            CTieuTKhaiChinh.Add(ctB8);
            XElement ctB9 = new XElement("ctB9", valueB9);
            CTieuTKhaiChinh.Add(ctB9);
            XElement ctB10 = new XElement("ctB10", valueB10);
            CTieuTKhaiChinh.Add(ctB10);
            XElement ctB11 = new XElement("ctB11", valueB11);
            CTieuTKhaiChinh.Add(ctB11);
            XElement ctB12 = new XElement("ctB12", valueB12);
            CTieuTKhaiChinh.Add(ctB12);
            XElement ctB13 = new XElement("ctB13", valueB13);
            CTieuTKhaiChinh.Add(ctB13);
            XElement ctB14 = new XElement("ctB14", valueB14);
            CTieuTKhaiChinh.Add(ctB14);

            XElement ctC1 = new XElement("ctC1", valueC1);
            CTieuTKhaiChinh.Add(ctC1);
            XElement ctC2 = new XElement("ctC2", valueC2);
            CTieuTKhaiChinh.Add(ctC2);
            XElement ctC3 = new XElement("ctC3", valueC3);
            CTieuTKhaiChinh.Add(ctC3);
            XElement ctC3a = new XElement("ctC3a", valueC3a);
            CTieuTKhaiChinh.Add(ctC3a);
            XElement ctC3b = new XElement("ctC3b", valueC3b);
            CTieuTKhaiChinh.Add(ctC3b);
            XElement ctC4 = new XElement("ctC4", valueC4);
            CTieuTKhaiChinh.Add(ctC4);
            XElement ctC5 = new XElement("ctC5", valueC5);
            CTieuTKhaiChinh.Add(ctC5);
            XElement ctC6 = new XElement("ctC6", valueC6);
            CTieuTKhaiChinh.Add(ctC6);
            XElement ctC7 = new XElement("ctC7", valueC7);
            CTieuTKhaiChinh.Add(ctC7);
            XElement ctC8 = new XElement("ctC8", valueC8);
            CTieuTKhaiChinh.Add(ctC8);
            XElement ctC9 = new XElement("ctC9", valueC9);
            CTieuTKhaiChinh.Add(ctC9);
            XElement ctC9a = new XElement("ctC9a", valueC9);
            CTieuTKhaiChinh.Add(ctC9a);
            XElement ctC10 = new XElement("ctC10", valueC10);
            CTieuTKhaiChinh.Add(ctC10);
            XElement ctC11 = new XElement("ctC11", valueC11);
            CTieuTKhaiChinh.Add(ctC11);
            XElement ctC12 = new XElement("ctC12", valueC12);
            CTieuTKhaiChinh.Add(ctC12);
            XElement ctC13 = new XElement("ctC13", valueC13);
            CTieuTKhaiChinh.Add(ctC13);
            XElement ctC14 = new XElement("ctC14", valueC14);
            CTieuTKhaiChinh.Add(ctC14);
            XElement ctC15 = new XElement("ctC15", valueC15);
            CTieuTKhaiChinh.Add(ctC15);
            XElement ctC16 = new XElement("ctC16", valueC16);
            CTieuTKhaiChinh.Add(ctC16);

            XElement ctD = new XElement("ctD", valueD);
            CTieuTKhaiChinh.Add(ctD);
            XElement ctD1 = new XElement("ctD1", valueD1);
            CTieuTKhaiChinh.Add(ctD1);
            XElement ctD2 = new XElement("ctD2", valueD2);
            CTieuTKhaiChinh.Add(ctD2);
            XElement ctD3 = new XElement("ctD3", valueD3);
            CTieuTKhaiChinh.Add(ctD3);

            XElement ctE = new XElement("ctE", valueE);
            CTieuTKhaiChinh.Add(ctE);
            XElement ctE1 = new XElement("ctE1", valueE1);
            CTieuTKhaiChinh.Add(ctE1);
            XElement ctE2 = new XElement("ctE2", valueE2);
            CTieuTKhaiChinh.Add(ctE2);
            XElement ctE3 = new XElement("ctE3", valueE3);
            CTieuTKhaiChinh.Add(ctE3);

            XElement ctG = new XElement("ctG", valueG);
            CTieuTKhaiChinh.Add(ctG);
            XElement ctG1 = new XElement("ctG1", valueG1);
            CTieuTKhaiChinh.Add(ctG1);
            XElement ctG2 = new XElement("ctG2", valueG2);
            CTieuTKhaiChinh.Add(ctG2);
            XElement ctG3 = new XElement("ctG3", valueE3);
            CTieuTKhaiChinh.Add(ctG3);

            XElement ctH = new XElement("ctH", valueG);
            CTieuTKhaiChinh.Add(ctH);
            XElement ctI = new XElement("ctI", valueG);
            CTieuTKhaiChinh.Add(ctI);

            XElement GiaHanNopThue = new XElement("GiaHanNopThue");

            XElement ctL1 = new XElement("ctL1", (valueL1 != null ? "1" : "0"));
            GiaHanNopThue.Add(ctL1);

            XElement ctL2_ma = new XElement("ctL2_ma", valueL2Code);
            GiaHanNopThue.Add(ctL2_ma);

            XElement ctL2_ten = new XElement("ctL2_ten", valueL2Name);
            GiaHanNopThue.Add(ctL2_ten);

            XElement ctL3 = new XElement("ctL3", (valueL3.Trim().Length > 0 ? DateTime.ParseExact(valueL3, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : ""));
            GiaHanNopThue.Add(ctL3);

            XElement ctL4 = new XElement("ctL4", valueL4);
            GiaHanNopThue.Add(ctL4);

            XElement ctL5 = new XElement("ctL5", valueL5);
            GiaHanNopThue.Add(ctL5);

            CTieuTKhaiChinh.Add(GiaHanNopThue);

            XElement TienChamNop = new XElement("TienChamNop");

            XElement CTM1 = new XElement("CTM1");

            XElement soNgay = new XElement("soNgay", valueM1DayQuantity);
            CTM1.Add(soNgay);

            XElement tuNgay = new XElement("tuNgay", (valueM1FromDate.Trim().Length > 0 ? DateTime.ParseExact(valueM1FromDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : ""));
            CTM1.Add(tuNgay);

            XElement denNgay = new XElement("denNgay", (valueM1ToDate.Trim().Length > 0 ? DateTime.ParseExact(valueM1ToDate, "dd/MM/yyyy", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd") : ""));
            CTM1.Add(denNgay);

            TienChamNop.Add(CTM1);

            XElement ctM2 = new XElement("ctM2", valueM2);
            TienChamNop.Add(ctM2);

            CTieuTKhaiChinh.Add(TienChamNop);

            XElement TaiLieu_Guikem = new XElement("TaiLieu_Guikem");
            CTieuTKhaiChinh.Add(TaiLieu_Guikem);

            HSoKhaiThue.Add(CTieuTKhaiChinh);

            root.Add(HSoKhaiThue);

            var xs = new XmlSerializer(root.GetType());
            var stream = new MemoryStream();
            var streamWriter = new StreamWriter(stream, System.Text.Encoding.UTF8);

            xs.Serialize(streamWriter, root);
            stream.Position = 0;
            return File(stream, "application/xml", "TNDN" + taxYear + ".xml");

            #endregion
        }

        #endregion
    }
}