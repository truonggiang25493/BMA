using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class StaffBusiness
    {
        private static BMAEntities db = new BMAEntities();

        #region Get staff list
        public List<Staff> GetStaffList()
        {
            List<Staff> staffList = db.Staffs.OrderBy(n => n.IsActive).ThenByDescending(n=>n.StaffId).ToList();
            return staffList;
        }
        #endregion

        #region View staff detail
        public Staff GetStaffDetail(int id)
        {
            Staff staffDetail = db.Staffs.SingleOrDefault(n => n.StaffId == id);
            return staffDetail;
        }
        #endregion

        #region Change staff status
        public static bool ChangeStaffStatus(int id)
        {
            var staffDetail = db.Staffs.SingleOrDefault(n => n.StaffId == id);
            if (staffDetail != null)
            {
                if (staffDetail.IsActive)
                {
                    staffDetail.IsActive = false;

                }
                else
                {
                    staffDetail.IsActive = true;
                }
                try
                {
                    db.SaveChanges();
                }
                catch (Exception)
                {

                    return false;
                }
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion

        #region Add new staff
        public static bool AddStaff(Staff staff)
        {
            if (staff == null)
            {
                return false;
            }
            try
            {
                db.Staffs.Add(staff);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                string s = e.ToString();
                return false;
            }
            return true;
        }
        #endregion

        #region Check Staff Infomation
        public int CheckStaffInfo(string staffUserAccount, string staffPhoneNumber, string staffEmail)
        {
            User checkStaffUserName = db.Users.FirstOrDefault(m => m.Username.Equals(staffUserAccount.Trim()));
            if (checkStaffUserName != null)
            {
                return 1;
            }
            Staff checkStaffPhoneNumber = db.Staffs.FirstOrDefault(m => m.StaffPhoneNumber.Equals(staffPhoneNumber.Trim()));
            if (checkStaffPhoneNumber != null)
            {
                return 2;
            }
            User checkStaffEmail = db.Users.FirstOrDefault(m => m.Email.Equals(staffEmail.Trim()));
            if (checkStaffEmail != null)
            {
                return 3;
            }            
            return 0;
        }

        #endregion

        #region Check Staff Infomation When Editting
        public int CheckStaffInfoInEdit(string staffPhoneNumber, string staffEmail, int id)
        {
            Staff checkStaffPhoneNumber = db.Staffs.FirstOrDefault(m => m.StaffPhoneNumber.Equals(staffPhoneNumber.Trim()) && m.UserId != id);
            if (checkStaffPhoneNumber != null)
            {
                return 1;
            }
            User checkStaffEmail = db.Users.FirstOrDefault(m => m.Email.Equals(staffEmail.Trim()) && m.UserId != id);
            if (checkStaffEmail != null)
            {
                return 2;
            }
            return 0;
        }

        #endregion

        #region Edit supplier
        public static bool EditStaffInfo(int userId, String staffName, String staffAddress, String staffPhoneNumber, String staffEmail)
        {
            var staffDetail = db.Staffs.SingleOrDefault(n => n.User.UserId == userId);
            if (staffDetail != null)
            {
                try
                {
                    staffDetail.User.UserId = userId;
                    staffDetail.User.Fullname = staffName;
                    staffDetail.User.Email = staffEmail;
                    staffDetail.StaffAddress = staffAddress;
                    staffDetail.StaffPhoneNumber = staffPhoneNumber;

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    string s = e.ToString();
                    return false;
                }
                return true;
            }

            else
            {
                return false;
            }
        }
        #endregion
    }
}