using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Business
{
    public class ReportBusiness
    {
        private BMAEntities db;

        public ReportBusiness()
        {
            db = new BMAEntities();
        }

        public List<sp_GetIncomeWeekly_Result> GetIncomeWeeklyStore(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetIncomeWeekly(startDate, endDate).ToList();
        }

        public List<sp_GetIncomeMonthly_Result> GetIncomeMonthlyStore(int startMonth, int startYear, int endMonth, int endYear)
        {
            return db.sp_GetIncomeMonthly(startMonth, startYear, endMonth, endYear).ToList();
        }

        public List<sp_GetIncomeYearly_Result> GetIncomeYearlyStore(int startYear, int endYear)
        {
            return db.sp_GetIncomeYearly(startYear, endYear).ToList();
        }

        public ReportIncomeViewModel ReviewIncomeWeeklyDetail(DateTime startDate, DateTime endDate)
        {
            ReportIncomeViewModel result = new ReportIncomeViewModel
            {
                StartDate = startDate,
                EndDate = endDate
            };

            #region Revenue
            #region Get completed order list

            DateTime tempEndDate = endDate.AddDays(1);
            List<Order> completeOrderList =
                db.Orders.Where(m => m.OrderStatus == 5 && m.FinishTime >= startDate && m.FinishTime < tempEndDate)
                    .ToList();

            int completeValue = 0;
            foreach (Order order in completeOrderList)
            {
                completeValue += (order.Amount - order.DiscountAmount);
            }

            result.CompletedOrderValue = completeValue;
            result.CompletedOrderList = completeOrderList;
            #endregion
            #region Get cancel order list
            List<Order> canceledOrderList = db.Orders.Where(m => m.OrderStatus == 6 && m.CancelTime >= startDate && m.CancelTime < tempEndDate)
                    .ToList();

            int canceledValue = 0;
            foreach (Order order in canceledOrderList)
            {
                canceledValue += (order.DepositAmount - order.ReturnDeposit);
            }
            result.CanceledOrderValue = canceledValue;
            result.CanceledOrderList = canceledOrderList;

            result.RevenueAmount = completeValue + canceledValue;
            #endregion
            #endregion

            #region MaterialExpense
            int materialExpenseValue = 0;

            #region CompletedOrderList
            List<ReportProductMaterial> productMaterialList = new List<ReportProductMaterial>();

            foreach (Order order in completeOrderList)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                    {
                        if (productMaterialList.Count == 0)
                        {
                            ReportProductMaterial productMaterial = new ReportProductMaterial();
                            productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                            productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                            productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                            int productMaterialValue = 0;
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {
                                double price =
                                    db.InputMaterials.FirstOrDefault(
                                        m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                             m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                            }
                            productMaterial.ProductMaterialAmount = productMaterialValue;

                            materialExpenseValue += productMaterialValue;

                            productMaterialList.Add(productMaterial);
                        }
                        else
                        {
                            bool check = true;
                            foreach (ReportProductMaterial reportProductMaterial in productMaterialList)
                            {

                                if (
                                    reportProductMaterial.ProductMaterialName.Equals(
                                        outputMaterial.ProductMaterial.ProductMaterialName))
                                {
                                    check = false;

                                    reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;
                                    int productMaterialValue = 0;
                                    foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                    {
                                        double price =
                                            db.InputMaterials.FirstOrDefault(
                                                m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                     m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                        productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                    }
                                    reportProductMaterial.ProductMaterialAmount += productMaterialValue;

                                    materialExpenseValue += productMaterialValue;
                                }
                            }
                            if (check)
                            {
                                ReportProductMaterial productMaterial = new ReportProductMaterial();
                                productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                                productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                                productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                                int productMaterialValue = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    double price =
                                        db.InputMaterials.FirstOrDefault(
                                            m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                 m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                    productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                }
                                productMaterial.ProductMaterialAmount = productMaterialValue;

                                materialExpenseValue += productMaterialValue;

                                productMaterialList.Add(productMaterial);
                            }
                        }
                    }
                }
            }
            #endregion

            #region CanceledOrderList

            foreach (Order order in canceledOrderList)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                    {
                        bool check = true;
                        foreach (ReportProductMaterial reportProductMaterial in productMaterialList)
                        {

                            if (
                                reportProductMaterial.ProductMaterialName.Equals(
                                    outputMaterial.ProductMaterial.ProductMaterialName))
                            {
                                check = false;

                                reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;
                                int productMaterialValue = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    double price =
                                        db.InputMaterials.FirstOrDefault(
                                            m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                 m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                    productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                }
                                reportProductMaterial.ProductMaterialAmount += productMaterialValue;

                                materialExpenseValue += productMaterialValue;
                            }
                        }
                        if (check)
                        {
                            ReportProductMaterial productMaterial = new ReportProductMaterial();
                            productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                            productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                            productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                            int productMaterialValue = 0;
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {
                                double price =
                                    db.InputMaterials.FirstOrDefault(
                                        m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                             m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                            }
                            productMaterial.ProductMaterialAmount = productMaterialValue;

                            materialExpenseValue += productMaterialValue;

                            productMaterialList.Add(productMaterial);
                        }
                    }
                }
            }

            #endregion

            result.ProductMaterialList = productMaterialList;
            result.MaterialExpense = materialExpenseValue;

            #endregion

            #region Income
            result.Income = result.RevenueAmount - result.MaterialExpense;
            #endregion

            return result;
        }

        public ReportIncomeViewModel ReviewIncomeMonthlyDetail(int month, int year)
        {
            ReportIncomeViewModel result = new ReportIncomeViewModel
            {
                Month = month,
                Year = year
            };

            #region Revenue
            #region Get completed order list

            DateTime startDate = new DateTime(year, month, 1);
            DateTime endDate = startDate.AddMonths(1);

            List<Order> completeOrderList =
                db.Orders.Where(m => m.OrderStatus == 5 && m.FinishTime >= startDate && m.FinishTime < endDate)
                    .ToList();

            int completeValue = 0;
            foreach (Order order in completeOrderList)
            {
                completeValue += (order.Amount - order.DiscountAmount);
            }

            result.CompletedOrderValue = completeValue;
            result.CompletedOrderList = completeOrderList;
            #endregion
            #region Get cancel order list
            List<Order> canceledOrderList = db.Orders.Where(m => m.OrderStatus == 6 && m.CancelTime >= startDate && m.CancelTime < endDate)
                    .ToList();

            int canceledValue = 0;
            foreach (Order order in canceledOrderList)
            {
                canceledValue += (order.DepositAmount - order.ReturnDeposit);
            }
            result.CanceledOrderValue = canceledValue;
            result.CanceledOrderList = canceledOrderList;

            result.RevenueAmount = completeValue + canceledValue;
            #endregion
            #endregion

            #region MaterialExpense
            int materialExpenseValue = 0;

            #region CompletedOrderList
            List<ReportProductMaterial> productMaterialList = new List<ReportProductMaterial>();

            foreach (Order order in completeOrderList)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                    {
                        if (productMaterialList.Count == 0)
                        {
                            ReportProductMaterial productMaterial = new ReportProductMaterial();
                            productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                            productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                            productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                            int productMaterialValue = 0;
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {
                                double price =
                                    db.InputMaterials.FirstOrDefault(
                                        m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                             m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                            }
                            productMaterial.ProductMaterialAmount = productMaterialValue;

                            materialExpenseValue += productMaterialValue;

                            productMaterialList.Add(productMaterial);
                        }
                        else
                        {
                            bool check = true;
                            foreach (ReportProductMaterial reportProductMaterial in productMaterialList)
                            {

                                if (
                                    reportProductMaterial.ProductMaterialName.Equals(
                                        outputMaterial.ProductMaterial.ProductMaterialName))
                                {
                                    check = false;

                                    reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;
                                    int productMaterialValue = 0;
                                    foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                    {
                                        double price =
                                            db.InputMaterials.FirstOrDefault(
                                                m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                     m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                        productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                    }
                                    reportProductMaterial.ProductMaterialAmount += productMaterialValue;

                                    materialExpenseValue += productMaterialValue;
                                }
                            }
                            if (check)
                            {
                                ReportProductMaterial productMaterial = new ReportProductMaterial();
                                productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                                productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                                productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                                int productMaterialValue = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    double price =
                                        db.InputMaterials.FirstOrDefault(
                                            m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                 m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                    productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                }
                                productMaterial.ProductMaterialAmount = productMaterialValue;

                                materialExpenseValue += productMaterialValue;

                                productMaterialList.Add(productMaterial);
                            }
                        }
                    }
                }
            }
            #endregion

            #region CanceledOrderList

            foreach (Order order in canceledOrderList)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                    {
                        bool check = true;
                        foreach (ReportProductMaterial reportProductMaterial in productMaterialList)
                        {

                            if (
                                reportProductMaterial.ProductMaterialName.Equals(
                                    outputMaterial.ProductMaterial.ProductMaterialName))
                            {
                                check = false;

                                reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;
                                int productMaterialValue = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    double price =
                                        db.InputMaterials.FirstOrDefault(
                                            m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                 m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                    productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                }
                                reportProductMaterial.ProductMaterialAmount += productMaterialValue;

                                materialExpenseValue += productMaterialValue;
                            }
                        }
                        if (check)
                        {
                            ReportProductMaterial productMaterial = new ReportProductMaterial();
                            productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                            productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                            productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                            int productMaterialValue = 0;
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {
                                double price =
                                    db.InputMaterials.FirstOrDefault(
                                        m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                             m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                            }
                            productMaterial.ProductMaterialAmount = productMaterialValue;

                            materialExpenseValue += productMaterialValue;

                            productMaterialList.Add(productMaterial);
                        }
                    }
                }
            }

            #endregion

            result.ProductMaterialList = productMaterialList;
            result.MaterialExpense = materialExpenseValue;

            #endregion

            #region OtherExpense

            int otherExpenseValue = 0;
            List<OtherExpense> otherExpenses =
                db.OtherExpenses.Where(m => m.OtherExpenseMonthTime == month && m.OtherExpenseYearTime == year).ToList();
            foreach (OtherExpense otherExpense in otherExpenses)
            {
                otherExpenseValue += otherExpense.OtherExpenseAmount;
            }


            result.OtherExpense = otherExpenseValue;
            result.OtherExpenseList = otherExpenses;
            #endregion

            #region Income
            result.Income = result.RevenueAmount - result.MaterialExpense - result.OtherExpense;
            #endregion

            return result;
        }
        public ReportIncomeViewModel ReviewIncomeYearlyDetail(int year)
        {
            ReportIncomeViewModel result = new ReportIncomeViewModel
            {
                Year = year
            };

            #region Revenue
            #region Get completed order list

            DateTime startDate = new DateTime(year, 1, 1);
            DateTime endDate = startDate.AddYears(1);

            List<Order> completeOrderList =
                db.Orders.Where(m => m.OrderStatus == 5 && m.FinishTime >= startDate && m.FinishTime < endDate)
                    .ToList();

            int completeValue = 0;
            foreach (Order order in completeOrderList)
            {
                completeValue += (order.Amount - order.DiscountAmount);
            }

            result.CompletedOrderValue = completeValue;
            result.CompletedOrderList = completeOrderList;
            #endregion
            #region Get cancel order list
            List<Order> canceledOrderList = db.Orders.Where(m => m.OrderStatus == 6 && m.CancelTime >= startDate && m.CancelTime < endDate)
                    .ToList();

            int canceledValue = 0;
            foreach (Order order in canceledOrderList)
            {
                canceledValue += (order.DepositAmount - order.ReturnDeposit);
            }
            result.CanceledOrderValue = canceledValue;
            result.CanceledOrderList = canceledOrderList;

            result.RevenueAmount = completeValue + canceledValue;
            #endregion
            #endregion

            #region MaterialExpense
            int materialExpenseValue = 0;

            #region CompletedOrderList
            List<ReportProductMaterial> productMaterialList = new List<ReportProductMaterial>();

            foreach (Order order in completeOrderList)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                    {
                        if (productMaterialList.Count == 0)
                        {
                            ReportProductMaterial productMaterial = new ReportProductMaterial();
                            productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                            productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                            productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                            int productMaterialValue = 0;
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {
                                double price =
                                    db.InputMaterials.FirstOrDefault(
                                        m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                             m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                            }
                            productMaterial.ProductMaterialAmount = productMaterialValue;

                            materialExpenseValue += productMaterialValue;

                            productMaterialList.Add(productMaterial);
                        }
                        else
                        {
                            bool check = true;
                            foreach (ReportProductMaterial reportProductMaterial in productMaterialList)
                            {

                                if (
                                    reportProductMaterial.ProductMaterialName.Equals(
                                        outputMaterial.ProductMaterial.ProductMaterialName))
                                {
                                    check = false;

                                    reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;
                                    int productMaterialValue = 0;
                                    foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                    {
                                        double price =
                                            db.InputMaterials.FirstOrDefault(
                                                m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                     m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                        productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                    }
                                    reportProductMaterial.ProductMaterialAmount += productMaterialValue;

                                    materialExpenseValue += productMaterialValue;
                                }
                            }
                            if (check)
                            {
                                ReportProductMaterial productMaterial = new ReportProductMaterial();
                                productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                                productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                                productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                                int productMaterialValue = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    double price =
                                        db.InputMaterials.FirstOrDefault(
                                            m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                 m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                    productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                }
                                productMaterial.ProductMaterialAmount = productMaterialValue;

                                materialExpenseValue += productMaterialValue;

                                productMaterialList.Add(productMaterial);
                            }
                        }
                    }
                }
            }
            #endregion

            #region CanceledOrderList

            foreach (Order order in canceledOrderList)
            {
                foreach (OrderItem orderItem in order.OrderItems)
                {
                    foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                    {
                        bool check = true;
                        foreach (ReportProductMaterial reportProductMaterial in productMaterialList)
                        {

                            if (
                                reportProductMaterial.ProductMaterialName.Equals(
                                    outputMaterial.ProductMaterial.ProductMaterialName))
                            {
                                check = false;

                                reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;
                                int productMaterialValue = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    double price =
                                        db.InputMaterials.FirstOrDefault(
                                            m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                                 m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                    productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                                }
                                reportProductMaterial.ProductMaterialAmount += productMaterialValue;

                                materialExpenseValue += productMaterialValue;
                            }
                        }
                        if (check)
                        {
                            ReportProductMaterial productMaterial = new ReportProductMaterial();
                            productMaterial.ProductMaterialName = outputMaterial.ProductMaterial.ProductMaterialName;
                            productMaterial.ProductMaterialUnit = outputMaterial.ProductMaterial.ProductMaterialUnit;
                            productMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;
                            int productMaterialValue = 0;
                            foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                            {
                                double price =
                                    db.InputMaterials.FirstOrDefault(
                                        m => m.ProductMaterialId == outputMaterial.ProductMaterialId &&
                                             m.InputBillId == exportFrom.InputbillId).InputMaterialPrice;
                                productMaterialValue += (int)(price * exportFrom.ExportFromQuantity);
                            }
                            productMaterial.ProductMaterialAmount = productMaterialValue;

                            materialExpenseValue += productMaterialValue;

                            productMaterialList.Add(productMaterial);
                        }
                    }
                }
            }

            #endregion

            result.ProductMaterialList = productMaterialList;
            result.MaterialExpense = materialExpenseValue;

            #endregion

            #region OtherExpense

            int otherExpenseValue = 0;
            List<OtherExpense> otherExpenses =
                db.OtherExpenses.Where(m => m.OtherExpenseYearTime == year).ToList();
            foreach (OtherExpense otherExpense in otherExpenses)
            {
                otherExpenseValue += otherExpense.OtherExpenseAmount;
            }


            result.OtherExpense = otherExpenseValue;
            result.OtherExpenseList = otherExpenses;
            #endregion

            #region Income
            result.Income = result.RevenueAmount - result.MaterialExpense - result.OtherExpense;
            #endregion

            return result;
        }

        public List<sp_GetTop10CustomerRevenueWeekly_Result> GetTop10CustomerRevenueWeekly(DateTime start, DateTime end)
        {
            return db.sp_GetTop10CustomerRevenueWeekly(start, end).ToList();
        }

        public List<sp_GetTop10CustomerRevenueMonthly_Result> GetTop10CustomerRevenueMonthly(DateTime start,
            DateTime end)
        {
            return db.sp_GetTop10CustomerRevenueMonthly(start, end).ToList();
        }

        public List<sp_GetTop10CustomerRevenueYearly_Result> GetTop10CustomerRevenueYearly(DateTime start, DateTime end)
        {
            return db.sp_GetTop10CustomerRevenueYearly(start, end).ToList();
        }

        public List<sp_GetTop10IncomeOfProductWeekly_Result> GetTop10ProductIncomeWeekly(DateTime start, DateTime end)
        {
            return db.sp_GetTop10IncomeOfProductWeekly(start, end).ToList();
        }

        public List<sp_GetTop10IncomeOfProductMonthly_Result> GetTop10ProductIncomeMonthly(DateTime start, DateTime end)
        {
            return db.sp_GetTop10IncomeOfProductMonthly(start, end).ToList();
        }

        public List<sp_GetTop10IncomeOfProductYearly_Result> GetTop10ProductIncomeYearly(DateTime start, DateTime end)
        {
            return db.sp_GetTop10IncomeOfProductYearly(start, end).ToList();
        }
    }
}