using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class HomeController : Controller
    {
        AccountBusiness ab = new AccountBusiness();
        //
        // GET: /Home/
        public ActionResult Index()
        {
            ViewBag.Show = "home";
            return View();
        }

        public ActionResult NotificatePartial()
        {
            try
            {
                if (Session["CusNotificateCount"] == null)
                {
                    int count = 0;
                    ViewBag.notificatePartialCount = count;
                    Session["CusNotificateCount"] = count;
                    int newOrderCount = 0;
                    ViewBag.newOrderCountPartial = newOrderCount;
                    Session["CusNewOrderCountPartial"] = newOrderCount;
                    int editOrderCount = 0;
                    ViewBag.editOrderCountPartial = editOrderCount;
                    Session["CusEditOrderCountPartial"] = editOrderCount;
                    int confirmOrderCount = 0;
                    ViewBag.confirmOrderCountPartial = confirmOrderCount;
                    Session["CusConfirmOrderCountPartial"] = confirmOrderCount;
                }
                else
                {
                    int count = Convert.ToInt32(Session["CusNotificateCount"]);
                    ViewBag.notificatePartialCount = count;
                    int newOrderCount = Convert.ToInt32(Session["CusNewOrderCountPartial"]);
                    ViewBag.newOrderCountPartial = newOrderCount;
                    int editOrderCount = Convert.ToInt32(Session["CusEditOrderCountPartial"]);
                    ViewBag.editOrderCountPartial = editOrderCount;
                    int confirmOrderCount = Convert.ToInt32(Session["CusConfirmOrderCountPartial"]);
                    ViewBag.confirmOrderCountPartial = confirmOrderCount;
                }
                return PartialView();
            }
            catch
            {
                return null;
            }
        }

        public int NotificatePartialLink(int count, int newOrderCount, int editOrderCount, int confirmOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = count;
                Session["CusNotificateCount"] = count;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["CusNewOrderCountPartial"] = newOrderCount;
                ViewBag.editOrderCountPartial = editOrderCount;
                Session["CusEditOrderCountPartial"] = editOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["CusConfirmOrderCountPartial"] = confirmOrderCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int RemoveNewOrderNoty(int editOrderCount, int confirmOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = editOrderCount + confirmOrderCount;
                Session["CusNotificateCount"] = editOrderCount + confirmOrderCount;
                int newOrderCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["CusNewOrderCountPartial"] = newOrderCount;
                ViewBag.editOrderCountPartial = editOrderCount;
                Session["CusEditOrderCountPartial"] = editOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["CusConfirmOrderCountPartial"] = confirmOrderCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int RemoveEditOrderNoty(int newOrderCount, int confirmOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = newOrderCount + confirmOrderCount;
                Session["CusNotificateCount"] = newOrderCount + confirmOrderCount;
                int editOrderCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["CusNewOrderCountPartial"] = newOrderCount;
                ViewBag.editOrderCountPartial = editOrderCount;
                Session["CusEditOrderCountPartial"] = editOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["CusConfirmOrderCountPartial"] = confirmOrderCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public int RemoveConfirmOrderNoty(int newOrderCount, int editOrderCount)
        {
            try
            {
                ViewBag.notificatePartialCount = editOrderCount + newOrderCount;
                Session["CusNotificateCount"] = editOrderCount + newOrderCount;
                int confirmOrderCount = 0;
                ViewBag.newOrderCountPartial = newOrderCount;
                Session["CusNewOrderCountPartial"] = newOrderCount;
                ViewBag.editOrderCountPartial = editOrderCount;
                Session["CusEditOrderCountPartial"] = editOrderCount;
                ViewBag.confirmOrderCountPartial = confirmOrderCount;
                Session["CusConfirmOrderCountPartial"] = confirmOrderCount;
                return 1;
            }
            catch
            {
                return -1;
            }
        }
    }
}