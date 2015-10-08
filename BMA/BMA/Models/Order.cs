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
    
    public partial class Order
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order()
        {
            this.GuestInfoes = new HashSet<GuestInfo>();
            this.OrderItems = new HashSet<OrderItem>();
            this.Orders1 = new HashSet<Order>();
        }
    
        public int OrderId { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> DeliveryTime { get; set; }
        public Nullable<System.DateTime> ApproveTime { get; set; }
        public string OrderNote { get; set; }
        public int DepositAmount { get; set; }
        public Nullable<System.DateTime> ConfirmDate { get; set; }
        public string CustomerUserId { get; set; }
        public string StaffApproveUserId { get; set; }
        public bool Flag { get; set; }
        public int OutputBillId { get; set; }
        public string OrderStatus { get; set; }
        public string OrderCode { get; set; }
        public Nullable<int> PreviousOrderId { get; set; }
        public int TotalValue { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<GuestInfo> GuestInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual OutputBill OutputBill { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders1 { get; set; }
        public virtual Order Order1 { get; set; }
    }
}
