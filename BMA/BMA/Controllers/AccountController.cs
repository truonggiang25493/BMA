using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Business;
using System.Net;
using System.Net.Mail;
using System.Text;
using BMA.DBChangesNotifer;
using Microsoft.AspNet.SignalR;
using BMA.Hubs;

namespace BMA.Controllers
{
    public class AccountController : Controller
    {
        BMAEntities db = new BMAEntities();
        //
        // GET: /Account/
        [HttpPost]
        public int Login(FormCollection f, string strURL)
        {
            AccountBusiness ab = new AccountBusiness();
            try
            {
                string sAccount = f.Get("txtAccount");
                string sPassword = f.Get("txtPassword");
                User endUser = ab.checkLogin(sAccount, sPassword);
                if (endUser == null)
                {
                    return -1;
                }
                if (!endUser.IsConfirmed)
                {
                    ab.ConfirmAccount(endUser.UserId);
                }
                if (endUser.RoleId == 3)
                {
                    if (!endUser.Customers.ElementAt(0).IsActive)
                    {
                        return -2;
                    }
                    Session["User"] = endUser;
                    Session["UserId"] = endUser.UserId;
                    Session["CusUserId"] = endUser.Customers.ElementAt(0).CustomerId;
                    Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber;

                    //Check customer off notifier
                    List<Order> lstNewOrderNoty = ab.CustomerOffNewOrderNoty((int)Session["UserId"]);
                    List<Order> lstEditedOrderNoty = ab.CustomerOffEditedOrderNoty((int)Session["UserId"]);
                    List<Order> lstConfirmOrderNoty = ab.CustomerOffConfirmOrderNoty((int)Session["UserId"]);
                    if (lstNewOrderNoty != null)
                    {
                        Session["CusNewOrderCountPartial"] = Session["CusNotificateCount"] = lstNewOrderNoty.Count;
                        if (lstEditedOrderNoty != null)
                        {
                            Session["CusEditOrderCountPartial"] = lstEditedOrderNoty.Count;
                            Session["CusNotificateCount"] = lstNewOrderNoty.Count + lstEditedOrderNoty.Count;
                            if (lstConfirmOrderNoty != null)
                            {
                                Session["CusConfirmOrderCountPartial"] = lstConfirmOrderNoty.Count;
                                Session["CusNotificateCount"] = lstNewOrderNoty.Count + lstEditedOrderNoty.Count + lstConfirmOrderNoty.Count;
                            }
                        }
                        else
                        {
                            if (lstConfirmOrderNoty != null)
                            {
                                Session["CusConfirmOrderCountPartial"] = lstConfirmOrderNoty.Count;
                                Session["CusNotificateCount"] = lstNewOrderNoty.Count + lstConfirmOrderNoty.Count;
                            }
                        }
                    }
                    else
                    {
                        if (lstEditedOrderNoty != null)
                        {
                            Session["CusEditOrderCountPartial"] = Session["CusNotificateCount"] = lstEditedOrderNoty.Count;
                            if (lstConfirmOrderNoty != null)
                            {
                                Session["CusConfirmOrderCountPartial"] = lstConfirmOrderNoty.Count;
                                Session["CusNotificateCount"] = lstEditedOrderNoty.Count + lstConfirmOrderNoty.Count;
                            }
                        }
                        else
                        {
                            if (lstConfirmOrderNoty != null)
                            {
                                Session["CusConfirmOrderCountPartial"] = Session["CusNotificateCount"] = lstConfirmOrderNoty.Count;
                            }
                        }
                    }

                    //Open connection with this Customer
                    string dependencyCheckSql = string.Format("{0}{1}", "SELECT OrderStatus FROM dbo.[Orders] WHERE CustomerUserId=", endUser.UserId);
                    Session["CheckToNotify"] = endUser.UserId;
                    MvcApplication.changeStatusNotifer.Start("BMAChangeDB", dependencyCheckSql);
                    MvcApplication.changeStatusNotifer.Change += this.OnChange3;
                    return 1;
                }
                else
                {
                    if (endUser.RoleId == 2)
                    {
                        if (!endUser.Staffs.ElementAt(0).IsActive)
                        {
                            return -2;
                        }

                        MvcApplication.notifier.Dispose();
                        MvcApplication.notifier.Start("BMAChangeDB", "SELECT OrderId FROM dbo.[Orders]");
                        MvcApplication.notifier.Change += this.OnChange;

                        MvcApplication.lowQuantityNotifer.Dispose();
                        MvcApplication.lowQuantityNotifer.Start("BMAChangeDB", "SELECT ProductMaterialId,CurrentQuantity,StandardQuantity FROM dbo.[ProductMaterial] WHERE (CurrentQuantity < StandardQuantity AND IsActive = 'True')");
                        MvcApplication.lowQuantityNotifer.Change += this.OnChange2;

                        //Check staff off notifier
                        List<Order> lstNewOrderNoty = ab.StaffOffNewOrderNoty();
                        List<Order> lstConfirmOrderNoty = ab.StaffOffConfirmOrderNoty();
                        List<Order> lstCancelOrderNoty = ab.StaffOffCancelOrderNoty();
                        List<ProductMaterial> lstLowQuantityNoty = ab.StaffOffLowQuantityNoty();

                        if (lstNewOrderNoty != null)
                        {
                            Session["NewOrderCountPartial"] = Session["NotificateCount"] = lstNewOrderNoty.Count;
                            if (lstConfirmOrderNoty != null)
                            {
                                Session["ConfirmOrderCountPartial"] = lstConfirmOrderNoty.Count;
                                Session["NotificateCount"] = lstConfirmOrderNoty.Count + lstNewOrderNoty.Count;
                                if (lstCancelOrderNoty != null)
                                {
                                    Session["CancelOrderCountPartial"] = lstCancelOrderNoty.Count;
                                    Session["NotificateCount"] = lstCancelOrderNoty.Count + lstConfirmOrderNoty.Count + lstNewOrderNoty.Count;
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstCancelOrderNoty.Count + lstConfirmOrderNoty.Count + lstNewOrderNoty.Count;
                                    }
                                }
                                else
                                {
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstConfirmOrderNoty.Count + lstNewOrderNoty.Count;
                                    }
                                }
                            }
                            else
                            {
                                if (lstCancelOrderNoty != null)
                                {
                                    Session["CancelOrderCountPartial"] = lstCancelOrderNoty.Count;
                                    Session["NotificateCount"] = lstCancelOrderNoty.Count + lstNewOrderNoty.Count;
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstCancelOrderNoty.Count + lstNewOrderNoty.Count;
                                    }
                                }
                                else
                                {
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstNewOrderNoty.Count;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (lstConfirmOrderNoty != null)
                            {
                                Session["ConfirmOrderCountPartial"] = Session["NotificateCount"] = lstConfirmOrderNoty.Count;
                                if (lstCancelOrderNoty != null)
                                {
                                    Session["CancelOrderCountPartial"] = lstCancelOrderNoty.Count;
                                    Session["NotificateCount"] = lstCancelOrderNoty.Count + lstConfirmOrderNoty.Count;
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstCancelOrderNoty.Count + lstConfirmOrderNoty.Count;
                                    }
                                }
                                else
                                {
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstConfirmOrderNoty.Count;
                                    }
                                }
                            }
                            else
                            {
                                if (lstCancelOrderNoty != null)
                                {
                                    Session["CancelOrderCountPartial"] = Session["NotificateCount"] = lstCancelOrderNoty.Count;
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = lstLowQuantityNoty.Count;
                                        Session["NotificateCount"] = lstLowQuantityNoty.Count + lstCancelOrderNoty.Count;
                                    }
                                }
                                else
                                {
                                    if (lstLowQuantityNoty != null)
                                    {
                                        Session["LowMaterialCountPartial"] = Session["NotificateCount"] = lstLowQuantityNoty.Count;
                                    }
                                }
                            }
                        }

                    }
                    Session["User"] = endUser;
                    Session["UserId"] = endUser.UserId;
                    Session["UserRole"] = endUser.Role.RoleId;
                    return 2;
                }
            }
            catch
            {
                return -1;
            }
        }

        private void OnChange(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange(e.Info, e.Source, e.Type);
        }

        private void OnChange2(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange2(e.Info, e.Source, e.Type);
        }

        private void OnChange3(object sender, ChangeEventArgs e)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<RealtimeNotifierHub>();
            context.Clients.All.OnChange3(e.Info, e.Source, e.Type);
        }
        public ActionResult Logout()
        {
            try
            {
                AccountBusiness ab = new AccountBusiness();
                if (Session["CusUserId"] != null)
                {
                    MvcApplication.changeStatusNotifer.Dispose();
                    ab.SetLogoutTime((int)Session["UserId"]);
                }
                if (Session["UserRole"] != null)
                {
                    if ((int)Session["UserRole"] == 2)
                    {
                        ab.SetLogoutTime((int)Session["UserId"]);
                    }
                }
                Session["User"] = null;
                Session["BeEdited"] = null;
                Session.Clear();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpGet]
        public ActionResult GetPassword()
        {
            return View();
        }

        [HttpPost]
        public int GetPassword(FormCollection f)
        {
            try
            {
                AccountBusiness ab = new AccountBusiness();
                string username = f["txtUsername"].ToString();
                List<User> lstUser = db.Users.ToList();
                for (int i = 0; i < lstUser.Count; i++)
                {
                    if (lstUser[i].Username == username)
                    {
                        try
                        {
                            string password = "Tiembanhdautay";
                            string from = "tiembanh.dautaybma@gmail.com";
                            string to = lstUser[i].Email;

                            MailMessage mail = new MailMessage();
                            mail.IsBodyHtml = true;
                            mail.To.Add(to);
                            mail.From = new MailAddress(from);
                            mail.Subject = string.Format("{0}{1}", "Tạo mật khẩu mới cho khách hàng ", lstUser[i].Fullname);
                            mail.Body += "<html lang='vi'>";
                            mail.Body += "<head>";
                            mail.Body += "<meta charset='utf-8'>";
                            mail.Body += "</head>";
                            mail.Body += "<body>";
                            mail.Body += "<div> Quý khách vừa gởi yêu cầu tạo mật khẩu mới bằng Email này ?</div>";
                            mail.Body += "<div> Nếu phải, vui lòng bấm vào 'Tạo mới mật khẩu' bên dưới, đường dẫn chỉ có hiệu lực trong vòng 24 tiếng kể từ khi quý khách nhận được email này</div>";
                            //string link = Url.Encode(string.Format("{0}{1}", Request.Url.Authority, Url.Action("CreateNewPassword", "Account", new { userId = lstUser[i].UserId, timeSend = DateTime.Now })));
                            //mail.Body += string.Format("<a href='{0}{1}'>Tạo mới mật khẩu</a>", "http://", link);
                            mail.Body += string.Format("<a href='{0}{1}{2}'>Tạo mới mật khẩu</a>", "http://", Request.Url.Authority, Url.Action("CreateNewPassword", "Account", new { strUserId = ab.EncodeUserId(lstUser[i].UserId), timeSend = DateTime.Now }));
                            mail.Body += "</body>";
                            mail.Body += "</html>";
                            var mailBody = mail.Body;
                            var htmlBody = AlternateView.CreateAlternateViewFromString(mailBody, null, "text/html");
                            mail.AlternateViews.Add(htmlBody);

                            mail.Priority = MailPriority.High;
                            SmtpClient smtp = new SmtpClient();
                            smtp.UseDefaultCredentials = false;
                            smtp.Credentials = new System.Net.NetworkCredential(from, password);
                            smtp.Port = 587;
                            smtp.Host = "smtp.gmail.com";
                            smtp.EnableSsl = true;
                            smtp.Send(mail);
                            return 1;
                        }
                        catch
                        {
                            return -2;
                        }
                    }
                }
                return -1;
            }
            catch (Exception)
            {
                return -1;
            }
            
        }

        [HttpGet]
        public ActionResult CreateNewPassword(string strUserId, DateTime timeSend)
        {
            try
            {
                DateTime activeTimeCheck = DateTime.Now;
                AccountBusiness ab = new AccountBusiness();
                int userId = 0;
                string salt = strUserId.Substring(strUserId.Length - 88);
                List<User> lstUser = db.Users.ToList();
                foreach (var item in lstUser)
                {
                    if (ab.CreateIdHash(item.UserId, salt) == strUserId)
                    {
                        userId = item.UserId;
                    }
                }
                double check = (activeTimeCheck - timeSend).TotalMinutes;
                if (check > 1440)
                {
                    ViewBag.outOfTime = "";
                }

                if (userId != 0)
                {
                    ViewBag.userId = userId;
                    return View();
                }
                else
                {
                    return RedirectToAction("Index", "Error");
                }
            }
            catch
            {
                return RedirectToAction("Index", "Error");
            }
        }

        [HttpPost]
        public int CreateNewPassword(FormCollection f)
        {
            try
            {
                AccountBusiness ab = new AccountBusiness();
                int userId = Convert.ToInt32(f["userId"]);
                string newPassword = f["txtPass"];
                ab.ChangePassword(userId, newPassword);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public ActionResult ManageUserLoginPartialView()
        {
            try
            {
                if (Session["User"] != null)
                {
                    User staffUser = Session["User"] as User;
                    if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] == 3)
                    {
                        return null;
                    }
                    return PartialView();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }

        }
    }
}