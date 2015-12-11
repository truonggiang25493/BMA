using System;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.WebPages;
using BMA.Business;
using BMA.Models;

namespace BMA.Controllers
{
    public class StaffController : Controller
    {
        private BMAEntities db = new BMAEntities();
        private StaffBusiness staffBusiness = new StaffBusiness();

        #region Get staff list

        public ActionResult StaffIndex()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "staff";
                ViewBag.TreeViewMenu = "listStaff";
                var stafflList = staffBusiness.GetStaffList();
                if (stafflList == null)
                {
                    RedirectToAction("StaffIndex", "Staff");
                }
                return View(stafflList);
            }
        }

        #endregion

        #region Get Staff Detail
        public ActionResult StaffDetail(int id)
        {
            User staffUser = Session["User"] as User;
            var uId = staffUser.UserId;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            //Login as Staff
            else
            {
                try
                {
                    ViewBag.TreeView = "staff";
                    Staff staffDetail = staffBusiness.GetStaffDetail(id);
                    if (staffDetail == null)
                    {
                        return RedirectToAction("StaffIndex", "Staff");

                    }
                    return View(staffDetail);
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "StoreInfor");
                }
            }
        }


        #endregion

        #region Change Staff Status

        [HttpPost]
        public int ChangeStaffStatus(int id)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return -7;
            }
            else
            {
                Boolean result = StaffBusiness.ChangeStaffStatus(id);
                if (result)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }

        #endregion

        #region Add New Staff View

        public ActionResult AddStaff()
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.TreeView = "staff";
                ViewBag.TreeViewMenu = "addStaff";
                return View("AddStaff");
            }
        }

        #endregion

        #region Add New Staff
        [HttpPost]
        public int AddStaff(FormCollection f)
        {
            AccountBusiness ab = new AccountBusiness();
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 1)
            {
                return -7;
            }
            else
            {
                String staffName = f["txtStaffName"];
                String staffUserAccount = f["txtStaffUserAccount"];
                String staffPhoneNumber = f["txtStaffPhoneNumber"];
                String staffEmail = f["txtStaffEmail"];
                String staffAddress = f["txtStaffAddress"];
                Staff staff = new Staff();
                User user = new User();

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var stringChars = new char[6];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                var finalString = new String(stringChars);

                try
                {
                    user.Fullname = staffName;
                    user.Username = staffUserAccount;
                    user.Email = staffEmail;
                    user.RoleId = 3;
                    user.Password = ab.CreateStaffPassword(finalString);
                    staff.StaffPhoneNumber = staffPhoneNumber;
                    staff.StaffAddress = staffAddress;
                    staff.IsActive = true;

                    staff.User = user;  
                    
                    string password = "Tiembanhdautay";
                    string from = "tiembanh.dautaybma@gmail.com";
                    string to = staffEmail;

                    MailMessage mail = new MailMessage();
                    mail.IsBodyHtml = true;
                    mail.To.Add(to);
                    mail.From = new MailAddress(from);
                    mail.Subject = string.Format("{0}{1}", "Tạo tài khoản cho nhân viên ", staffName);
                    mail.Body += "<html lang='vi'>";
                    mail.Body += "<head>";
                    mail.Body += "<meta charset='utf-8'>";
                    mail.Body += "</head>";
                    mail.Body += "<body>";
                    mail.Body += "<div> Bạn vừa được tạo tài khoản tại Tiệm Bánh Dâu Tây</div>";
                    mail.Body += string.Format("{0}{1}", "Tên tài khoản: ", staffUserAccount);
                    mail.Body+="<div></div>";
                    mail.Body += string.Format("{0}{1}", "Mật khẩu: ", finalString);
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
                             
                }
                catch (Exception)
                {
                    return 0;

                }

                bool result = StaffBusiness.AddStaff(staff);
                if (result)
                {
                    return 1;
                }
                else
                {
                    return 0;
                }
            }
        }
        #endregion

        #region Edit Staff View

        public ActionResult StaffEditInfo(int id)
        {
            User staffUser = Session["User"] as User;
            var uId = staffUser.UserId;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2 || uId != id)
            {
                return RedirectToAction("Index", "Manage");
            }
            else
            {
                ViewBag.TreeView = "staff";
                try
                {
                    Staff staff = db.Staffs.SingleOrDefault(m => m.UserId == id);
                    if (staff == null)
                    {
                        return RedirectToAction("Index", "Manage");
                    }
                    return View(staff);
                }
                catch (Exception)
                {
                    return RedirectToAction("Index", "StoreInfor");
                }
            }
        }

        #endregion

        #region Edit Staff

        [HttpPost]
        public int StaffEditInfo(FormCollection f)
        {
            User staffUser = Session["User"] as User;
            if (staffUser == null || Session["UserRole"] == null || (int)Session["UserRole"] != 2)
            {
                return -7;
            }
            else
            {
                String staffName = f["txtStaffName"];
                String userIdString = f["UserId"];
                String staffPhoneNumber = f["txtStaffPhoneNumber"];
                String staffEmail = f["txtStaffEmail"];
                String staffAddress = f["txtStaffAddress"];
                if (
                    !(staffName.IsEmpty() || userIdString.IsEmpty() ||
                      staffPhoneNumber.IsEmpty() || staffEmail.IsEmpty() ||
                      staffAddress.IsEmpty()))
                {
                    int userId = Convert.ToInt32(userIdString);

                    bool result = StaffBusiness.EditStaffInfo(userId, staffName, staffAddress, staffPhoneNumber,
                        staffEmail);
                    return result ? 1 : 0;
                }
                return 0;
            }
        }

        #endregion

        #region Check staff information is exsited in database when add

        [HttpPost]
        public int CheckStaffInfo(FormCollection form)
        {

            string staffUserAccount = form["txtStaffUserAccount"];
            string staffPhoneNumber = form["txtStaffPhoneNumber"];
            string staffEmail = form["txtStaffEmail"];


            return staffBusiness.CheckStaffInfo(staffUserAccount, staffPhoneNumber, staffEmail);
        }
        #endregion

        #region Check staff information is exsited in database when edit
        public int CheckStaffInfoinEdit(FormCollection form)
        {
            string staffPhoneNumber = form["txtStaffPhoneNumber"];
            string staffEmail = form["txtStaffEmail"];
            int userId = Convert.ToInt32(form["UserId"]);
            return staffBusiness.CheckStaffInfoInEdit(staffPhoneNumber, staffEmail, userId);
        }
        #endregion
    }
}