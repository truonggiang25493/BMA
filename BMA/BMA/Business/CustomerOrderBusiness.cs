using System;
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

        public bool TurnFlagOn(int orderId)
        {
            Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            order.CustomerEditingFlag = true;
            db.SaveChanges();
            return true;
        }

        public bool TurnFlagOff(int orderId)
        {
            Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            order.CustomerEditingFlag = false;
            db.SaveChanges();
            return true;
        }

        public bool IsActiveProduct(int productId)
        {
            var product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            if (product.IsActive)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                        cart.ProductImage = orderItems[i].Product.ProductImage;
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

        public bool EditOrder(int orderId, DateTime planDeliveryDate, int Amount, int taxAmount, int discount, int cusUserId, List<CustomerCartViewModel> cart)
        {
            Order order = db.Orders.Find(orderId);
            order.PlanDeliveryTime = planDeliveryDate;
            order.Amount = Amount;
            order.TaxAmount = taxAmount;
            order.DiscountAmount = discount;
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
                db.OrderItems.Add(orderDetail);
            }
            db.SaveChanges();
            return true;
        }
        public bool OrderProduct(string orderTime, DateTime planDeliveryDate, int Amount, int taxAmount, int discount, int cusUserId, List<CustomerCartViewModel> cart)
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
            order.DiscountAmount = discount;
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
                db.OrderItems.Add(orderDetail);
            }
            db.SaveChanges();
            return true;
        }


        public bool GuestOrderProduct(string orderTime, DateTime planDeliveryDate, int Amount, int taxAmount, int discount, List<CustomerCartViewModel> cart, string sName, string sPhone, string sAddress, string sEmail)
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
            order.DiscountAmount = discount;
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
                db.OrderItems.Add(orderDetail);
            }
            db.SaveChanges();
            return true;
        }

        //public bool GuestOrderProduct(string orderTime, DateTime planDeliveryDate, int Amount, int taxAmount, List<CustomerCartViewModel> cart, string sName, string sPhone, string sEmail)
        //{
        //    Order order = new Order();
        //    order.OrderCode = orderTime;
        //    order.CreateTime = DateTime.Now;
        //    order.PlanDeliveryTime = planDeliveryDate;
        //    order.DepositAmount = 0;
        //    order.IsStaffEdit = false;
        //    order.CustomerEditingFlag = false;
        //    order.OrderStatus = 0;
        //    order.Amount = Amount;
        //    order.TaxAmount = taxAmount;
        //    GuestInfo guestInfo = db.GuestInfoes.SingleOrDefault(n => n.GuestInfoName == sName && n.GuestInfoPhone == sPhone && n.GuestInfoEmail == sEmail);
        //    order.GuestInfoId = guestInfo.GuestInfoId;
        //    db.Orders.Add(order);
        //    db.SaveChanges();
        //    string orderNumber = order.OrderId.ToString().PadLeft(4, '0');
        //    order.OrderCode = String.Format("{0}{1}{2}", 'O', orderTime, orderNumber);
        //    db.SaveChanges();
        //    foreach (var item in cart)
        //    {
        //        OrderItem orderDetail = new OrderItem();
        //        orderDetail.OrderId = order.OrderId;
        //        orderDetail.ProductId = item.ProductId;
        //        orderDetail.Quantity = item.Quantity;
        //        orderDetail.RealPrice = item.Price;
        //        orderDetail.Amount = item.Total;
        //        orderDetail.TaxAmount = Convert.ToInt32(item.Total * 0.1);
        //        db.OrderItems.Add(orderDetail);
        //    }
        //    db.SaveChanges();
        //    return true;
        //}

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

        public List<OrderItem> OrderSuccess()
        {
            List<Order> lstOrder = db.Orders.OrderBy(n => n.OrderId).ToList();
            int orderId = lstOrder.LastOrDefault().OrderId;
            List<OrderItem> lstOrderItem = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            return lstOrderItem;
        }

        public List<OrderItem> EditOrderSuccess(int orderId)
        {
            List<OrderItem> lstOrderItem = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            return lstOrderItem;
        }

        public bool checkStaffDuplicate(string Email, string phoneNumber)
        {
            var checkUser = db.Users.SingleOrDefault(n => n.Email == Email);
            if (checkUser != null)
            {
                if (checkUser.RoleId == 1)
                {
                    return true;
                }
                else if (checkUser.RoleId == 2)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkCustomerDuplicate(string Email, string phoneNumber)
        {
            var checkUser = db.Users.SingleOrDefault(n => n.Email == Email);
            if (checkUser != null)
            {
                var checkCustomer = db.Customers.SingleOrDefault(n => n.UserId == checkUser.UserId);
                if (checkCustomer.CustomerPhoneNumber == phoneNumber)
                {
                    return true;
                }
            }
            return false;
        }

        public DiscountByQuantity checkDiscount(int quantity)
        {
            var discountCate = db.DiscountByQuantities.ToList();
            int discountId = 0;
            foreach (var item in discountCate)
            {
                if (quantity >= item.QuantityFrom && quantity <= item.QuantityTo)
                {
                    discountId = item.Id;
                }
            }
            var discount = db.DiscountByQuantities.SingleOrDefault(n => n.Id == discountId);
            return discount;
        }

        //public bool checkGuestDuplicate(string Name, string phoneNumber, string Email)
        //{
        //    var checkGuest = db.GuestInfoes.SingleOrDefault(n => n.GuestInfoName == Name && n.GuestInfoPhone == phoneNumber && n.GuestInfoEmail == Email);
        //    if (checkGuest == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}
    }
}