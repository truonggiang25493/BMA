﻿using System;
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

        public bool ConfirmAccount(int userId)
        {
            var User = db.Users.FirstOrDefault(n => n.UserId == userId);
            User.IsConfirmed = true;
            db.SaveChanges();
            return true;
        }
        public User GetUser(int cusId)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == cusId);
            return user;
        }

        public bool checkEmailExisted(int cusUserId, string Email)
        {
            var checkUser = db.Users.SingleOrDefault(n => n.Email == Email && n.UserId != cusUserId);
            if (checkUser != null)
            {
                return true;
            }
            return false;
        }

        public bool checkPhoneExisted(int userId, string phoneNumber)
        {
            var checkCustomerUser = db.Customers.SingleOrDefault(n => n.CustomerPhoneNumber == phoneNumber && n.UserId != userId);
            if (checkCustomerUser != null)
            {
                return true;
            }
            var checkStaffUser = db.Staffs.SingleOrDefault(n => n.StaffPhoneNumber == phoneNumber && n.UserId != userId);
            if (checkStaffUser != null)
            {
                return true;
            }
            return false;
        }

        public bool checkTaxCodeExisted(int cusUserId, string taxCode)
        {
            var checkUser = db.Customers.SingleOrDefault(n => n.TaxCode == taxCode && n.UserId != cusUserId);
            if (checkUser != null)
            {
                return true;
            }
            return false;
        }

        public bool ChangeInformation(int userId, string name, string email, string address, string taxCode, string phone)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == userId);
            if (user.RoleId == 3)
            {
                user.Fullname = name;
                user.Email = email;
                user.Customers.ElementAt(0).CustomerAddress = address;
                user.Customers.ElementAt(0).TaxCode = taxCode;
                user.Customers.ElementAt(0).CustomerPhoneNumber = phone;
                db.SaveChanges();
                return true;
            }
            else if (user.RoleId == 2)
            {
                user.Fullname = name;
                user.Email = email;
                user.Staffs.ElementAt(0).StaffAddress = address;
                user.Staffs.ElementAt(0).StaffPhoneNumber = phone;
                db.SaveChanges();
                return true;
            }
            else if (user.RoleId == 1)
            {
                user.Fullname = name;
                user.Email = email;
                db.SaveChanges();
                return true;
            }
            return false;
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

        public string CreateStaffPassword(string password)
        {
            string salt = CreateSalt();
            string encodePassword = CreatePasswordHash(password, salt);
            return encodePassword;
        }
        public string CreatePassword(string rawPassword)
        {
            return CreatePasswordHash(rawPassword, CreateSalt());
        }

        public string CreateIdHash(int userId, string salt)
        {
            string passwordSalt = String.Concat(userId, salt);
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(passwordSalt, "sha1");
            string completePws = string.Format("{0}{1}", hashedPwd, salt);
            return completePws;
        }
        public string EncodeUserId(int userId)
        {
            return CreateIdHash(userId, CreateSalt());
        }
    }
}