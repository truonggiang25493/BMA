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
            Cart cart = lstCart.Find(n => n.productId == productId);
            if (cart == null)
            {
                cart = new Cart(productId);
                lstCart.Add(cart);
                return Redirect(strURL);
            }
            else
            {
                cart.quantity++;
                return Redirect(strURL);
            }       
        }
        [HttpPost]
        public ActionResult AddCart(FormCollection f)
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
                Cart cart = lstCart.Find(n => n.productId == productId);
                if (cart == null)
                {
                    cart = new Cart(productId);
                    cart.quantity = productNumber;
                    lstCart.Add(cart);
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    cart.quantity += productNumber;
                    return RedirectToAction("Index", "Product");
                }
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }         
        }

        public ActionResult UpdateCart(FormCollection f)
        {
            List<Cart> lstCart = GetCart();
            String[] listQuantity = f["txtQuantity"].ToString().Split(',');
            for (int i = 0; i < lstCart.Count; i++)
            {
                try
                {
                    lstCart[i].quantity = int.Parse(listQuantity[i]);
                }
                catch (Exception)
                {
                    
                    throw;
                }
                
            }
            return RedirectToAction("Cart");
        }

        public ActionResult DeleteCart(int productId)
        {
            Product product = db.Products.SingleOrDefault(n => n.ProductId == productId);
            if (product == null)
            {
                RedirectToAction("Index", "Error");
                return null;
            }
            List<Cart> lstCart = GetCart();
            Cart cart = lstCart.Find(n => n.productId == productId);
            if (cart != null)
            {
                lstCart.RemoveAll(n => n.productId == productId);
            }
            if (lstCart.Count == 0)
            {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Cart");
        }

        public ActionResult DeleteAll()
        {
            Session["Cart"] = null;
            Session.Clear();
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
                sumQuantity = lstCart.Sum(n => n.quantity);
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
    }
}