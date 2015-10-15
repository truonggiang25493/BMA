using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
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

        private List<CartViewModel> GetCart()
        {
            List<CartViewModel> cartList = Session["Cart"] as List<CartViewModel>;
            if (cartList == null)
            {
                cartList = new List<CartViewModel>();
                Session["Cart"] = cartList;
            }
            return cartList;
        }
        public ActionResult Add()
        {
            List<CartViewModel> inputList = GetCart();
            if (inputList.Count == 0)
            {
                List<Product> productList = db.Products.Where(m => m.IsActive).ToList();
                foreach (Product product in productList)
                {
                    CartViewModel cart = new CartViewModel();
                    cart.ProductId = product.ProductId;
                    cart.ProductName = product.ProductName;
                    cart.Quantity = 0;
                    cart.StandardPrice = product.ProductStandardPrice;
                    inputList.Add(cart);
                }
            }
            List<CartViewModel> cartList = inputList.OrderByDescending(m => m.Quantity).ToList();
            Session["Cart"] = cartList;
            return View(cartList);
        }

        [HttpPost]
        public ActionResult AddProductToOrder(int productId, int price, int quantity)
        {
            List<CartViewModel> cartList = GetCart();
            if (cartList.Count != 0)
            {
                foreach (CartViewModel cartViewModel in cartList)
                {
                    if (cartViewModel.ProductId == productId)
                    {
                        cartViewModel.Quantity = quantity;
                        cartViewModel.RealPrice = price;
                    }
                }
            }
            Session["Cart"] = cartList;
            return RedirectToAction("Add");
        }

        public ActionResult AddCustomerToOrder()
        {
            return View("AddCustomerToOrder");
        }

        [HttpPost]
        public ActionResult CheckoutWithCustomer(string customerId)
        {
            List<CartViewModel> inputCartList = GetCart();
            OrderViewModel order = OrderBusiness.MakeOrderViewModel(inputCartList, customerId);
            return View(order);
        }

        [HttpPost]
        public ActionResult AddOrderForCustomer(int deposit, DateTime deliveryDate, string customerId)
        {
            List<CartViewModel> cart = Session["Cart"] as List<CartViewModel>;
            bool rs = OrderBusiness.AddOrderForCustomer(cart, customerId, deposit, deliveryDate);
            if (rs)
            {
                return View("Index");

            }
            return View("CheckoutWithCustomer", customerId);
        }
    }
}