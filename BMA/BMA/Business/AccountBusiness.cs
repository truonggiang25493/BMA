using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;
using System.Security.Cryptography;
using System.Web.Security;

namespace BMA.Business
{
    public class AccountBusiness
    {
        private readonly BMAEntities db;
        public AccountBusiness()
        {
            db = new BMAEntities();
        }
        //public User checkLogin(string account, string password)
        //{
        //    User endUser = db.Users.SingleOrDefault(n => n.Username == account);
        //    if (endUser != null)
        //    {
        //        if (endUser.Password == CreatePasswordHash(password, endUser.Salt))
        //        {
        //            return endUser;
        //        }
        //    }
        //    return null;
        //}

        public User checkLogin(string account, string password)
        {
            User endUser = db.Users.SingleOrDefault(n => n.Username.Equals(account));
            if (endUser != null)
            {
                string salt = endUser.Password.Substring(endUser.Password.Length - 88);
                //string passToCompare = endUser.Password.Remove(endUser.Password.Length - 88);
                if (endUser.Password == CreatePasswordHash(password, salt))
                {
                    return endUser;
                }
            }
            return null;
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
            string salt = user.Password.Substring(user.Password.Length - 88);
            oldPass = CreatePasswordHash(oldPass, salt);
            if (user.Password == oldPass)
            {
                return true;
            }
            return false;
        }

        private static string CreateSalt()
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] byteArr = new byte[64];
            rng.GetBytes(byteArr);
            return Convert.ToBase64String(byteArr);
        }

        private static string CreatePasswordHash(string password, string salt)
        {
            string passwordSalt = String.Concat(password, salt);
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordSalt, "sha1");
            string completePws = string.Format("{0}{1}", hashedPwd, salt);
            return completePws;
        }
        public bool ChangePassword(int cusId, string newPass)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == cusId);
            //user.Salt = CreateSalt();
            user.Password = CreatePasswordHash(newPass, CreateSalt());
            db.SaveChanges();
            return true;
        }
    }
}