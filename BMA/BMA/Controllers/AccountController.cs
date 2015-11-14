using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Business;

namespace BMA.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        [HttpPost]
        public ActionResult Login(FormCollection f, string strURL)
        {
            AccountBusiness ab = new AccountBusiness();
            try
            {
                string sAccount = f.Get("txtAccount");
                string sPassword = f.Get("txtPassword");
                User endUser = ab.checkLogin(sAccount, sPassword);
                if (endUser.RoleId == 3)
                {
                    Session["User"] = endUser;
                    Session["UserId"] = endUser.UserId;
                    Session["CusUserId"] = endUser.Customers.ElementAt(0).CustomerId;
                    Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber;
                    return RedirectToAction("Index", "Product");
                }
                else
                {
                    Session["User"] = endUser;
                    Session["UserRole"] = endUser.Role.RoleId;
                    return RedirectToAction("Index", "StoreInfor");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        public ActionResult Logout()
        {
            Session["User"] = null;
            Session["BeEdited"] = null;
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}