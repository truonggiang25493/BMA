using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class CartController : Controller
    {
        BMAEntities db = new BMAEntities();
        #region Cart method
        public List<Cart> GetCart()
        {
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart == null)
            {
                lstCart = new List<Cart>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
        }
        [HttpGet]
        public ActionResult AddCart(int productId, string strURL)
        {
            Product product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            if (product == null)
            {
                RedirectToAction("Index", "Error");
                return null;
            }
            List<Cart> lstCart = GetCart();
            Cart cart = lstCart.Find(n => n.ProductId == productId);
            if (cart == null)
            {
                cart = new Cart(productId);
                lstCart.Add(cart);
                return Redirect(strURL);
            }
            else
            {
                cart.Quantity++;
                return Redirect(strURL);
            }
        }

        [HttpPost]
        public ActionResult AddCart(FormCollection f, string strURL)
        {
            try
            {
                if (f == null)
                {
                    RedirectToAction("Index", "Error");
                    return null;
                }
                int productId = int.Parse(f["productID"].ToString());
                int productNumber = int.Parse(f["inputNumber"].ToString());

                List<Cart> lstCart = GetCart();
                Cart cart = lstCart.Find(n => n.ProductId == productId);
                if (cart == null)
                {
                    cart = new Cart(productId);
                    cart.Quantity = productNumber;
                    cart.Total = cart.Price * cart.Quantity;
                    lstCart.Add(cart);
                    return Redirect(strURL);
                }
                else
                {
                    cart.Quantity += productNumber;
                    cart.Total += cart.Price * cart.Quantity;
                    return Redirect(strURL);
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult UpdateCart(FormCollection f)
        {
            try
            {
                List<Cart> lstCart = GetCart();
                String[] listQuantity = f["txtQuantity"].ToString().Split(',');
                for (int i = 0; i < lstCart.Count; i++)
                {
                    lstCart[i].Quantity = int.Parse(listQuantity[i]);
                    lstCart[i].Total = lstCart[i].Price * lstCart[i].Quantity;
                    if (lstCart[i].Quantity.ToString() == null)
                    {
                        lstCart[i].Quantity = 1;
                    }
                }
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
            }
            return RedirectToAction("Cart");
        }

        public ActionResult DeleteCart(int productId)
        {
            try
            {
                Product product = db.Products.SingleOrDefault(n => n.ProductId == productId);
                if (product == null)
                {
                    RedirectToAction("Index", "Error");
                    return null;
                }
                List<Cart> lstCart = GetCart();
                Cart cart = lstCart.Find(n => n.ProductId == productId);
                if (cart != null)
                {
                    lstCart.RemoveAll(n => n.ProductId == productId);
                }
                if (lstCart.Count == 0)
                {
                    return RedirectToAction("Index", "Product");
                }
                return RedirectToAction("Cart");
            }
            catch (Exception)
            {
                Response.StatusCode = 404;
            }
            return RedirectToAction("Cart");
        }

        public ActionResult DeleteAll()
        {
            Session["Cart"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Cart()
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<Cart> lstCart = GetCart();
            return View(lstCart);
        }

        private int SumQuantity()
        {
            int sumQuantity = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                sumQuantity = lstCart.Sum(n => n.Quantity);
            }
            return sumQuantity;
        }

        private double TotalPrice()
        {
            double totalPrice = 0;
            List<Cart> lstCart = Session["Cart"] as List<Cart>;
            if (lstCart != null)
            {
                totalPrice = lstCart.Sum(n => n.Total);
            }
            return totalPrice;
        }

        public ActionResult CartPartial()
        {
            if (SumQuantity() == 0)
            {
                return PartialView();
            }
            ViewBag.SumQuantity = SumQuantity();
            ViewBag.TotalQuantity = TotalPrice();
            return PartialView();
        }
        #endregion
        #region Order method

        public ActionResult GetOrderToCart(int orderId)
        {
            try
            {
                List<Cart> lstCart = new List<Cart>();
                Order order = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                var orderItems = db.OrderItems.Where(n => n.OrderId == orderId).ToList();
                if (lstCart != null)
                {
                    for (int i = 0; i < orderItems.Count; i++)
                    {
                        Cart cart = lstCart.Find(n => n.ProductId == orderItems[i].ProductId);
                        if (cart == null)
                        {
                            cart = new Cart(orderItems[i].ProductId);
                            cart.ProductId = orderItems[i].ProductId;
                            cart.ProductName = orderItems[i].Product.ProductName;
                            cart.Quantity = orderItems[i].Quantity;
                            cart.Price = orderItems[i].RealPrice;
                            cart.Total = orderItems[i].Amount;
                            lstCart.Add(cart);
                        }
                    }
                    Session["Cart"] = lstCart;
                    Session["BeEdited"] = order.OrderId;
                }
                return View(lstCart);
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }

        }

        public ActionResult EditOrder(FormCollection f, int orderId)
        {
            try
            {
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                Order order = db.Orders.Find(orderId);
                List<Cart> cart = GetCart();
                DateTime? deliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
                int totalValue = Convert.ToInt32(TempData["TotalValue"]);
                order.DeliveryTime = deliveryDate;
                TempData["deliveryDate"] = deliveryDate;
                //order.TotalValue = totalValue;
                if (Session["User"] != null)
                {
                    int cusUserId = Convert.ToInt32(Session["UserId"]);
                    order.CustomerUserId = cusUserId;
                    TempData["userName"] = Session["Username"].ToString();
                    TempData["phoneNumber"] = Session["Phonenumber"].ToString();
                }
                else
                {
                    RedirectToAction("Index", "Product");
                }
                TryUpdateModel(order);
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
                TempData["orderCode"] = order.OrderCode.ToString();
                db.SaveChanges();
                Session["Cart"] = null;
                Session["BeEdited"] = null;
                return RedirectToAction("EditOrderSuccess", "Cart");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
        }


        //For customer order
        public ActionResult OrderProduct(FormCollection f)
        {

            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Product");
            }
            Order order = new Order();
            List<Cart> cart = GetCart();
            string orderTime = DateTime.Now.ToString("yyyyMMdd");
            string orderNumber;
            DateTime? deliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
            int totalValue = Convert.ToInt32(TempData["TotalValue"]);
            order.OrderCode = orderTime;
            order.CreateTime = DateTime.Now;
            order.DeliveryTime = deliveryDate;
            Session["DeliveryDate"] = deliveryDate;
            order.DepositAmount = 0;
            order.IsStaffEdit = false;
            //order.Flag = false;
            order.OrderStatus = 0;
            //order.TotalValue = totalValue;
            if (Session["User"] != null)
            {
                int cusUserId = Convert.ToInt32(Session["UserId"]);
                order.CustomerUserId = cusUserId;
                TempData["userName"] = Session["Username"].ToString();
                TempData["phoneNumber"] = Session["Phonenumber"].ToString();
            }
            else
            {
                RedirectToAction("Index", "Product");
            }
            db.Orders.Add(order);
            db.SaveChanges();
            orderNumber = order.OrderId.ToString().PadLeft(4, '0');
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
            TempData["orderCode"] = order.OrderCode.ToString();
            db.SaveChanges();
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess", "Cart");


        }

        //For customer after enter information
        public ActionResult LoginOrderProduct(FormCollection f)
        {
            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Product");
            }
            Order order = new Order();
            List<Cart> cart = GetCart();
            string orderTime = DateTime.Now.ToString("yyyyMMdd");
            string orderNumber;
            int totalValue = Convert.ToInt32(TempData["TotalValue"]);
            string sAccount = f.Get("txtAccount").ToString();
            string sPassword = f.Get("txtPassword").ToString();
            User endUser = db.Users.SingleOrDefault(n => n.Username == sAccount && n.Password == sPassword);
            if (endUser != null)
            {
                Session["User"] = endUser;
                Session["Username"] = endUser.Username;
                Session["UserId"] = endUser.Customers.ElementAt(0).CustomerId;
                order.CustomerUserId = endUser.Customers.ElementAt(0).CustomerId;
                TempData["userName"] = endUser.Username.ToString();
                Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber.ToString();
            }
            else
            {
                ViewBag.Notify = "Sai tài khoản hoặc mật khẩu";
                return RedirectToAction("OrderInfo");
            }
            order.OrderCode = orderTime;
            order.CreateTime = DateTime.Now;
            order.DeliveryTime = (DateTime)Session["DeliveryDate"];
            order.DepositAmount = 0;
            order.IsStaffEdit = false;
            //order.Flag = false;
            order.OrderStatus = 0;
            //order.TotalValue = totalValue;

            //Lấy thông tin từ account, xét có thì login, add cusid

            db.Orders.Add(order);
            db.SaveChanges();
            orderNumber = order.OrderId.ToString().PadLeft(4, '0');
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
            TempData["orderCode"] = order.OrderCode.ToString();
            db.SaveChanges();
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess", "Cart");
        }

        public ActionResult GuestOrderProduct(FormCollection f)
        {
            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Product");
            }
            Order order = new Order();
            List<Cart> cart = GetCart();
            string orderTime = DateTime.Now.ToString("yyyyMMdd");
            string orderNumber;
            int totalValue = Convert.ToInt32(TempData["TotalValue"]);
            order.OrderCode = orderTime;
            order.CreateTime = DateTime.Now;
            order.DeliveryTime = (DateTime)Session["DeliveryDate"];
            order.DepositAmount = 0;
            order.IsStaffEdit = false;
            //order.Flag = false;
            order.OrderStatus = 0;
            //order.TotalValue = totalValue;

            // add guestinfo,guestid
            GuestInfo guestInfo = new GuestInfo();
            string sName = f.Get("txtName").ToString();
            string sPhone = f.Get("txtPhoneNumber").ToString();
            string sAddress = f.Get("txtAddress").ToString();
            string sEmail = f.Get("txtEmail").ToString();
            guestInfo.GuestInfoName = sName;
            guestInfo.GuestInfoPhone = sPhone;
            guestInfo.GuestInfoAddress = sAddress;
            guestInfo.GuestInfoEmail = sEmail;
            TempData["userName"] = sName;
            Session["Phonenumber"] = sPhone;
            if ((TempData["userName"]) != null && (Session["Phonenumber"]) != null && sAddress != null)
            {
                db.GuestInfoes.Add(guestInfo);
                db.SaveChanges();
            }
            else
            {
                ViewBag.Notify = "Sai tài khoản hoặc mật khẩu";
                return RedirectToAction("OrderInfo");
            }
            order.GuestInfoId = guestInfo.GuestInfoId;
            db.Orders.Add(order);
            db.SaveChanges();
            orderNumber = order.OrderId.ToString().PadLeft(4, '0');
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
            TempData["orderCode"] = order.OrderCode.ToString();
            db.SaveChanges();
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess", "Cart");
        }

        public ActionResult OrderInfo(FormCollection f)
        {
            try
            {
                DateTime? deliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
                if (Session["DeliveryDate"] == null)
                {
                    Session["DeliveryDate"] = deliveryDate;
                }
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
        }
        public ActionResult OrderSuccess()
        {
            return View();
        }

        public ActionResult EditOrderSuccess()
        {
            return View();
        }
        #endregion
    }
}