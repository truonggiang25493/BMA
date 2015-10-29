using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using PagedList;
using PagedList.Mvc;
using System.Net;
using System.Data;
using System.Data.Entity;

namespace BMA.Controllers
{
    public class CusManageOrderController : Controller
    {
        BMAEntities db = new BMAEntities();
        public ActionResult Index(int? page)
        {
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                if (Session["User"] != null)
                {
                    int cusId = Convert.ToInt32(Session["UserId"]);
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
                    var orderList = orderToCheck.OrderByDescending(n=>n.CreateTime).ToPagedList(pageNumber, pageSize);
                    return View(orderList);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
            
        }

        public ActionResult ConfirmIndex(int? page)
        {
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                if (Session["User"] != null)
                {
                    int cusUserId = Convert.ToInt32(Session["UserId"]);
                    var confirmOrderList = db.Orders.Where(x => x.CustomerUserId == cusUserId && x.OrderStatus == 1).OrderBy(n => n.CreateTime).ToList().ToPagedList(pageNumber, pageSize);
                    return View(confirmOrderList);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {

                return RedirectToAction("Index", "Error");
            }
            
        }
        public ActionResult OrderDetail(int orderId)
        {
            try
            {
                Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
                ViewBag.orderItems = orderItems;
                return View(order);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
            
        }

        public ActionResult CancelOrder(int? orderId)
        {
            try
            {
                if (orderId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Order order = db.Orders.Find(orderId);
                List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
                ViewBag.orderItems = orderItems;
                if (order == null)
                {
                    return HttpNotFound();
                }
                return View(order);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
            
        }

        public ActionResult CancelOrderConfirm(int orderId)
        {
            try
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
            }
            catch (DataException)
            {
                return RedirectToAction("CancelOrder");
            }
            return RedirectToAction("Index");
        }

        #region ConfirmOrder
        public ActionResult ConfirmOrder(int orderId)
        {
            try
            {
                Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
                Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
                List<OrderItem> oldOrderItems = db.OrderItems.Where(x => x.OrderId == confirmedOrder.PreviousOrderId).ToList();
                ViewBag.oldOrder = oldOrder;
                ViewBag.orderItems = orderItems;
                ViewBag.oldOrderItems = oldOrderItems;
                return View(confirmedOrder);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
           
        }

        public ActionResult AcceptEditedOrder(int orderId)
        {
            try
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
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }            
        }

        public ActionResult CancelEditedOrder(int orderId)
        {
            try
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
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }         
        }


        public ActionResult CancelBothOrder(int? orderId, int? oldOrderId)
        {
            try
            {
                if (orderId == null || oldOrderId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
                Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
                List<OrderItem> oldOrderItems = db.OrderItems.Where(x => x.OrderId == confirmedOrder.PreviousOrderId).ToList();
                ViewBag.oldOrder = oldOrder;
                ViewBag.orderItems = orderItems;
                ViewBag.oldOrderItems = oldOrderItems;
                ViewBag.oldOrder = oldOrder;
                if (confirmedOrder == null || oldOrder == null)
                {
                    return HttpNotFound();
                }
                return View(confirmedOrder);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }         
        }

        public ActionResult CancelBothOrderConfirm(int orderId, int oldOrderId)
        {
            try
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

                for (int i = 0; i < orderItems.Count; i++)
                {
                    db.OrderItems.Remove(orderItems[i]);
                }
                db.Orders.Remove(confirmedOrder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }          
        }
        #endregion
    }
}