﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;
using BMA.Models.ViewModel;
using System.Web.Mvc;
using BMA.Controllers;

namespace BMA.Business
{
    public class CustomerOrderBusiness
    {
        private readonly BMAEntities db;
        public CustomerOrderBusiness()
        {
            db = new BMAEntities();
        }

        public TaxRate GetTaxRate()
        {
            var taxRate = db.TaxRates.SingleOrDefault(n => n.TaxTypeId == 1);
            return taxRate;
        }
        public List<CustomerCartViewModel> GetOrderToCart(int orderId)
        {
            List<CustomerCartViewModel> lstCart = new List<CustomerCartViewModel>();
            Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            var orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            if (lstCart != null)
            {
                for (int i = 0; i < orderItems.Count; i++)
                {
                    CustomerCartViewModel cart = lstCart.Find(n => n.ProductId == orderItems[i].ProductId);
                    if (cart == null)
                    {
                        cart = new CustomerCartViewModel(orderItems[i].ProductId);
                        cart.ProductId = orderItems[i].ProductId;
                        cart.ProductName = orderItems[i].Product.ProductName;
                        cart.Quantity = orderItems[i].Quantity;
                        cart.Price = orderItems[i].RealPrice;
                        cart.Total = orderItems[i].Amount;
                        lstCart.Add(cart);
                    }
                }
            }
            return lstCart;
        }

        public bool EditOrder(int orderId, DateTime planDeliveryDate, int Amount, int taxAmount, int cusUserId, List<CustomerCartViewModel> cart)
        {
            Order order = db.Orders.Find(orderId);
            order.PlanDeliveryTime = planDeliveryDate;
            order.Amount = Amount;
            order.TaxAmount = taxAmount;
            order.CustomerUserId = cusUserId;
            db.SaveChanges();
            List<OrderItem> orderItem = db.OrderItems.Where(n => n.OrderId == order.OrderId).ToList();
            for (int i = 0; i < orderItem.Count; i++)
            {
                db.OrderItems.Remove(orderItem[i]);
            }
            foreach (var item in cart)
            {
                OrderItem orderDetail = new OrderItem();
                orderDetail.OrderId = order.OrderId;
                orderDetail.ProductId = item.ProductId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.RealPrice = item.Price;
                orderDetail.Amount = item.Total;
                orderDetail.TaxAmount = Convert.ToInt32(item.Total * 0.1);
                db.OrderItems.Add(orderDetail);
            }
            db.SaveChanges();
            return true;
        }
        public bool OrderProduct(string orderTime, DateTime planDeliveryDate, int Amount, int taxAmount, int cusUserId, List<CustomerCartViewModel> cart)
        {
            Order order = new Order();
            order.OrderCode = orderTime;
            order.CreateTime = DateTime.Now;
            order.PlanDeliveryTime = planDeliveryDate;
            order.DepositAmount = 0;
            order.IsStaffEdit = false;
            order.CustomerEditingFlag = false;
            order.OrderStatus = 0;
            order.Amount = Amount;
            order.TaxAmount = taxAmount;
            order.CustomerUserId = cusUserId;
            db.Orders.Add(order);
            db.SaveChanges();
            string orderNumber = order.OrderId.ToString().PadLeft(4, '0');
            order.OrderCode = String.Format("{0}{1}{2}", 'O', orderTime, orderNumber);
            db.SaveChanges();
            foreach (var item in cart)
            {
                OrderItem orderDetail = new OrderItem();
                orderDetail.OrderId = order.OrderId;
                orderDetail.ProductId = item.ProductId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.RealPrice = item.Price;
                orderDetail.Amount = item.Total;
                orderDetail.TaxAmount = Convert.ToInt32(item.Total * 0.1);
                db.OrderItems.Add(orderDetail);
            }
            db.SaveChanges();
            return true;
        }


        public bool GuestOrderProduct(string orderTime, DateTime planDeliveryDate, int Amount, int taxAmount, List<CustomerCartViewModel> cart, string sName, string sPhone, string sAddress, string sEmail)
        {
            Order order = new Order();
            order.OrderCode = orderTime;
            order.CreateTime = DateTime.Now;
            order.PlanDeliveryTime = planDeliveryDate;
            order.DepositAmount = 0;
            order.IsStaffEdit = false;
            order.CustomerEditingFlag = false;
            order.OrderStatus = 0;
            order.Amount = Amount;
            order.TaxAmount = taxAmount;
            GuestInfo guestInfo = new GuestInfo();
            guestInfo.GuestInfoName = sName;
            guestInfo.GuestInfoPhone = sPhone;
            guestInfo.GuestInfoAddress = sAddress;
            guestInfo.GuestInfoEmail = sEmail;
            db.GuestInfoes.Add(guestInfo);
            db.SaveChanges();
            order.GuestInfoId = guestInfo.GuestInfoId;
            db.Orders.Add(order);
            db.SaveChanges();
            string orderNumber = order.OrderId.ToString().PadLeft(4, '0');
            order.OrderCode = String.Format("{0}{1}{2}", 'O', orderTime, orderNumber);
            db.SaveChanges();
            foreach (var item in cart)
            {
                OrderItem orderDetail = new OrderItem();
                orderDetail.OrderId = order.OrderId;
                orderDetail.ProductId = item.ProductId;
                orderDetail.Quantity = item.Quantity;
                orderDetail.RealPrice = item.Price;
                orderDetail.Amount = item.Total;
                orderDetail.TaxAmount = Convert.ToInt32(item.Total * 0.1);
                db.OrderItems.Add(orderDetail);
            }
            db.SaveChanges();
            return true;
        }

        public string GetOrderCode()
        {
            List<Order> lstOrder = db.Orders.OrderBy(n => n.OrderId).ToList();
            string orderCode = lstOrder.LastOrDefault().OrderCode;
            return orderCode;
        }

        public string EditGetOrderCode(int orderId)
        {
            Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            string orderCode = order.OrderCode;
            return orderCode;
        }
    }
}