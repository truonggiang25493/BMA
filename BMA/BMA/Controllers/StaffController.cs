﻿using System;
using System.Linq;
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

        #region Get supplier list
        public ActionResult StaffIndex()
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
        #endregion

        #region Get Staff Detail
        public ActionResult StaffDetail(int id)
        {
            ViewBag.TreeView = "staff";
            Staff staffDetail = staffBusiness.GetStaffDetail(id);
            if (staffDetail == null)
            {
                RedirectToAction("StaffIndex", "Staff");

            }
            return View(staffDetail);

        }

        #endregion

        #region Change Staff Status
        [HttpPost]
        public int ChangeStaffStatus(int id)
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
        #endregion

        #region Add New Staff View
        public ActionResult AddStaff()
        {
            ViewBag.TreeView = "staff";
            ViewBag.TreeViewMenu = "addStaff";
            return View("AddStaff");
        }
        #endregion

        #region Add New Staff
        [HttpPost]
        public int AddStaff(FormCollection f)
        {

            String staffName = f["txtStaffName"];
            String staffUserAccount = f["txtStaffUserAccount"];
            String staffPhoneNumber = f["txtStaffPhoneNumber"];
            String staffEmail = f["txtStaffEmail"];
            String staffAddress = f["txtStaffAddress"];
            Staff staff = new Staff();
            User user = new User();
            try
            {
                user.Fullname = staffName;
                user.Username = staffUserAccount;
                user.Email = staffEmail;
                user.RoleId = 3;
                user.Password = Convert.ToString(123456);
                staff.StaffPhoneNumber = staffPhoneNumber;
                staff.StaffAddress = staffAddress;
                staff.IsActive = true;

                staff.User = user;

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
        #endregion

        #region Edit Staff View
        public ActionResult StaffEditInfo(int id)
        {
            ViewBag.TreeView = "staff";
            Staff staff = db.Staffs.SingleOrDefault(m => m.UserId == id);
            return View(staff);
        }
        #endregion

        #region Edit Staff
        [HttpPost]
        public int StaffEditInfo(FormCollection f)
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

                bool result = StaffBusiness.EditStaffInfo(userId, staffName, staffAddress, staffPhoneNumber, staffEmail);
                return result ? 1 : 0;
            }
            return 0;
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
            int staffId = Convert.ToInt32(form["#StaffId"]);
            return staffBusiness.CheckStaffInfoInEdit(staffPhoneNumber, staffEmail, staffId);
        }
        #endregion
    }
}