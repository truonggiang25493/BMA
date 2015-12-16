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
            if (Session["CusUserId"] != null)
            {
                MvcApplication.changeStatusNotifer.Dispose();
            }
            Session["User"] = null;
            Session["BeEdited"] = null;
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult GetPassword()
        {
            return View();
        }

        [HttpPost]
        public int GetPassword(FormCollection f)
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

        [HttpGet]
        public ActionResult CreateNewPassword(string strUserId, DateTime timeSend)
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

        [HttpPost]
        public int CreateNewPassword(FormCollection f)
        {
            AccountBusiness ab = new AccountBusiness();
            int userId = Convert.ToInt32(f["userId"]);
            string newPassword = f["txtPass"];
            try
            {
                ab.ChangePassword(userId, newPassword);
                return 1;
            }
            catch
            {
                return -1;
            }
        }

        public ActionResult ManageUserLogin()
        {
            try
            {
                User staffUser = Session["User"] as User;
                if (staffUser == null || Session["UserRole"] == null || (int) Session["UserRole"] == 3)
                {
                    return RedirectToAction("Index", "Home");
                }
                return PartialView();
            }
            catch (Exception)
            {
                return RedirectToAction("Index", "Error");
            }
           
        }
    }
}