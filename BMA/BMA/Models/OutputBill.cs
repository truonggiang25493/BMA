//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMA.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class OutputBill
    {
        public int OutputBillId { get; set; }
        public string OutputBillCode { get; set; }
        public int OutputBillAmount { get; set; }
        public string OutputBillRawImage { get; set; }
        public int OutputBillTaxAmount { get; set; }
        public System.DateTime ExportDate { get; set; }
        public int OrderId { get; set; }
    
        public virtual Order Order { get; set; }
    }
}
