using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;
using BMA.Models;
using BMA.Models.ViewModel;

namespace BMA.Controllers
{
    public class OrderController : Controller
    {
        private BMAEntities db = new BMAEntities();
        // GET: Order
        public ActionResult Index()
        {
            List<Order> orderList = db.Orders.ToList();
            // Custom sort
            orderList.Sort(
                delegate(Order o1, Order o2)
                {
                    if (o1.OrderStatus == o2.OrderStatus)
                    {
                        return o1.CreateTime.CompareTo(o2.CreateTime);
                    }
                    return o1.OrderStatus.CompareTo(o2.OrderStatus);
                });
            return View(orderList);
        }

        // GET: Detail
        public ActionResult Detail(int id)
        {
            OrderViewModel orderViewModel = OrderBusiness.GetOrderViewModel(id);
            if (orderViewModel != null)
            {
                return View(orderViewModel);
            }
            return RedirectToAction("Index");
        }

        // GET: Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            Order order = db.Orders.FirstOrDefault(m => m.OrderId == id);
            if (order != null)
            {
                // Get and transfer total amount and total tax of order
                int totalAmount = 0;
                int totalTax = 0;
                foreach (var orderItem in order.OrderItems)
                {
                    totalAmount += orderItem.Amount;
                    totalTax += orderItem.TaxAmount;
                }
                ViewBag.TotalAmount = totalAmount;
                ViewBag.TotalTax = totalTax;
                //  if customer is not null, get and transfer
                //  else get and transfer guest info
                if (order.CustomerUserId != null)
                {
                    Customer customer = db.Customers.FirstOrDefault(m => m.UserId == order.CustomerUserId);
                    ViewBag.Customer = customer;
                }
                else
                {
                    GuestInfo guestInfo = db.GuestInfoes.FirstOrDefault(m => m.GuestInfoId == order.GuestInfoId);
                    ViewBag.GuestInfo = guestInfo;
                }
            }


            return View(order);
        }
        [HttpPost]
        public ActionResult Edit(FormCollection form)
        {
            return RedirectToAction("Index", "Order");
        }

        [HttpPost]
        public ActionResult ApproveOrder(int deposit, DateTime deliveryDate, int orderId)
        {
            bool rs = OrderBusiness.ApproveOrder(orderId, deposit, deliveryDate);
            if (!rs)
            {
                return RedirectToAction("Detail", orderId);
            }
            return RedirectToAction("Index");
        }

    }
}