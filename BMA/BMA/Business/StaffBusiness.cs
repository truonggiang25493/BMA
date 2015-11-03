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
            List<Staff> staffList = db.Staffs.OrderBy(n => n.IsActive).ToList();
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
    }
}