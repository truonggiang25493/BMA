using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Business;

namespace BMA.Controllers
{
    public class ManageController : Controller
    {
        // GET: Manage
        public ActionResult Index(int userId)
        {
            try
            {
                if (Session["User"] == null || Session["UserRole"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                if ((int)Session["UserId"] != userId)
                {
                    return RedirectToAction("Index", "StoreInfor");
                }
                AccountBusiness ab = new AccountBusiness();
                var user = ab.GetUser(userId);
                return View(user);
            }
            catch
            {
                return RedirectToAction("ManageError", "Error");
            }
        }

        public int EditInformation(FormCollection f)
        {
            try
            {
                AccountBusiness ab = new AccountBusiness();
                int userId = Convert.ToInt32(Session["UserId"]);
                string sName = f["customerName"];
                string sEmail = f["customerEmail"];
                if ((int)Session["UserRole"] == 1)
                {
                    if (ab.checkEmailExisted(userId, sEmail))
                    {
                        return -1;
                    }
                    ab.ChangeInformation(userId, sName, sEmail, null,null,null);
                }
                if ((int)Session["UserRole"] == 2)
                {
                    if (ab.checkEmailExisted(userId, sEmail))
                    {
                        return -1;
                    }
                    string sAddress = f["customerAddress"];
                    string sPhone = f["customerPhoneNumber"];
                    if (ab.checkPhoneExisted(userId, sPhone))
                    {
                        return -2;
                    }
                    ab.ChangeInformation(userId, sName, sEmail, sAddress, null, sPhone);
                }
                return 1;
            }
            catch
            {
                return -3;
            }
        }

        public int ChangePassword(FormCollection f)
        {
            AccountBusiness ab = new AccountBusiness();
            int cusUserId = Convert.ToInt32(Session["UserId"]);
            string sOldPass = f["txtOldPass"];
            string sNewPass = f["txtNewPass"];
            string sNewPassConfirm = f["txtNewPassConfirm"];
            if (!ab.checkPass(cusUserId, sOldPass))
            {
                return -1;
            }
            if (sOldPass == sNewPass)
            {
                return -2;
            }
            if (sNewPass != sNewPassConfirm)
            {
                return -3;
            }
            ab.ChangePassword(cusUserId, sNewPass);
            return 1;
        }
    }
}