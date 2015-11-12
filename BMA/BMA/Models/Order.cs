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
        public Order()
        {
            this.OrderItems = new HashSet<OrderItem>();
            this.Orders1 = new HashSet<Order>();
        }
    
        public int OrderId { get; set; }
        public string OrderCode { get; set; }
        public System.DateTime CreateTime { get; set; }
        public System.DateTime PlanDeliveryTime { get; set; }
        public Nullable<System.DateTime> ApproveTime { get; set; }
        public string OrderNote { get; set; }
        public int DepositAmount { get; set; }
        public Nullable<System.DateTime> ConfirmTime { get; set; }
        public Nullable<int> CustomerUserId { get; set; }
        public Nullable<int> StaffApproveUserId { get; set; }
        public bool IsStaffEdit { get; set; }
        public bool CustomerEditingFlag { get; set; }
        public Nullable<int> OutputBillId { get; set; }
        public int OrderStatus { get; set; }
        public Nullable<int> PreviousOrderId { get; set; }
        public Nullable<int> GuestInfoId { get; set; }
        public int ReturnDeposit { get; set; }
        public Nullable<int> CancelUserId { get; set; }
        public Nullable<System.DateTime> CancelTime { get; set; }
        public Nullable<System.DateTime> FinishTime { get; set; }
        public Nullable<System.DateTime> DeliveryTime { get; set; }
        public Nullable<System.DateTime> StartProduceTime { get; set; }
        public int Amount { get; set; }
        public int TaxAmount { get; set; }
        public int DiscountAmount { get; set; }
    
        public virtual GuestInfo GuestInfo { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
        public virtual ICollection<Order> Orders1 { get; set; }
        public virtual Order Order1 { get; set; }
        public virtual OutputBill OutputBill { get; set; }
        public virtual User User { get; set; }
        public virtual User User1 { get; set; }
        public virtual User User2 { get; set; }
    }
}
