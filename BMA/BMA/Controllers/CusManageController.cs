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
    public class CusManageController : Controller
    {
        BMAEntities db = new BMAEntities();

        public ActionResult Index()
        {
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpGet]
        public ActionResult ChangeInformation(int UserId)
        {
            AccountBusiness ab = new AccountBusiness();
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = ab.GetUser(UserId);
            return View(user);
        }
        [HttpPost, ActionName("ChangeInformation")]
        public ActionResult ChangeInformationConfirm(FormCollection f)
        {
            AccountBusiness ab = new AccountBusiness();
            int cusUserId = Convert.ToInt32(Session["UserId"]);
            string sName = f["txtName"];
            string sAddress = f["txtAdress"];
            string sEmail = f["txtEmail"];
            string sTaxCode = f["txtTaxCode"];
            string sPhone = f["txtPhone"];
            ab.ChangeInformation(cusUserId, sName, sEmail, sAddress, sTaxCode, sPhone);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ChangePassword(int UserId)
        {
            AccountBusiness ab = new AccountBusiness();
            if (Session["User"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var user = ab.GetUser(UserId);
            return View(user);
        }
        [HttpPost, ActionName("ChangePassword")]
        public ActionResult ChangePasswordConfirm(FormCollection f)
        {
            AccountBusiness ab = new AccountBusiness();
            int cusUserId = Convert.ToInt32(Session["UserId"]);
            string sOldPass = f["txtOldPass"];
            string sNewPass = f["txtNewPass"];
            string sNewPassConfirm = f["txtNewPassConfirm"];
            if (!ab.checkPass(cusUserId, sOldPass))
            {
                TempData["checkOldPass"] = "Mật khẩu không đúng! Vui lòng thử lại.";
                return RedirectToAction("ChangePassword", new { UserId = cusUserId });
            }
            if (sOldPass == sNewPass)
            {
                TempData["checkUnchange"] = "Mật khẩu mới và mật khẩu cũ giống nhau! Vui lòng thử lại.";
                return RedirectToAction("ChangePassword", new { UserId = cusUserId });
            }
            if (sNewPass != sNewPassConfirm)
            {
                TempData["checkConfirmPass"] = "Mật khẩu và mật khẩu xác nhận không trùng khớp.";
                return RedirectToAction("ChangePassword", new { UserId = cusUserId });
            }
            ab.ChangePassword(cusUserId, sNewPass);
            return RedirectToAction("Index");
        }

        public ActionResult OrderList(int? page)
        {
            CusManageBusiness cmb = new CusManageBusiness();
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                if (Session["User"] != null)
                {
                    int cusId = Convert.ToInt32(Session["CusUserId"]);
                    List<OrderItem> orderItemList = new List<OrderItem>();
                    var orderList = cmb.GetOrder(cusId).ToPagedList(pageNumber, pageSize);
                    foreach (var item in orderList)
                    {
                        orderItemList = orderItemList.Union(cmb.GetOrderItem(item.OrderId)).ToList();
                    }
                    ViewBag.orderItemList = orderItemList;
                    return View(orderList);
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }

        }

        public ActionResult ConfirmList(int? page)
        {
            CusManageBusiness cmb = new CusManageBusiness();
            try
            {
                int pageSize = 10;
                int pageNumber = (page ?? 1);
                if (Session["User"] != null)
                {
                    int cusUserId = Convert.ToInt32(Session["CusUserId"]);
                    List<OrderItem> orderItemList = new List<OrderItem>();
                    var confirmOrderList = cmb.GetConfirmOrder(cusUserId).ToPagedList(pageNumber, pageSize);
                    foreach (var item in confirmOrderList)
                    {
                        orderItemList = orderItemList.Union(cmb.GetOrderItem(item.OrderId)).ToList();
                    }
                    ViewBag.orderItemList = orderItemList;
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
            CusManageBusiness cmb = new CusManageBusiness();
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            try
            {

                var order = cmb.GetOrderDetail(orderId);
                var orderItems = cmb.GetOrderItem(orderId);
                ViewBag.taxRate = cob.GetTaxRate();
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

        public ActionResult CancelOrderConfirm(int orderId, string strURL)
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
                return RedirectToAction("Index");
            }
            return Redirect(strURL);
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