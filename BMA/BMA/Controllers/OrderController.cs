using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

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
                    if (o1.OrderStatus[0].Equals(o2.OrderStatus[0]))
                    {
                        return o1.CreateTime.CompareTo(o2.CreateTime);
                    }
                    return o1.OrderStatus[0].CompareTo(o2.OrderStatus[0]);
                });
            return View(orderList);
        }

        // GET: Detail
        public ActionResult Detail(int id)
        {
            return View();
        }

        // GET: Delete
        public ActionResult Delete(int id)
        {
            return View();
        }

        // GET: Edit
        public ActionResult Edit(int id)
        {
            return View();
        }
    }
}