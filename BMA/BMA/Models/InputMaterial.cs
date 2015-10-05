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
    
    public partial class InputMaterial
    {
        public InputMaterial()
        {
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public int InputMaterialId { get; set; }
        public int Quantiry { get; set; }
        public double Price { get; set; }
        public System.DateTime InputMaterialImportDate { get; set; }
        public System.DateTime InputMaterialExpiryDate { get; set; }
        public string InputMaterialNote { get; set; }
        public int InputBillId { get; set; }
    
        public virtual InputBill InputBill { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}