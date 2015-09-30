//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BMA.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Order
    {
        public Order()
        {
            this.GuestInfoes = new HashSet<GuestInfo>();
            this.OrderItems = new HashSet<OrderItem>();
        }
    
        public int OrderId { get; set; }
        public System.DateTime CreateTime { get; set; }
        public Nullable<System.DateTime> DeliveryTime { get; set; }
        public Nullable<System.DateTime> ApproveTime { get; set; }
        public string OrderNote { get; set; }
        public int DepositAmount { get; set; }
        public string CustomerUserId { get; set; }
        public string StaffApproveUserId { get; set; }
        public int OutputBillId { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual AspNetUser AspNetUser1 { get; set; }
        public virtual ICollection<GuestInfo> GuestInfoes { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual OutputBill OutputBill { get; set; }
    }
}