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
                if (endUser.RoleId == 3)
                {
                    Session["User"] = endUser;
                    Session["UserId"] = endUser.UserId;
                    Session["CusUserId"] = endUser.Customers.ElementAt(0).CustomerId;
                    Session["Phonenumber"] = endUser.Customers.ElementAt(0).CustomerPhoneNumber;
                    return 1;
                }
                else
                {
                    Session["User"] = endUser;
                    Session["UserRole"] = endUser.Role.RoleId;
                    return 2;
                }
            }
            catch
            {
                return -1;
            }
        }

        public ActionResult Logout()
        {
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
                        mail.Body += string.Format("<a href='{0}{1}{2}'>Tạo mới mật khẩu</a>", "http://", Request.Url.Authority, Url.Action("CreateNewPassword", "Account", new { userId = lstUser[i].UserId, timeSend = DateTime.Now }));
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
        public ActionResult CreateNewPassword(int userId, DateTime timeSend)
        {
            DateTime activeTimeCheck = DateTime.Now;
            double check = (activeTimeCheck - timeSend).TotalMinutes;
            if (check > 1440)
            {
                ViewBag.outOfTime = "";
            }
            ViewBag.userId = userId;
            return View();
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
    }
}