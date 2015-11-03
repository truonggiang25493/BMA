using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

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
            Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
            oldOrder.OrderStatus = 0;
            for (int i = 0; i < orderItems.Count; i++)
            {
                db.OrderItems.Remove(orderItems[i]);
            }
            oldOrder.ConfirmTime = DateTime.Now;
            db.Orders.Remove(confirmedOrder);
            db.SaveChanges();
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