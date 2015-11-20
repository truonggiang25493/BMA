using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BMA.Models.ViewModel
{
    public class DeclarationVatForm
    {
        public int Type { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string StoreOwnerName { get; set; }
        public string StoreTaxCode { get; set; }
        public string VatAgentOwnerName { get; set; }
        public string VatAgentName { get; set; }
        public string VatAgentNo { get; set; }
        public string VatAgentTaxCode { get; set; }

        public string SignName { get; set; }
        public string CreateLocation { get; set; }
        public string CreateDay { get; set; }

        public List<DeclarationVatCategory> Categories1 { get; set; }
        public string Categories1TotalAmount { get; set; }
        public string Categories1TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> Categories2 { get; set; }
        public string Categories2TotalAmount { get; set; }
        public string Categories2TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> Categories3 { get; set; }
        public string Categories3TotalAmount { get; set; }
        public string Categories3TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> Categories4 { get; set; }
        public string Categories4TotalAmount { get; set; }
        public string Categories4TotalTaxAmount { get; set; }

        public List<DeclarationVatCategory> Categories5 { get; set; }
        public string Categories5TotalAmount { get; set; }
        public string Categories5TotalTaxAmount { get; set; }

        public string TotalAmount { get; set; }
        public string TotalTaxAmount { get; set; }
    }

    public class DeclarationVatCategory
    {
        public string Column2 { get; set; }
        public string Column3 { get; set; }
        public string Column4 { get; set; }
        public string Column5 { get; set; }
        public string Column6 { get; set; }
        public string Column7 { get; set; }
        public string Column8 { get; set; }
        public string Column9 { get; set; }
        public string Column10 { get; set; }
        public string Column11 { get; set; }
        public string Column12 { get; set; }
    }
}