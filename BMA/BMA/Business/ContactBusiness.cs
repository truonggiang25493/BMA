using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class ContactBusiness
    {
        private readonly BMAEntities db;
        public ContactBusiness()
        {
            db = new BMAEntities();
        }

        public User GetStaff()
        {
            User staffUser = db.Users.FirstOrDefault(n => n.RoleId == 2);
            return staffUser;
        }

        public string staffPhone(int userId)
        {
            Staff staffPhone = db.Staffs.FirstOrDefault(n => n.UserId == userId);
            return staffPhone.StaffPhoneNumber;
        }

        public StoreInfo StoreOwner()
        {
            StoreInfo storeOwner = db.StoreInfoes.FirstOrDefault();
            return storeOwner;
        }
    }
}