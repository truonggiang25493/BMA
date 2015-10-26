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
using BMA.Business;

namespace BMA.Controllers
{
    public class CusManageOrderController : Controller
    {
        public ActionResult Index(int? page)
        {
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                if (Session["User"] != null)
                {
                    CusManageOrderBusiness cob = new CusManageOrderBusiness();
                    int cusId = Convert.ToInt32(Session["UserId"]);
                    var orderList = cob.GetOrder(cusId).ToPagedList(pageNumber, pageSize);
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
                    CusManageOrderBusiness cob = new CusManageOrderBusiness();
                    int cusId = Convert.ToInt32(Session["UserId"]);
                    var confirmOrderList = cob.GetConfirmOrder(cusId).ToPagedList(pageNumber, pageSize);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                var order = cob.GetOrderDetail(orderId);
                var orderItems = cob.GetOrderItem(orderId);
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

        public ActionResult CancelOrder(int orderId)
        {
            try
            {
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                var order = cob.GetOrderDetail(orderId);
                var orderItems = cob.GetOrderItem(orderId);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                cob.CancelOrderConfirm(orderId);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                var confirmedOrder = cob.GetOrderDetail(orderId);
                var orderItems = cob.GetOrderItem(orderId);
                int? previousId = confirmedOrder.PreviousOrderId;
                var oldOrder = cob.GetOrderDetail(previousId);
                var oldOrderItems = cob.GetOrderItem(previousId);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                cob.AcceptEditedOrder(orderId);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                cob.CancelEditedOrder(orderId);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                if (orderId == null || oldOrderId == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                var confirmedOrder = cob.GetOrderDetail(orderId);
                var orderItems = cob.GetOrderItem(orderId);
                Order oldOrder = cob.GetOrderDetail(oldOrderId);
                List<OrderItem> oldOrderItems = cob.GetOrderItem(oldOrderId);
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
                CusManageOrderBusiness cob = new CusManageOrderBusiness();
                cob.CancelBothOrderConfirm(orderId, oldOrderId);
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