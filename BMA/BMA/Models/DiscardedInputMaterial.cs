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
    
    public partial class DiscardedInputMaterial
    {
        public int DiscardId { get; set; }
        public int InputMaterialId { get; set; }
        public int DiscardQuantity { get; set; }
        public System.DateTime DiscardDate { get; set; }
        public string DiscardNote { get; set; }
    
        public virtual InputMaterial InputMaterial { get; set; }
    }
}
