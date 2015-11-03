using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class AccountBusiness
    {
        private readonly BMAEntities db;
        public AccountBusiness()
        {
            db = new BMAEntities();
        }
        public User checkLogin(string account, string password)
        {
            User endUser = db.Users.SingleOrDefault(n => n.Username == account && n.Password == password);
            return endUser;
        }

        public User GetUser(int cusId)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == cusId);
            return user;
        }
        public bool ChangeInformation(int cusId, string name, string email, string address, string taxCode, string phone)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == cusId);
            user.Fullname = name;
            user.Email = email;
            user.Customers.ElementAt(0).CustomerAddress = address;
            user.Customers.ElementAt(0).TaxCode = taxCode;
            user.Customers.ElementAt(0).CustomerPhoneNumber = phone;
            db.SaveChanges();
            return true;
        }

        public bool checkPass(int cusId, string oldPass)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == cusId);
            if (user.Password == oldPass)
            {
                return true;
            }
            return false;
        }
        public bool ChangePassword(int cusId, string newPass)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == cusId);
            user.Password = newPass;
            db.SaveChanges();
            return true;
        }
    }
}