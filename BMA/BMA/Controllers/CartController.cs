using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Models.ViewModel;
using BMA.Business;

namespace BMA.Controllers
{
    public class CartController : Controller
    {
        #region Cart method
        public List<CustomerCartViewModel> GetCart()
        {
            List<CustomerCartViewModel> lstCart = Session["Cart"] as List<CustomerCartViewModel>;
            if (lstCart == null)
            {
                lstCart = new List<CustomerCartViewModel>();
                Session["Cart"] = lstCart;
            }
            return lstCart;
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

                List<CustomerCartViewModel> lstCart = GetCart();
                CustomerCartViewModel cart = lstCart.Find(n => n.ProductId == productId);
                if (cart == null)
                {
                    cart = new CustomerCartViewModel(productId);
                    cart.ProductId = productId;
                    cart.Quantity = productNumber;
                    cart.Total = cart.Price * cart.Quantity;
                    lstCart.Add(cart);
                    return Redirect(strURL);
                }
                else
                {
                    cart.Quantity += productNumber;
                    if (cart.Quantity > 9999)
                    {
                        cart.Quantity = 9999;
                    }
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
                List<CustomerCartViewModel> lstCart = GetCart();
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
                List<CustomerCartViewModel> lstCart = GetCart();
                CustomerCartViewModel cart = lstCart.Find(n => n.ProductId == productId);
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
            CustomerOrberBusiness cob = new CustomerOrberBusiness();
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.taxRate = cob.GetTaxRate();
            List<CustomerCartViewModel> lstCart = GetCart();
            return View(lstCart);
        }

        private int SumQuantity()
        {
            int sumQuantity = 0;
            List<CustomerCartViewModel> lstCart = Session["Cart"] as List<CustomerCartViewModel>;
            if (lstCart != null)
            {
                sumQuantity = lstCart.Sum(n => n.Quantity);
            }
            return sumQuantity;
        }

        private double TotalPrice()
        {
            double totalPrice = 0;
            List<CustomerCartViewModel> lstCart = Session["Cart"] as List<CustomerCartViewModel>;
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
                CustomerOrberBusiness cob = new CustomerOrberBusiness();
                List<CustomerCartViewModel> lstCart = new List<CustomerCartViewModel>();
                lstCart = cob.GetOrderToCart(orderId);
                if (lstCart != null)
                {
                    Session["Cart"] = lstCart;
                    Session["BeEdited"] = orderId;
                }
                return RedirectToAction("Cart");
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
                CustomerOrberBusiness cob = new CustomerOrberBusiness();
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                List<CustomerCartViewModel> cart = GetCart();
                DateTime planDeliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
                int amount = Convert.ToInt32(TempData["Amount"]);
                int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
                int cusUserId = Convert.ToInt32(Session["UserId"]);
                if (Session["User"] != null)
                {
                    TempData["userName"] = (Session["User"] as User).Username;
                    TempData["phoneNumber"] = Session["Phonenumber"].ToString();
                }
                else
                {
                    RedirectToAction("Index", "Product");
                }
                cob.EditOrder(orderId, planDeliveryDate, amount, taxAmount, cusUserId, cart);
                TempData["orderCode"] = cob.EditGetOrderCode(orderId);
                TempData["deliveryDate"] = planDeliveryDate;
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
            //try
            //{
                CustomerOrberBusiness cob = new CustomerOrberBusiness();
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                List<CustomerCartViewModel> cart = GetCart();
                string orderTime = DateTime.Now.ToString("yyyyMMdd");
                DateTime planDeliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
                int amount = Convert.ToInt32(TempData["Amount"]);
                int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
                int cusUserId = Convert.ToInt32(Session["UserId"]);
                if (Session["User"] != null)
                {
                    TempData["userName"] = (Session["User"] as User).Username;
                    TempData["phoneNumber"] = Session["Phonenumber"].ToString();
                }
                else
                {
                    RedirectToAction("Index", "Product");
                }
                cob.OrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cusUserId, cart);
                Session["DeliveryDate"] = planDeliveryDate;
                TempData["orderCode"] = cob.GetOrderCode();
                Session["Cart"] = null;
                return RedirectToAction("OrderSuccess", "Cart");
            //}
            //catch
            //{
            //    return RedirectToAction("Index", "Error");
            //}
        }

        //For customer after enter information
        public ActionResult LoginOrderProduct(FormCollection f)
        {
            try
            {
                CustomerOrberBusiness cob = new CustomerOrberBusiness();
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                List<CustomerCartViewModel> cart = GetCart();
                string orderTime = DateTime.Now.ToString("yyyyMMdd");
                int amount = Convert.ToInt32(TempData["Amount"]);
                int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
                string sAccount = f.Get("txtAccount").ToString();
                string sPassword = f.Get("txtPassword").ToString();
                User endUser = AccountBusiness.checkLogin(sAccount, sPassword);
                if (endUser != null)
                {
                    Session["User"] = endUser;
                    Session["UserId"] = endUser.Customers.ElementAt(0).CustomerId;
                    TempData["userName"] = endUser.Username.ToString();
                    Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber.ToString();
                }
                else
                {
                    TempData["Notify"] = "Sai tài khoản hoặc mật khẩu";
                    return RedirectToAction("OrderInfo");
                }
                int cusUserId = Convert.ToInt32(Session["UserId"]);
                DateTime planDeliveryDate = Convert.ToDateTime(Session["DeliveryDate"]);
                cob.OrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cusUserId, cart);
                TempData["orderCode"] = cob.GetOrderCode();
                Session["Cart"] = null;
                return RedirectToAction("OrderSuccess", "Cart");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult GuestOrderProduct(FormCollection f)
        {
            try
            {
                CustomerOrberBusiness cob = new CustomerOrberBusiness();
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                List<CustomerCartViewModel> cart = GetCart();
                string orderTime = DateTime.Now.ToString("yyyyMMdd");
                int amount = Convert.ToInt32(TempData["Amount"]);
                int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
                DateTime planDeliveryDate = Convert.ToDateTime(Session["DeliveryDate"]);
                string sName = f.Get("txtName").ToString();
                string sPhone = f.Get("txtPhoneNumber").ToString();
                string sAddress = f.Get("txtAddress").ToString();
                string sEmail = f.Get("txtEmail").ToString();
                if (sName == null || sPhone == null || sAddress == null || sEmail == null)
                {
                    RedirectToAction("OrderInfo");
                }
                cob.GuestOrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cart, sName, sPhone, sAddress, sEmail);
                TempData["userName"] = sName;
                Session["Phonenumber"] = sPhone;
                TempData["orderCode"] = cob.GetOrderCode();
                Session["Cart"] = null;
                return RedirectToAction("OrderSuccess", "Cart");
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
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