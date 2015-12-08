using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class DeclarationVatForm
    {
        public int Quarter { get; set; }
        public int Year { get; set; }
        public string StoreOwnerName { get; set; }
        public string StoreTaxCode { get; set; }
        public string StoreAddress { get; set; }
        public string StoreDistrict { get; set; }
        public string StoreProvince { get; set; }
        public string StorePhone { get; set; }
        public string StoreFax { get; set; }
        public string StoreEmail { get; set; }
        public string VatAgentOwnerName { get; set; }
        public string VatAgentName { get; set; }
        public string VatAgentNo { get; set; }
        public string VatAgentTaxCode { get; set; }
        public string VatAgentAddress { get; set; }
        public string VatAgentDistrict { get; set; }
        public string VatAgentProvince { get; set; }
        public string VatAgentPhone { get; set; }
        public string VatAgentFax { get; set; }
        public string VatAgentEmail { get; set; }

        public string SignName { get; set; }
        public string CreateLocation { get; set; }
        public int CreateMonth { get; set; }
        public int CreateDay { get; set; }

        public List<DeclarationVatCategory> InputCategories1 { get; set; }
        public int InputCategories1TotalAmount { get; set; }
        public int InputCategories1TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> InputCategories2 { get; set; }
        public int InputCategories2TotalAmount { get; set; }
        public int InputCategories2TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> InputCategories3 { get; set; }
        public int InputCategories3TotalAmount { get; set; }
        public int InputCategories3TotalTaxAmount { get; set; }

        public int InputTotalAmount { get; set; }
        public int InputTotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> OutputCategories1 { get; set; }
        public int OutputCategories1TotalAmount { get; set; }

        public List<DeclarationVatCategory> OutputCategories2 { get; set; }
        public int OutputCategories2TotalAmount { get; set; }

        public List<DeclarationVatCategory> OutputCategories3 { get; set; }
        public int OutputCategories3TotalAmount { get; set; }
        public int OutputCategories3TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> OutputCategories4 { get; set; }
        public int OutputCategories4TotalAmount { get; set; }
        public int OutputCategories4TotalTaxAmount { get; set; }

        public int OutputTotalAmount { get; set; }
        public int HaveTaxOutputAmount { get; set; }
        public int OutputTotalTaxAmount { get; set; }

        public int Value20No { get; set; }
        public DateTime Value20Date { get; set; }
        public bool Value21 { get; set; }
        public int Value22 { get; set; }
        public int Value25 { get; set; }
        public int Value36 { get; set; }
        public int Value37 { get; set; }
        public int Value38 { get; set; }
        public int Value39 { get; set; }
        public int Value40 { get; set; }
        public int Value40A { get; set; }
        public int Value40B { get; set; }
        public int Value41 { get; set; }
        public int Value42 { get; set; }
        public int Value43 { get; set; }

    }

    public class DeclarationVatCategory
    {
        public string Column2 { get; set; }
        public DateTime Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public int Column6 { get; set; }
        public int Column7 { get; set; }
        public string Column8 { get; set; }
    }
}