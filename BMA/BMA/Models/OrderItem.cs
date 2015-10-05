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
    
    public partial class OrderItem
    {
        public OrderItem()
        {
            this.InputMaterials = new HashSet<InputMaterial>();
        }
    
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public int Quantity { get; set; }
        public double RealPrice { get; set; }
        public double Amount { get; set; }
        public double TaxAmount { get; set; }
        public int TaxRateId { get; set; }
    
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public virtual TaxRate TaxRate { get; set; }
        public virtual ICollection<InputMaterial> InputMaterials { get; set; }
    }
}