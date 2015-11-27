using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Models.ViewModel;
using BMA.Business;
using System.Globalization;

namespace BMA.Controllers
{
    public class CartController : Controller
    {
        BMAEntities db = new BMAEntities();
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
                List<CustomerCartViewModel> lstCart = GetCart();
                CustomerCartViewModel product = lstCart.Find(n => n.ProductId == productId);
                if (product == null)
                {
                    product = new CustomerCartViewModel(productId);
                    product.ProductId = productId;
                    product.ProductImage = productImage;
                    product.Quantity = productNumber;
                    product.Total = product.Price * product.Quantity;
                    lstCart.Add(product);
                    ViewBag.lstProductCart = lstCart;
                }
                else
                {
                    product.Quantity = productNumber;
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
                int checkQuantity = 0;
                List<CustomerCartViewModel> lstCart = GetCart();
                foreach (var item in lstCart)
                {
                    checkQuantity += item.Quantity;
                }
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
            return RedirectToAction("Index", "Product");
        }

        public ActionResult Cart()
        {
            try
            {
                CustomerOrderBusiness cob = new CustomerOrderBusiness();
                int quantity = 0;
                if (Session["Cart"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.taxRate = cob.GetTaxRate();
                List<CustomerCartViewModel> lstCart = GetCart();
                if (lstCart.Count == 0)
                {
                    return RedirectToAction("Index", "Product");
                }
                List<DiscountByQuantity> dbq = db.DiscountByQuantities.ToList();
                Policy policy = db.Policies.SingleOrDefault(n => n.PolicyId == 1);
                ViewBag.policy = policy;
                foreach (var item in lstCart)
                {
                    quantity += item.Quantity;
                }
                if (dbq[0].beUsing)
                {
                    TempData["Discount"] = cob.checkDiscount(quantity);
                    return View(lstCart);
                }
                else
                {
                    return View(lstCart);
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
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
                cob.TurnFlagOn(orderId);
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

        public int EditOrder(int orderId)
        {
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            if (Session["Cart"] == null)
            {
                return -1;
            }
            List<CustomerCartViewModel> cart = GetCart();
            foreach (var item in cart)
            {
                if (!cob.IsActiveProduct(item.ProductId))
                {
                    return -2;
                }
            }
            //DateTime planDeliveryDate = Convert.ToDateTime(f.Get("txtDeliveryDate"));
            DateTime planDeliveryDate = DateTime.ParseExact(Session["DeliveryDate"].ToString(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            int amount = Convert.ToInt32(Session["Amount"]);
            int taxAmount = Convert.ToInt32(Session["TaxAmount"]);
            int discount = Convert.ToInt32(Session["DiscountAmount"]);
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
            cob.EditOrder(orderId, planDeliveryDate, amount, taxAmount, discount, cusUserId, cart);
            cob.TurnFlagOff(orderId);
            TempData["orderCode"] = cob.EditGetOrderCode(orderId);
            Session["DeliveryDate"] = planDeliveryDate;
            Session["Cart"] = null;
            Session["BeEdited"] = null;
            Session["Amount"] = null;
            Session["TaxAmount"] = null;
            Session["DiscountAmount"] = null;
            return 1;
        }


        //For customer order
        public int OrderProduct()
        {
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            if (Session["Cart"] == null)
            {
                return -1;
            }
            List<CustomerCartViewModel> cart = GetCart();
            string orderTime = DateTime.Now.ToString("yyyyMMdd");
            //DateTime planDeliveryDate = DateTime.Now;
            DateTime planDeliveryDate = DateTime.ParseExact(Session["DeliveryDate"].ToString(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            int amount = Convert.ToInt32(Session["Amount"]);
            int taxAmount = Convert.ToInt32(Session["TaxAmount"]);
            int discount = Convert.ToInt32(Session["DiscountAmount"]);
            int cusUserId = Convert.ToInt32(Session["UserId"]);
            if (Session["User"] != null)
            {
                TempData["userName"] = (Session["User"] as User).Username;
                TempData["phoneNumber"] = Session["Phonenumber"].ToString();
            }
            else
            {
                return -1;
            }
            foreach (var item in cart)
            {
                if (!cob.IsActiveProduct(item.ProductId))
                {
                    return -2;
                }
            }
            cob.OrderProduct(orderTime, planDeliveryDate, amount, taxAmount, discount, cusUserId, cart);
            TempData["orderCode"] = cob.GetOrderCode();
            Session["Cart"] = null;
            Session["Amount"] = null;
            Session["TaxAmount"] = null;
            Session["DiscountAmount"] = null;
            return 1;

        }

        //For customer after enter information
        public int LoginOrderProduct(FormCollection f)
        {
            AccountBusiness ab = new AccountBusiness();
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            if (Session["Cart"] == null)
            {
                return -4;
            }
            List<CustomerCartViewModel> cart = GetCart();
            string orderTime = DateTime.Now.ToString("yyyyMMdd");
            int amount = Convert.ToInt32(Session["Amount"]);
            int taxAmount = Convert.ToInt32(Session["TaxAmount"]);
            int discount = Convert.ToInt32(Session["DiscountAmount"]);
            string sAccount = f.Get("txtAccount").ToString();
            string sPassword = f.Get("txtPassword").ToString();
            User endUser = ab.checkLogin(sAccount, sPassword);
            if (endUser != null)
            {
                int checkRole = endUser.RoleId;
                if (checkRole != 3)
                {
                    TempData["Notify"] = "Tài khoản không hợp lệ";
                    return -2;
                }
                Session["User"] = endUser;
                Session["UserId"] = endUser.UserId;
                Session["CusUserId"] = endUser.Customers.ElementAt(0).CustomerId;
                TempData["userName"] = endUser.Username.ToString();
                Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber.ToString();
            }
            else
            {
                TempData["Notify"] = "Sai tài khoản hoặc mật khẩu";
                return -3;
            }
            int cusUserId = Convert.ToInt32(Session["UserId"]);
            DateTime planDeliveryDate = DateTime.ParseExact(Session["DeliveryDate"].ToString(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
            cob.OrderProduct(orderTime, planDeliveryDate, amount, taxAmount, discount, cusUserId, cart);
            TempData["orderCode"] = cob.GetOrderCode();
            Session["Cart"] = null;
            Session["Amount"] = null;
            Session["TaxAmount"] = null;
            Session["DiscountAmount"] = null;
            return 1;

        }

        public int GuestOrderProduct(FormCollection f)
        {
            try
            {
                CustomerOrderBusiness cob = new CustomerOrderBusiness();
                if (Session["Cart"] == null)
                {
                    return -1;
                }
                List<CustomerCartViewModel> cart = GetCart();
                string orderTime = DateTime.Now.ToString("yyyyMMdd");
                int amount = Convert.ToInt32(Session["Amount"]);
                int taxAmount = Convert.ToInt32(Session["TaxAmount"]);
                int discount = Convert.ToInt32(Session["DiscountAmount"]);
                DateTime planDeliveryDate = DateTime.ParseExact(Session["DeliveryDate"].ToString(), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture);
                string sName = f.Get("txtName").ToString();
                string sPhone = f.Get("txtPhoneNumber").ToString();
                string sAddress = f.Get("txtAddress").ToString();
                string sEmail = f.Get("txtEmail").ToString();

                if (sName == null || sPhone == null || sAddress == null || sEmail == null)
                {
                    RedirectToAction("OrderInfo");
                }

                if (cob.checkStaffDuplicate(sEmail, sPhone))
                {
                    return -2;
                }
                //Kiểm tra guest đó có tồn tại trong dtb user hoặc guest chưa
                if (cob.checkCustomerDuplicate(sEmail, sPhone))
                {
                    return -3;
                }

                //if (cob.checkGuestDuplicate(sName, sPhone, sEmail))
                //{
                //    cob.GuestOrderProduct(orderTime, planDeliveryDate, amount, taxAmount, cart, sName, sPhone, sEmail);
                //}
                else
                {
                    cob.GuestOrderProduct(orderTime, planDeliveryDate, amount, taxAmount, discount, cart, sName, sPhone, sAddress, sEmail);
                }
                TempData["orderCode"] = cob.GetOrderCode();
                Session["Cart"] = null;
                Session["Amount"] = null;
                Session["TaxAmount"] = null;
                Session["DiscountAmount"] = null;
                return 1;
            }
            catch
            {
                return -4;
            }

        }

        public ActionResult ProceedCheckout(FormCollection f)
        {
            try
            {
                CustomerOrderBusiness cob = new CustomerOrderBusiness();
                int quantity = 0;
                if (Session["Cart"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.taxRate = cob.GetTaxRate();
                string planDeliveryDate = f["txtDelivery"].ToString();
                Session["DeliveryDate"] = planDeliveryDate;
                List<CustomerCartViewModel> lstCart = GetCart();
                foreach (var item in lstCart)
                {
                    if (!cob.IsActiveProduct(item.ProductId))
                    {
                        ViewBag.productName = item.ProductName;
                    }
                }
                if (lstCart.Count == 0)
                {
                    return RedirectToAction("Index", "Product");
                }
                List<Boolean> dbq = db.DiscountByQuantities.Select(n => n.beUsing).ToList();
                foreach (var item in lstCart)
                {
                    quantity += item.Quantity;
                }
                if (dbq[0])
                {
                    TempData["Discount"] = cob.checkDiscount(quantity);
                }
                else
                {

                }
                return View(lstCart);
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public int CheckProductActive()
        {
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            List<CustomerCartViewModel> lstCart = GetCart();
            foreach (var item in lstCart)
            {
                if (!cob.IsActiveProduct(item.ProductId))
                {
                    return -1;
                }
            }
            return 1;
        }
        public ActionResult OrderInfo(FormCollection f)
        {
            if (Session["Cart"] == null)
            {
                return RedirectToAction("Index", "Product");
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

        public ActionResult EditOrderSuccess(int orderId)
        {
            CustomerOrderBusiness cob = new CustomerOrderBusiness();
            var lstOrderItems = cob.EditOrderSuccess(orderId);
            return View(lstOrderItems);
        }
        #endregion
    }
}