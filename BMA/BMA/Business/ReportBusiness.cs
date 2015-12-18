using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Common;
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

        public CustomerRevenueReport GetCustomerRevenueReport(int customerUserId, DateTime? startDate, DateTime? endDate,
            int? startMonth, int? startYear, int? endMonth, int? endYear)
        {
            CustomerRevenueReport result = new CustomerRevenueReport();

            #region Get customer info

            User customer = db.Users.FirstOrDefault(m => m.UserId == customerUserId);

            if (customer != null)
            {
                result.Username = customer.Username;
                result.Fullname = customer.Fullname;
            }

            #endregion

            if (startDate != null && endDate != null)
            {
                result.StartDate = startDate.Value;
                result.EndDate = endDate.Value;

                #region Create List revenue report period
                DateTime firstWeekEndDate = startDate.Value.LastDayOfWeek();
                DateTime lastWeekStartDate = endDate.Value.FirstDayOfWeek();

                List<CustomerRevenueReportPeriod> revenueReportPeriods = new List<CustomerRevenueReportPeriod>();

                CustomerRevenueReportPeriod firstPeriod = new CustomerRevenueReportPeriod
                {
                    StartDate = startDate.Value,
                    EndDate = firstWeekEndDate
                };

                revenueReportPeriods.Add(firstPeriod);
                for (DateTime temp = firstWeekEndDate.AddDays(1); temp < lastWeekStartDate; temp = temp.AddDays(7))
                {
                    CustomerRevenueReportPeriod period = new CustomerRevenueReportPeriod
                    {
                        StartDate = temp,
                        EndDate = temp.LastDayOfWeek()
                    };

                    revenueReportPeriods.Add(period);
                }

                CustomerRevenueReportPeriod lastPeriod = new CustomerRevenueReportPeriod
                {
                    StartDate = lastWeekStartDate,
                    EndDate = endDate.Value
                };

                revenueReportPeriods.Add(lastPeriod);
                #endregion

                #region Get each revenue report

                int totalRevenue = 0;

                foreach (CustomerRevenueReportPeriod reportPeriod in revenueReportPeriods)
                {
                    int revenue = 0;
                    #region Get completed order list

                    int completedOrderValue = 0;

                    DateTime tempEndDate = reportPeriod.EndDate.AddDays(1);

                    List<Order> completedOrderList =
                        db.Orders.Where(
                            m =>
                                m.CustomerUserId == customerUserId && m.OrderStatus == 5 && m.FinishTime >= reportPeriod.StartDate &&
                                m.FinishTime < tempEndDate).ToList();
                    foreach (Order order in completedOrderList)
                    {
                        completedOrderValue += (order.Amount - order.DiscountAmount);
                    }

                    reportPeriod.CompletedOrderValue = completedOrderValue;
                    reportPeriod.CompletedOrderList = completedOrderList;

                    revenue += completedOrderValue;

                    #endregion

                    #region Get Canceled order list

                    int canceledOrderValue = 0;

                    List<Order> canceledOrderList =
                        db.Orders.Where(m => m.OrderStatus == 6 && m.CustomerUserId == customerUserId && m.CancelTime >= reportPeriod.StartDate && m.CancelTime < tempEndDate).ToList();

                    foreach (Order order in canceledOrderList)
                    {
                        canceledOrderValue += (order.DepositAmount - order.ReturnDeposit);
                    }

                    reportPeriod.CanceledOrderValue = canceledOrderValue;
                    reportPeriod.CanceledOrderList = canceledOrderList;

                    revenue += canceledOrderValue;
                    reportPeriod.RevenueAmount = revenue;

                    totalRevenue += revenue;

                    #endregion

                }

                #endregion

                #region Finish result

                result.CustomerRevenueReportPeriodList = revenueReportPeriods;
                result.TotalRevenue = totalRevenue;

                #endregion

            }
            else if (startMonth != null && endMonth != null && startYear != null && endYear != null)
            {
                int totalRevenue = 0;
                // Bug Now
                result.StartMonth = startMonth.Value;
                result.StartYear = startYear.Value;
                result.EndMonth = endMonth.Value;
                result.EndYear = endYear.Value;

                #region Create List revenue report period

                DateTime firstMonthStartDate = new DateTime(startYear.Value, startMonth.Value, 1);
                DateTime lastMonthStartDate = new DateTime(endYear.Value, endMonth.Value, 1);
                List<CustomerRevenueReportPeriod> revenueReportPeriods = new List<CustomerRevenueReportPeriod>();

                for (DateTime tempDate = firstMonthStartDate;
                    tempDate <= lastMonthStartDate;
                    tempDate = tempDate.AddMonths(1))
                {
                    CustomerRevenueReportPeriod reportPeriod = new CustomerRevenueReportPeriod
                    {
                        Month = tempDate.Month,
                        Year = tempDate.Year
                    };

                    int revenue = 0;
                    #region Get completed order list

                    int completedOrderValue = 0;

                    DateTime tempEndDate = tempDate.AddMonths(1);

                    List<Order> completedOrderList =
                        db.Orders.Where(
                            m => m.OrderStatus == 5 &&
                                m.CustomerUserId == customerUserId && m.FinishTime >= tempDate &&
                                m.FinishTime < tempEndDate).ToList();
                    foreach (Order order in completedOrderList)
                    {
                        completedOrderValue += (order.Amount - order.DiscountAmount);
                    }

                    reportPeriod.CompletedOrderValue = completedOrderValue;
                    reportPeriod.CompletedOrderList = completedOrderList;

                    revenue += completedOrderValue;

                    #endregion

                    #region Get Canceled order list

                    int canceledOrderValue = 0;

                    List<Order> canceledOrderList =
                        db.Orders.Where(
                            m => m.OrderStatus == 6 &&
                                m.CustomerUserId == customerUserId && m.CancelTime >= tempDate &&
                                m.CancelTime < tempEndDate).ToList();

                    foreach (Order order in canceledOrderList)
                    {
                        canceledOrderValue += (order.DepositAmount - order.ReturnDeposit);
                    }

                    reportPeriod.CanceledOrderValue = canceledOrderValue;
                    reportPeriod.CanceledOrderList = canceledOrderList;

                    revenue += canceledOrderValue;
                    reportPeriod.RevenueAmount = revenue;

                    totalRevenue += revenue;
                    #endregion

                    revenueReportPeriods.Add(reportPeriod);
                }

                #endregion

                #region Finish result

                result.CustomerRevenueReportPeriodList = revenueReportPeriods;
                result.TotalRevenue = totalRevenue;

                #endregion

            }
            else if (startMonth == null && endMonth == null && startYear != null && endYear != null)
            {

                result.StartYear = startYear.Value;
                result.EndYear = endYear.Value;

                int totalRevenue = 0;

                #region Create List revenue report period

                DateTime firstMonthStartDate = new DateTime(startYear.Value, 1, 1);
                DateTime lastMonthStartDate = new DateTime(endYear.Value, 1, 1);
                List<CustomerRevenueReportPeriod> revenueReportPeriods = new List<CustomerRevenueReportPeriod>();

                for (DateTime tempDate = firstMonthStartDate;
                    tempDate <= lastMonthStartDate;
                    tempDate = tempDate.AddYears(1))
                {
                    CustomerRevenueReportPeriod reportPeriod = new CustomerRevenueReportPeriod
                    {
                        Year = tempDate.Year
                    };
                    int revenue = 0;
                    #region Get completed order list

                    int completedOrderValue = 0;

                    DateTime tempEndDate = tempDate.AddYears(1);

                    List<Order> completedOrderList =
                        db.Orders.Where(
                            m => m.OrderStatus == 5 &&
                                m.CustomerUserId == customerUserId && m.FinishTime >= tempDate &&
                                m.FinishTime < tempEndDate).ToList();
                    foreach (Order order in completedOrderList)
                    {
                        completedOrderValue += (order.Amount - order.DiscountAmount);
                    }

                    reportPeriod.CompletedOrderValue = completedOrderValue;
                    reportPeriod.CompletedOrderList = completedOrderList;

                    revenue += completedOrderValue;

                    #endregion

                    #region Get Canceled order list

                    int canceledOrderValue = 0;

                    List<Order> canceledOrderList =
                        db.Orders.Where(
                            m => m.OrderStatus == 6 &&
                                m.CustomerUserId == customerUserId && m.CancelTime >= tempDate &&
                                m.CancelTime < tempEndDate).ToList();

                    foreach (Order order in canceledOrderList)
                    {
                        canceledOrderValue += (order.DepositAmount - order.ReturnDeposit);
                    }

                    reportPeriod.CanceledOrderValue = canceledOrderValue;
                    reportPeriod.CanceledOrderList = canceledOrderList;

                    revenue += canceledOrderValue;

                    #endregion

                    reportPeriod.RevenueAmount = revenue;
                    revenueReportPeriods.Add(reportPeriod);

                    totalRevenue += revenue;
                }

                #endregion

                #region Finish result

                result.CustomerRevenueReportPeriodList = revenueReportPeriods;
                result.TotalRevenue = totalRevenue;

                #endregion

            }

            return result;
        }
        public ReportProductIncomeViewModel GetReportProductIncomeViewModel(int productId, DateTime? startDate, DateTime? endDate,
            int? startMonth, int? startYear, int? endMonth, int? endYear)
        {
            ReportProductIncomeViewModel result = new ReportProductIncomeViewModel();

            #region Get Product info

            Product product = db.Products.FirstOrDefault(m => m.ProductId == productId);
            if (product != null)
            {
                result.ProductCode = product.ProductCode;
                result.ProductName = product.ProductName;
            }

            #endregion



            if (startDate != null && endDate != null)
            {
                #region Get report product weekly
                result.StartDate = startDate.Value;
                result.EndDate = endDate.Value;

                int totalIncome = 0;

                #region Create List income report period
                DateTime firstWeekEndDate = startDate.Value.LastDayOfWeek();
                DateTime lastWeekStartDate = endDate.Value.FirstDayOfWeek();

                List<ReportProductIncomePeriod> incomeReportPeriods = new List<ReportProductIncomePeriod>();

                ReportProductIncomePeriod firstPeriod = new ReportProductIncomePeriod
                {
                    StartDate = startDate.Value,
                    EndDate = firstWeekEndDate
                };

                incomeReportPeriods.Add(firstPeriod);
                for (DateTime temp = firstWeekEndDate.AddDays(1); temp < lastWeekStartDate; temp = temp.AddDays(7))
                {
                    ReportProductIncomePeriod period = new ReportProductIncomePeriod
                    {
                        StartDate = temp,
                        EndDate = temp.LastDayOfWeek()
                    };

                    incomeReportPeriods.Add(period);
                }

                ReportProductIncomePeriod lastPeriod = new ReportProductIncomePeriod
                {
                    StartDate = lastWeekStartDate,
                    EndDate = endDate.Value
                };

                incomeReportPeriods.Add(lastPeriod);
                #endregion

                #region Get each income report

                foreach (ReportProductIncomePeriod incomePeriod in incomeReportPeriods)
                {
                    DateTime tempEndDate = incomePeriod.EndDate.AddDays(1);

                    int revenue = 0;
                    int expense = 0;

                    List<OrderItem> orderItems =
                        db.OrderItems.Where(
                            m =>
                                m.ProductId == productId && m.Order.OrderStatus == 5 && m.Order.FinishTime >= incomePeriod.StartDate &&
                                m.Order.FinishTime < tempEndDate).ToList();
                    List<ReportProductOrder> reportProductOrderList = new List<ReportProductOrder>();
                    List<ReportProductMaterial> reportProductMaterialList = new List<ReportProductMaterial>();
                    foreach (OrderItem orderItem in orderItems)
                    {
                        #region Create ReportProductOrder

                        ReportProductOrder reportProductOrder = new ReportProductOrder();
                        reportProductOrder.OrderCode = orderItem.Order.OrderCode;
                        reportProductOrder.Quantity = orderItem.Quantity;
                        reportProductOrder.Amount = orderItem.Amount;
                        reportProductOrder.DiscountAmount = (orderItem.Amount * (orderItem.Order.DiscountAmount * 100 / orderItem.Order.Amount)) / 100;
                        reportProductOrder.Revenue = reportProductOrder.Amount - reportProductOrder.DiscountAmount;
                        reportProductOrderList.Add(reportProductOrder);

                        revenue += reportProductOrder.Revenue;

                        #endregion

                        #region Create ReportProductMaterial

                        if (reportProductMaterialList.Count == 0)
                        {
                            foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                            {
                                ReportProductMaterial reportProductMaterial = new ReportProductMaterial();
                                reportProductMaterial.ProductMaterialName =
                                    outputMaterial.ProductMaterial.ProductMaterialName;
                                reportProductMaterial.ProductMaterialUnit =
                                    outputMaterial.ProductMaterial.ProductMaterialUnit;
                                reportProductMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;

                                int productMaterialId = outputMaterial.ProductMaterialId;
                                int amount = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    InputMaterial inputMaterial =
                                        db.InputMaterials.FirstOrDefault(
                                            m =>
                                                m.ProductMaterialId == productMaterialId && m.InputBillId ==
                                                exportFrom.InputbillId);
                                    amount += exportFrom.ExportFromQuantity * (int)inputMaterial.InputMaterialPrice;
                                }
                                reportProductMaterial.ProductMaterialAmount = amount;
                                reportProductMaterialList.Add(reportProductMaterial);

                                expense += amount;
                            }
                        }
                        else
                        {
                            foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                            {
                                foreach (ReportProductMaterial reportProductMaterial in reportProductMaterialList)
                                {
                                    if (
                                        reportProductMaterial.ProductMaterialName.Equals(
                                            outputMaterial.ProductMaterial.ProductMaterialName))
                                    {
                                        reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;

                                        int productMaterialId = outputMaterial.ProductMaterialId;
                                        int amount = 0;
                                        foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                        {
                                            InputMaterial inputMaterial =
                                                db.InputMaterials.FirstOrDefault(
                                                    m =>
                                                        m.ProductMaterialId == productMaterialId && m.InputBillId ==
                                                        exportFrom.InputbillId);
                                            amount += exportFrom.ExportFromQuantity * (int)inputMaterial.InputMaterialPrice;
                                        }
                                        reportProductMaterial.ProductMaterialAmount += amount;

                                        expense += amount;
                                    }
                                }
                            }
                        }



                        #endregion

                    }

                    incomePeriod.RevenueAmount = revenue;
                    incomePeriod.OrderList = reportProductOrderList;
                    incomePeriod.MaterialExpense = expense;
                    incomePeriod.ProductMaterialList = reportProductMaterialList;
                    incomePeriod.Income = revenue - expense;

                    totalIncome += incomePeriod.Income;
                }

                #endregion

                #region Finish result

                result.ReportProductIncomePeriodList = incomeReportPeriods;
                result.TotalIncome = totalIncome;

                #endregion
                #endregion
            }
            else if (startMonth != null && endMonth != null && startYear != null && endYear != null)
            {
                #region Get report product monthly

                result.StartMonth = startMonth.Value;
                result.StartYear = startYear.Value;
                result.EndMonth = endMonth.Value;
                result.EndYear = endYear.Value;

                #region Create List income report period

                DateTime firstMonthStartDate = new DateTime(startYear.Value, startMonth.Value, 1);
                DateTime lastMonthStartDate = new DateTime(endYear.Value, endMonth.Value, 1);
                List<ReportProductIncomePeriod> incomeReportPeriods = new List<ReportProductIncomePeriod>();

                int totalIncome = 0;

                for (DateTime tempDate = firstMonthStartDate;
                    tempDate <= lastMonthStartDate;
                    tempDate = tempDate.AddMonths(1))
                {
                    ReportProductIncomePeriod reportPeriod = new ReportProductIncomePeriod
                    {
                        Month = tempDate.Month,
                        Year = tempDate.Year
                    };

                    int revenue = 0;
                    int expense = 0;

                    DateTime tempEndDate = tempDate.AddMonths(1);

                    List<OrderItem> orderItems =
                        db.OrderItems.Where(
                            m =>
                                m.ProductId == productId && m.Order.OrderStatus == 5 && m.Order.FinishTime >= tempDate &&
                                m.Order.FinishTime < tempEndDate).ToList();
                    List<ReportProductOrder> reportProductOrderList = new List<ReportProductOrder>();
                    List<ReportProductMaterial> reportProductMaterialList = new List<ReportProductMaterial>();
                    foreach (OrderItem orderItem in orderItems)
                    {
                        #region Create ReportProductOrder

                        ReportProductOrder reportProductOrder = new ReportProductOrder();
                        reportProductOrder.OrderCode = orderItem.Order.OrderCode;
                        reportProductOrder.Quantity = orderItem.Quantity;
                        reportProductOrder.Amount = orderItem.Amount;
                        reportProductOrder.DiscountAmount = (orderItem.Amount * (orderItem.Order.DiscountAmount * 100 / orderItem.Order.Amount)) / 100;
                        reportProductOrder.Revenue = reportProductOrder.Amount - reportProductOrder.DiscountAmount;
                        reportProductOrderList.Add(reportProductOrder);

                        revenue += reportProductOrder.Revenue;

                        #endregion

                        #region Create ReportProductMaterial

                        if (reportProductMaterialList.Count == 0)
                        {
                            foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                            {
                                ReportProductMaterial reportProductMaterial = new ReportProductMaterial();
                                reportProductMaterial.ProductMaterialName =
                                    outputMaterial.ProductMaterial.ProductMaterialName;
                                reportProductMaterial.ProductMaterialUnit =
                                    outputMaterial.ProductMaterial.ProductMaterialUnit;
                                reportProductMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;

                                int productMaterialId = outputMaterial.ProductMaterialId;
                                int amount = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    InputMaterial inputMaterial =
                                        db.InputMaterials.FirstOrDefault(
                                            m =>
                                                m.ProductMaterialId == productMaterialId && m.InputBillId ==
                                                exportFrom.InputbillId);
                                    amount += exportFrom.ExportFromQuantity * (int)inputMaterial.InputMaterialPrice;
                                }
                                reportProductMaterial.ProductMaterialAmount = amount;
                                reportProductMaterialList.Add(reportProductMaterial);

                                expense += amount;
                            }
                        }
                        else
                        {
                            foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                            {
                                foreach (ReportProductMaterial reportProductMaterial in reportProductMaterialList)
                                {
                                    if (
                                        reportProductMaterial.ProductMaterialName.Equals(
                                            outputMaterial.ProductMaterial.ProductMaterialName))
                                    {
                                        reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;

                                        int productMaterialId = outputMaterial.ProductMaterialId;
                                        int amount = 0;
                                        foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                        {
                                            InputMaterial inputMaterial =
                                                db.InputMaterials.FirstOrDefault(
                                                    m =>
                                                        m.ProductMaterialId == productMaterialId && m.InputBillId ==
                                                        exportFrom.InputbillId);
                                            amount += exportFrom.ExportFromQuantity * (int)inputMaterial.InputMaterialPrice;
                                        }
                                        reportProductMaterial.ProductMaterialAmount += amount;

                                        expense += amount;
                                    }
                                }
                            }
                        }



                        #endregion

                    }

                    reportPeriod.RevenueAmount = revenue;
                    reportPeriod.OrderList = reportProductOrderList;
                    reportPeriod.MaterialExpense = expense;
                    reportPeriod.ProductMaterialList = reportProductMaterialList;
                    reportPeriod.Income = revenue - expense;

                    incomeReportPeriods.Add(reportPeriod);
                    totalIncome += reportPeriod.Income;
                }

                #endregion

                #region Finish result

                result.ReportProductIncomePeriodList = incomeReportPeriods;
                result.TotalIncome = totalIncome;

                #endregion
                #endregion
            }
            else if (startMonth == null && endMonth == null && startYear != null && endYear != null)
            {
                #region Get report product monthly

                result.StartYear = startYear.Value;
                result.EndYear = endYear.Value;

                #region Create List income report period

                DateTime firstMonthStartDate = new DateTime(startYear.Value, 1, 1);
                DateTime lastMonthStartDate = new DateTime(endYear.Value, 1, 1);
                List<ReportProductIncomePeriod> incomeReportPeriods = new List<ReportProductIncomePeriod>();

                int totalIncome = 0;

                for (DateTime tempDate = firstMonthStartDate;
                    tempDate <= lastMonthStartDate;
                    tempDate = tempDate.AddYears(1))
                {
                    ReportProductIncomePeriod reportPeriod = new ReportProductIncomePeriod
                    {
                        Year = tempDate.Year
                    };

                    int revenue = 0;
                    int expense = 0;

                    DateTime tempEndDate = tempDate.AddYears(1);

                    List<OrderItem> orderItems =
                        db.OrderItems.Where(
                            m =>
                                m.ProductId == productId && m.Order.OrderStatus == 5 && m.Order.FinishTime >= tempDate &&
                                m.Order.FinishTime < tempEndDate).ToList();
                    List<ReportProductOrder> reportProductOrderList = new List<ReportProductOrder>();
                    List<ReportProductMaterial> reportProductMaterialList = new List<ReportProductMaterial>();
                    foreach (OrderItem orderItem in orderItems)
                    {
                        #region Create ReportProductOrder

                        ReportProductOrder reportProductOrder = new ReportProductOrder();
                        reportProductOrder.OrderCode = orderItem.Order.OrderCode;
                        reportProductOrder.Quantity = orderItem.Quantity;
                        reportProductOrder.Amount = orderItem.Amount;
                        reportProductOrder.DiscountAmount = (orderItem.Amount * (orderItem.Order.DiscountAmount * 100 / orderItem.Order.Amount)) / 100;
                        reportProductOrder.Revenue = reportProductOrder.Amount - reportProductOrder.DiscountAmount;
                        reportProductOrderList.Add(reportProductOrder);

                        revenue += reportProductOrder.Revenue;

                        #endregion

                        #region Create ReportProductMaterial

                        if (reportProductMaterialList.Count == 0)
                        {
                            foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                            {
                                ReportProductMaterial reportProductMaterial = new ReportProductMaterial();
                                reportProductMaterial.ProductMaterialName =
                                    outputMaterial.ProductMaterial.ProductMaterialName;
                                reportProductMaterial.ProductMaterialUnit =
                                    outputMaterial.ProductMaterial.ProductMaterialUnit;
                                reportProductMaterial.ProductMaterialQuantity = outputMaterial.ExportQuantity;

                                int productMaterialId = outputMaterial.ProductMaterialId;
                                int amount = 0;
                                foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                {
                                    InputMaterial inputMaterial =
                                        db.InputMaterials.FirstOrDefault(
                                            m =>
                                                m.ProductMaterialId == productMaterialId && m.InputBillId ==
                                                exportFrom.InputbillId);
                                    amount += exportFrom.ExportFromQuantity * (int)inputMaterial.InputMaterialPrice;
                                }
                                reportProductMaterial.ProductMaterialAmount = amount;
                                reportProductMaterialList.Add(reportProductMaterial);

                                expense += amount;
                            }
                        }
                        else
                        {
                            foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                            {
                                foreach (ReportProductMaterial reportProductMaterial in reportProductMaterialList)
                                {
                                    if (
                                        reportProductMaterial.ProductMaterialName.Equals(
                                            outputMaterial.ProductMaterial.ProductMaterialName))
                                    {
                                        reportProductMaterial.ProductMaterialQuantity += outputMaterial.ExportQuantity;

                                        int productMaterialId = outputMaterial.ProductMaterialId;
                                        int amount = 0;
                                        foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                                        {
                                            InputMaterial inputMaterial =
                                                db.InputMaterials.FirstOrDefault(
                                                    m =>
                                                        m.ProductMaterialId == productMaterialId && m.InputBillId ==
                                                        exportFrom.InputbillId);
                                            amount += exportFrom.ExportFromQuantity * (int)inputMaterial.InputMaterialPrice;
                                        }
                                        reportProductMaterial.ProductMaterialAmount += amount;

                                        expense += amount;
                                    }
                                }
                            }
                        }



                        #endregion

                    }

                    reportPeriod.RevenueAmount = revenue;
                    reportPeriod.OrderList = reportProductOrderList;
                    reportPeriod.MaterialExpense = expense;
                    reportPeriod.ProductMaterialList = reportProductMaterialList;
                    reportPeriod.Income = revenue - expense;

                    incomeReportPeriods.Add(reportPeriod);
                    totalIncome += reportPeriod.Income;
                }

                #endregion

                #region Finish result

                result.ReportProductIncomePeriodList = incomeReportPeriods;
                result.TotalIncome = totalIncome;

                #endregion
                #endregion
            }


            return result;
        }

        public List<sp_GetAllProductIncomeWeekly_Result> GetAllProductIncomeWeekly(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetAllProductIncomeWeekly(startDate, endDate).ToList();
        }
        public List<sp_GetAllProductIncomeMonthly_Result> GetAllProductIncomeMonthly(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetAllProductIncomeMonthly(startDate, endDate).ToList();
        }
        public List<sp_GetAllProductIncomeYearly_Result> GetAllProductIncomeYearly(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetAllProductIncomeYearly(startDate, endDate).ToList();
        }
        public List<sp_GetAllCustomerRevenueWeekly_Result> GetAllCustomerRevenueWeekly(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetAllCustomerRevenueWeekly(startDate, endDate).ToList();
        }
        public List<sp_GetAllCustomerRevenueMonthly_Result> GetAllCustomerRevenueMonthly(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetAllCustomerRevenueMonthly(startDate, endDate).ToList();
        }
        public List<sp_GetAllCustomerRevenueYearly_Result> GetAllCustomerRevenueYearly(DateTime startDate, DateTime endDate)
        {
            return db.sp_GetAllCustomerRevenueYearly(startDate, endDate).ToList();
        }


    }


}