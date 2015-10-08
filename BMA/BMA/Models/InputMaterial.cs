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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InputMaterial()
        {
            this.MaterialInOrderItems = new HashSet<MaterialInOrderItem>();
        }
    
        public int InputMaterialId { get; set; }
        public int ImportQuantity { get; set; }
        public int RemainQuantity { get; set; }
        public double InputMaterialPrice { get; set; }
        public System.DateTime InputMaterialImportDate { get; set; }
        public System.DateTime InputMaterialExpiryDate { get; set; }
        public string InputMaterialNote { get; set; }
        public int InputBillId { get; set; }
        public Nullable<int> ProductMaterialId { get; set; }
        public bool IsActive { get; set; }
    
        public virtual InputBill InputBill { get; set; }
        public virtual ProductMaterial ProductMaterial { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MaterialInOrderItem> MaterialInOrderItems { get; set; }
    }
}
