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
    
    public partial class Product
    {
        public Product()
        {
            this.OrderItem = new HashSet<OrderItem>();
            this.Recipe = new HashSet<Recipe>();
        }
    
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public double ProductWeight { get; set; }
        public string Unit { get; set; }
        public string Descriptions { get; set; }
        public string Note { get; set; }
        public string ProductImage { get; set; }
        public int ProductStandardPrice { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public bool IsActive { get; set; }
        public string ProductCode { get; set; }
    
        public virtual Category Category { get; set; }
        public virtual ICollection<OrderItem> OrderItem { get; set; }
        public virtual ICollection<Recipe> Recipe { get; set; }
    }
}
