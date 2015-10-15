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
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (Session["User"] != null)
            {
                string cusId = Session["UserId"].ToString();
                var orderList = db.Orders.Where(n => n.CustomerUserId == cusId && n.OrderStatus != "Chờ xác nhận").OrderByDescending(n => n.CreateTime).ToList().ToPagedList(pageNumber, pageSize);
                var confirmOrderList = db.Orders.Where(x => x.CustomerUserId == cusId && x.OrderStatus == "Chờ xác nhận").OrderBy(n => n.CreateTime).ToList();
                List<int> checkId = new List<int> { };
                for (int i = 0; i < confirmOrderList.Count; i++)
                {
                    checkId.Insert(i, (int)confirmOrderList[i].PreviousOrderId);
                    for (int j = 0; j < orderList.Count; j++)
                    {
                        if (orderList[j].OrderId == checkId[i])
                        {
                            orderList[j].ToString().Remove(j);
                        }
                    }
                }
                return View(orderList);
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult ConfirmIndex(int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            if (Session["User"] != null)
            {
                string cusId = Session["UserId"].ToString();
                var confirmOrderList = db.Orders.Where(x => x.CustomerUserId == cusId && x.OrderStatus == "Chờ xác nhận").OrderBy(n => n.CreateTime).ToList().ToPagedList(pageNumber, pageSize);
                return View(confirmOrderList);
            }
            return RedirectToAction("Index", "Home");
        }
        public ActionResult OrderDetail(int orderId)
        {
            Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            ViewBag.orderItems = orderItems;
            return View(order);
        }

        public ActionResult CancelOrder(int? orderId)
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

        public ActionResult CancelOrderConfirm(int orderId)
        {
            try
            {
                Order order = db.Orders.Find(orderId);
                if (order.OrderStatus == "Chờ xử lý")
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
                    order.OrderStatus = "Đã hủy";
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
            Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
            List<OrderItem> oldOrderItems = db.OrderItems.Where(x => x.OrderId == confirmedOrder.PreviousOrderId).ToList();
            ViewBag.oldOrder = oldOrder;
            ViewBag.orderItems = orderItems;
            ViewBag.oldOrderItems = oldOrderItems;
            return View(confirmedOrder);
        }

        public ActionResult AcceptEditedOrder(int orderId)
        {
            Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
            List<OrderItem> oldOrderItems = db.OrderItems.Where(x => x.OrderId == confirmedOrder.PreviousOrderId).ToList();
            confirmedOrder.OrderStatus = "Đã duyệt";
            for (int i = 0; i < oldOrderItems.Count; i++)
            {
                db.OrderItems.Remove(oldOrderItems[i]);
            }
            db.Orders.Remove(oldOrder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult CancelEditedOrder(int orderId)
        {
            Order confirmedOrder = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
            List<OrderItem> orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
            Order oldOrder = db.Orders.SingleOrDefault(x => x.OrderId == confirmedOrder.PreviousOrderId);
            oldOrder.OrderStatus = "Chờ xử lý";
            for (int i = 0; i < orderItems.Count; i++)
            {
                db.OrderItems.Remove(orderItems[i]);
            }
            db.Orders.Remove(confirmedOrder);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult CancelBothOrder(int? orderId, int? oldOrderId)
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

        public ActionResult CancelBothOrderConfirm(int orderId, int oldOrderId)
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
        #endregion
    }
}