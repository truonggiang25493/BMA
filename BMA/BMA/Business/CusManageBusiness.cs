using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;
using System.Data.Entity;

namespace BMA.Business
{
    public class CusManageBusiness
    {
        private readonly BMAEntities db;
        public CusManageBusiness()
        {
            db = new BMAEntities();
        }

        public List<Order> GetOrder(int cusId)
        {
            List<Order> orderToCheck = db.Orders.Where(n => n.CustomerUserId == cusId && n.OrderStatus != 1).ToList();
            var confirmOrderList = db.Orders.Where(x => x.CustomerUserId == cusId && x.OrderStatus == 1).OrderBy(n => n.CreateTime).ToList();
            List<int> checkId = new List<int> { };
            for (int i = 0; i < confirmOrderList.Count; i++)
            {
                checkId.Insert(i, (int)confirmOrderList[i].PreviousOrderId);
                for (int j = 0; j < orderToCheck.Count; j++)
                {
                    if (orderToCheck[j].OrderId == checkId[i])
                    {
                        orderToCheck.RemoveAt(j);
                    }
                }
            }
            var orderList = orderToCheck.OrderByDescending(n => n.CreateTime).ToList();
            return orderList;
        }

        public List<Order> GetConfirmOrder(int cusId)
        {
            var confirmOrderList = db.Orders.Where(x => x.CustomerUserId == cusId && x.OrderStatus == 1).OrderBy(n => n.CreateTime).ToList();
            return confirmOrderList;
        }

        public Order GetOrderDetail(int orderId)
        {
            Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            return order;
        }

        public Order GetOrderDetail(int? previousId)
        {
            Order order = db.Orders.SingleOrDefault(x => x.OrderId == previousId);
            return order;
        }

        public List<OrderItem> GetOrderItem(int orderId)
        {

            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            return orderItems;
        }

        public List<OrderItem> GetOrderItem(int? previousId)
        {

            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == previousId).ToList();
            return orderItems;
        }

        public bool CancelOrderConfirm(int orderId)
        {
            Order order = db.Orders.Find(orderId);
            if (order.OrderStatus == 0)
            {
                List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
                for (int i = 0; i < orderItems.Count; i++)
                {
                    db.OrderItems.Remove(orderItems[i]);
                }
                db.Orders.Remove(order);
            }
            else
            {
                order.OrderStatus = 6;
            }
            db.SaveChanges();
            return true;
        }

        public bool AcceptEditedOrder(int orderId)
        {
            Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
            List<OrderItem> oldOrderItems = db.OrderItems.Where(x => x.OrderId == confirmedOrder.PreviousOrderId).ToList();
            confirmedOrder.OrderStatus = 2;
            for (int i = 0; i < oldOrderItems.Count; i++)
            {
                db.OrderItems.Remove(oldOrderItems[i]);
            }
            confirmedOrder.ConfirmTime = DateTime.Now;
            db.Orders.Remove(oldOrder);
            db.SaveChanges();
            return true;
        }

        public bool CancelEditedOrder(int orderId)
        {
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId);
            DbContextTransaction contextTransaction = db.Database.BeginTransaction();
            foreach (OrderItem orderItem in order.OrderItems)
            {
                foreach (OutputMaterial outputMaterial in orderItem.OutputMaterials)
                {
                    foreach (ExportFrom exportFrom in outputMaterial.ExportFroms)
                    {

                        // Return InputMaterial
                        int productMaterialId = outputMaterial.ProductMaterialId;
                        InputMaterial inputMaterial = db.InputMaterials.FirstOrDefault(
                            m => m.ProductMaterialId == productMaterialId && m.InputBillId == exportFrom.InputbillId);
                        if (inputMaterial != null)
                        {
                            inputMaterial.RemainQuantity += exportFrom.ExportFromQuantity;
                        }
                        // Return ProductMaterial
                        ProductMaterial productMaterial =
                            db.ProductMaterials.FirstOrDefault(m => m.ProductMaterialId == productMaterialId);
                        if (productMaterial != null)
                        {
                            productMaterial.CurrentQuantity += exportFrom.ExportFromQuantity;
                        }

                    }
                    db.SaveChanges();
                    // Remove ExportFrom
                    db.ExportFroms.RemoveRange(outputMaterial.ExportFroms);

                }
                // Remove OutputMaterial
                db.OutputMaterials.RemoveRange(orderItem.OutputMaterials);
            }
            order.ReturnDeposit = 0;
            // Change order status become "Hủy"
            order.OrderStatus = 6;
            order.CancelTime = DateTime.Now;
            //Temp Bug
            order.CancelUserId = 2;

            db.SaveChanges();
            // Commit transaction
            try
            {
                contextTransaction.Commit();
            }
            catch (Exception)
            {
                contextTransaction.Rollback();
            }
            finally
            {
                contextTransaction.Dispose();
            }
            return true;
        }

        public bool CancelBothOrderConfirm(int? orderId, int? oldOrderId)
        {
            Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
            List<OrderItem> oldOrderItems = db.OrderItems.Where(x => x.OrderId == confirmedOrder.PreviousOrderId).ToList();

            for (int i = 0; i < oldOrderItems.Count; i++)
            {
                db.OrderItems.Remove(oldOrderItems[i]);
            }
            db.Orders.Remove(oldOrder);

            confirmedOrder.OrderStatus = 6;
            db.SaveChanges();
            return true;
        }
    }
}