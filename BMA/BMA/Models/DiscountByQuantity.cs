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
    
    public partial class DiscountByQuantity
    {
        public int Id { get; set; }
        public int QuantityFrom { get; set; }
        public int QuantityTo { get; set; }
        public int DiscountValue { get; set; }
        public Nullable<int> ReachNumber { get; set; }
        public bool beUsing { get; set; }
    }
}
