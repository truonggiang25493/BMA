﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class BMAEntities : DbContext
    {
        public BMAEntities()
            : base("name=BMAEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<DiscardedInputMaterial> DiscardedInputMaterials { get; set; }
        public virtual DbSet<DiscountByQuantity> DiscountByQuantities { get; set; }
        public virtual DbSet<ExportFrom> ExportFroms { get; set; }
        public virtual DbSet<GuestInfo> GuestInfoes { get; set; }
        public virtual DbSet<InputBill> InputBills { get; set; }
        public virtual DbSet<InputMaterial> InputMaterials { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OtherExpense> OtherExpenses { get; set; }
        public virtual DbSet<OutputBill> OutputBills { get; set; }
        public virtual DbSet<OutputMaterial> OutputMaterials { get; set; }
        public virtual DbSet<Policy> Policies { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductMaterial> ProductMaterials { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Staff> Staffs { get; set; }
        public virtual DbSet<StoreInfo> StoreInfoes { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<TaxRate> TaxRates { get; set; }
        public virtual DbSet<TaxType> TaxTypes { get; set; }
        public virtual DbSet<User> Users { get; set; }
    
        public virtual ObjectResult<sp_GetIncomeMonthly_Result> sp_GetIncomeMonthly(Nullable<int> startMonth, Nullable<int> startYear, Nullable<int> endMonth, Nullable<int> endYear)
        {
            var startMonthParameter = startMonth.HasValue ?
                new ObjectParameter("startMonth", startMonth) :
                new ObjectParameter("startMonth", typeof(int));
    
            var startYearParameter = startYear.HasValue ?
                new ObjectParameter("startYear", startYear) :
                new ObjectParameter("startYear", typeof(int));
    
            var endMonthParameter = endMonth.HasValue ?
                new ObjectParameter("endMonth", endMonth) :
                new ObjectParameter("endMonth", typeof(int));
    
            var endYearParameter = endYear.HasValue ?
                new ObjectParameter("endYear", endYear) :
                new ObjectParameter("endYear", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetIncomeMonthly_Result>("sp_GetIncomeMonthly", startMonthParameter, startYearParameter, endMonthParameter, endYearParameter);
        }
    
        public virtual ObjectResult<sp_GetIncomeWeekly_Result> sp_GetIncomeWeekly(Nullable<System.DateTime> start_Date, Nullable<System.DateTime> end_Date)
        {
            var start_DateParameter = start_Date.HasValue ?
                new ObjectParameter("start_Date", start_Date) :
                new ObjectParameter("start_Date", typeof(System.DateTime));
    
            var end_DateParameter = end_Date.HasValue ?
                new ObjectParameter("end_Date", end_Date) :
                new ObjectParameter("end_Date", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetIncomeWeekly_Result>("sp_GetIncomeWeekly", start_DateParameter, end_DateParameter);
        }
    
        public virtual ObjectResult<sp_GetIncomeYearly_Result> sp_GetIncomeYearly(Nullable<int> startYear, Nullable<int> endYear)
        {
            var startYearParameter = startYear.HasValue ?
                new ObjectParameter("startYear", startYear) :
                new ObjectParameter("startYear", typeof(int));
    
            var endYearParameter = endYear.HasValue ?
                new ObjectParameter("endYear", endYear) :
                new ObjectParameter("endYear", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<sp_GetIncomeYearly_Result>("sp_GetIncomeYearly", startYearParameter, endYearParameter);
        }
    }
}
