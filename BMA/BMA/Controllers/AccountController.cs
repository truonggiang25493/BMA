﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;

namespace BMA.Controllers
{
    public class AccountController : Controller
    {
        //
        // GET: /Account/
        BMAEntities db = new BMAEntities();
        public ActionResult Login(FormCollection f,string strURL)
        {
            try
            {
                string sAccount = f.Get("txtAccount").ToString();
                string sPassword = f.Get("txtPassword").ToString();
                AspNetUser endUser = db.AspNetUsers.SingleOrDefault(n => n.UserName == sAccount && n.PasswordHash == sPassword);
                if (endUser != null)
                {
                    Session["User"] = endUser;
                    Session["Username"] = endUser.UserName;
                    Session["UserId"] = endUser.Id;
                    Session["Phonenumber"] = endUser.PhoneNumber;
                    return RedirectToAction("Index", "Home");
                }
                ViewBag.Notify = "Sai tài khoản hoặc mật khẩu";
                return RedirectToAction("Login");
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