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

        public ReportIncomeViewModel ReviewIncomeByTimeDetail(DateTime startDate, DateTime endDate)
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
                db.Orders.Where(m => m.OrderStatus == 5 && m.FinishTime >= startDate && m.FinishTime <= tempEndDate)
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
            List<Order> canceledOrderList = db.Orders.Where(m => m.OrderStatus == 6 && m.CancelTime >= startDate && m.CancelTime <= tempEndDate)
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
            result.Income = result.RevenueAmount - result.MaterialExpense;
            #endregion
            return result;
        }
    }
}