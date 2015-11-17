using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BMA.Models;
using BMA.Business;
using System.Net;
using System.Net.Mail;

namespace BMA.Controllers
{
    public class AccountController : Controller
    {
        BMAEntities db = new BMAEntities();
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

        [HttpGet]
        public ActionResult GetPassword()
        {
            return View();
        }

        [HttpPost]
        public int GetPassword(FormCollection f)
        {
            string email = f["txtEmail"].ToString();
            List<User> lstUser = db.Users.ToList();
            for (int i = 0; i < lstUser.Count; i++)
            {
                if (lstUser[i].Email == email)
                {
                    string password = "Tiembanhdautay";
                    string from = "tiembanh.dautaybma@gmail.com";
                    string to = "gaumapxipo@gmail.com";
                    MailMessage mail = new MailMessage();
                    mail.To.Add(to);
                    mail.From = new MailAddress(from);
                    mail.Subject = "This is test mail";
                    mail.Body = "Recovering the password";

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
            }
            return -1;
            //MailMessage mail = new MailMessage();
            //SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            //mail.From = new MailAddress("tiembanh.dautaybma@gmail.com");
            //mail.To.Add("gaumapxipo@gmail.com");
            //mail.Subject = "Password Recovery ";
            //mail.Body += " <html>";
            //mail.Body += "<body>";
            //mail.Body += "<table>";
            //mail.Body += "<tr>";
            //mail.Body += "<td>User Name : </td><td> HAi </td>";
            //mail.Body += "</tr>";

            //mail.Body += "<tr>";
            //mail.Body += "<td>Password : </td><td>aaaaaaaaaa</td>";
            //mail.Body += "</tr>";
            //mail.Body += "</table>";
            //mail.Body += "</body>";
            //mail.Body += "</html>";
            //mail.IsBodyHtml = true;
            //SmtpServer.Port = 587;
            //SmtpServer.Credentials = new System.Net.NetworkCredential("tiembanh.dautaybma@gmail.com", "Tiembanhdautay");
            //SmtpServer.EnableSsl = true;
            //SmtpServer.TargetName = "STARTTLS/smtp.gmail.com";
            //SmtpServer.Send(mail);
            //return 1;
        }
    }
}