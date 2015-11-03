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
                string productImage = f["productImage"].ToString();
                int productNumber = int.Parse(f["txtQuantity"].ToString());
                int checkQuantity = 0;

                List<CustomerCartViewModel> lstCart = GetCart();
                CustomerCartViewModel product = lstCart.Find(n => n.ProductId == productId);
                if (product == null)
                {
                    product = new CustomerCartViewModel(productId);
                    product.ProductId = productId;
                    product.ProductImage = productImage;
                    product.Quantity = productNumber;
                    product.Total = product.Price * product.Quantity;

                    //Kiểm tra tổng sp giỏ hàng đã lớn hơn 10k hay chưa
                    foreach (var item in lstCart)
                    {
                        checkQuantity += item.Quantity;
                    }
                    if ((checkQuantity + productNumber) > 10000)
                    {
                        return Redirect(strURL);
                    }

                    lstCart.Add(product);
                    ViewBag.lstProductCart = lstCart;
                }
                else
                {
                    product.Quantity = productNumber;
                    if (product.Quantity > 5000)
                    {
                        product.Quantity = 5000;
                    }
                    //Kiểm tra tổng sp giỏ hàng đã lớn hơn 10k hay chưa
                    foreach (var item in lstCart)
                    {
                        checkQuantity += item.Quantity;
                    }
                    if ((checkQuantity + productNumber) > 10000)
                    {
                        return Redirect(strURL);
                    }

                    product.Total = product.Price * product.Quantity;
                    ViewBag.lstProductCart = lstCart;
                    return Redirect(strURL);
                }

                return Redirect(strURL);
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
                String[] listQuantity = f["quanitySniper"].ToString().Split(',');
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
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            int quantity = 0;
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.taxRate = cob.GetTaxRate();
            List<CustomerCartViewModel> lstCart = GetCart();
            //foreach (var item in lstCart)
            //{
            //    quantity += item.Quantity;
            //}
            //ViewBag.discount = cob.checkDiscount(quantity);
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
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            List<CustomerCartViewModel> lstProductCart = GetCart();
            ViewBag.SumQuantity = lstProductCart.Count;
            ViewBag.lstProductCart = lstProductCart;
            return PartialView();
        }
        #endregion
        #region Order method

        public ActionResult GetOrderToCart(int orderId)
        {
            try
            {
                CustomerOrderBusiness cob = new CustomerOrderBusiness();
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
                CustomerOrderBusiness cob = new CustomerOrderBusiness();
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                List<CustomerCartViewModel> cart = GetCart();
                DateTime planDeliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
                int amount = Convert.ToInt32(TempData["Amount"]);
                int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
                int cusUserId = Convert.ToInt32(Session["CusUserId"]);
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
        public ActionResult OrderProduct()
        {
            //try
            //{
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            if (Session["Cart"] == null)
            {
                RedirectToAction("Index", "Product");
            }
            List<CustomerCartViewModel> cart = GetCart();
            string orderTime = DateTime.Now.ToString("yyyyMMdd");
            //DateTime planDeliveryDate = DateTime.Parse(Session["DeliveryDate"].ToString());
            DateTime planDeliveryDate = DateTime.Now;
            int amount = Convert.ToInt32(TempData["Amount"]);
            int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
            int cusUserId = Convert.ToInt32(Session["CusUserId"]);
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
            AccountBusiness ab = new AccountBusiness();
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
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
            User endUser = ab.checkLogin(sAccount, sPassword);
            if (endUser != null)
            {
                Session["User"] = endUser;
                Session["UserId"] = endUser.UserId;
                Session["CusUserId"] = endUser.Customers.ElementAt(0).CustomerId;
                TempData["userName"] = endUser.Username.ToString();
                Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber.ToString();
            }
            else
            {
                TempData["Notify"] = "Sai tài khoản hoặc mật khẩu";
                return RedirectToAction("OrderInfo");
            }
            int cusUserId = Convert.ToInt32(Session["CusUserId"]);
            //DateTime planDeliveryDate = Convert.ToDateTime(Session["DeliveryDate"]);
            DateTime planDeliveryDate = DateTime.Now;
            cob.OrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cusUserId, cart);
            Session["DeliveryDate"] = planDeliveryDate;
            TempData["orderCode"] = cob.GetOrderCode();
            Session["Cart"] = null;
            return RedirectToAction("OrderSuccess", "Cart");

        }

        public ActionResult GuestOrderProduct(FormCollection f)
        {
            
                CustomerOrderBusiness cob = new CustomerOrderBusiness();
                if (Session["Cart"] == null)
                {
                    RedirectToAction("Index", "Product");
                }
                List<CustomerCartViewModel> cart = GetCart();
                string orderTime = DateTime.Now.ToString("yyyyMMdd");
                int amount = Convert.ToInt32(TempData["Amount"]);
                int taxAmount = Convert.ToInt32(TempData["TaxAmount"]);
                //DateTime planDeliveryDate = Convert.ToDateTime(Session["DeliveryDate"]);
                DateTime planDeliveryDate = DateTime.Now;
                string sName = f.Get("txtName").ToString();
                string sPhone = f.Get("txtPhoneNumber").ToString();
                string sAddress = f.Get("txtAddress").ToString();
                string sEmail = f.Get("txtEmail").ToString();

                if (sName == null || sPhone == null || sAddress == null || sEmail == null)
                {
                    RedirectToAction("OrderInfo");
                }
                //Kiểm tra guest đó có tồn tại trong dtb user hoặc guest chưa
                if (cob.checkUserDuplicate(sEmail, sPhone))
                {
                    TempData["guestError"] = "Email và số điện thoại đã tồn tại, nếu đã có tài khoản xin vui lòng đăng nhập.";
                    return RedirectToAction("OrderInfo");
                }

                //if (cob.checkGuestDuplicate(sName, sPhone, sEmail))
                //{
                //    cob.GuestOrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cart, sName, sPhone, sEmail);
                //}
                else
                {
                    cob.GuestOrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cart, sName, sPhone, sAddress, sEmail);
                }

                Session["DeliveryDate"] = planDeliveryDate;
                TempData["orderCode"] = cob.GetOrderCode();
                Session["Cart"] = null;
                return RedirectToAction("OrderSuccess", "Cart");
         
        }

        public ActionResult ProceedCheckout(FormCollection f)
        {
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            int quantity = 0;
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.taxRate = cob.GetTaxRate();
            List<CustomerCartViewModel> lstCart = GetCart();
            //foreach (var item in lstCart)
            //{
            //    quantity += item.Quantity;
            //}
            //ViewBag.discount = cob.checkDiscount(quantity);
            return View(lstCart);
        }
        public ActionResult OrderInfo(FormCollection f)
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            try
            {
                return View();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
        }
        public ActionResult OrderSuccess()
        {
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            var lstOrderItems = cob.OrderSuccess();
            return View(lstOrderItems);
        }

        public ActionResult EditOrderSuccess()
        {
            return View();
        }
        #endregion
    }
}