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

        public bool SetLogoutTime(int userId)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == userId);
            user.LogoutTime = DateTime.Now;
            db.SaveChanges();
            return true;
        }

        public List<Order> CustomerOffNewOrderNoty(int userId)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == userId);
            List<Order> lstNotyOrder = new List<Order>();
            if (user.LogoutTime != null)
            {
                DateTime logoutTime = user.LogoutTime.Value;
                List<Order> lstOrder = db.Orders.Where(n => n.CustomerUserId == userId).ToList();
                for (int i = 0; i < lstOrder.Count; i++)
                {
                    int orderId = lstOrder[i].OrderId;
                    Order orderNoty = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                    if (orderNoty != null && orderNoty.CreateTime != null)
                    {
                        if (orderNoty.CreateTime > logoutTime)
                        {
                            lstNotyOrder.Add(orderNoty);
                        }
                    }
                }
            }
            return lstNotyOrder;
        }

        public List<Order> CustomerOffEditedOrderNoty(int userId)
        {
            User user = db.Users.SingleOrDefault(n => n.UserId == userId);
            List<Order> lstNotyOrder = new List<Order>();
            if (user.LogoutTime != null)
            {
                DateTime logoutTime = user.LogoutTime.Value;
                List<Order> lstOrder = db.Orders.Where(n => n.CustomerUserId == userId).ToList();
                for (int i = 0; i < lstOrder.Count; i++)
                {
                    int orderId = lstOrder[i].OrderId;
                    Order orderNoty = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                    if (orderNoty != null && orderNoty.CreateTime != null)
                    {
                        if (orderNoty.CreateTime > logoutTime)
                        {

                        }
                        else
                        {
                            if (orderNoty != null && orderNoty.ApproveTime != null)
                            {
                                if (orderNoty.ApproveTime > logoutTime)
                                {
                                    lstNotyOrder.Add(orderNoty);
                                }
                            }
                        }
                    }

                    if (orderNoty != null && orderNoty.StartProduceTime != null)
                    {
                        if (orderNoty.StartProduceTime > logoutTime)
                        {
                            lstNotyOrder.Add(orderNoty);
                        }
                    }

                    if (orderNoty != null && orderNoty.DeliveryTime != null)
                    {
                        if (orderNoty.DeliveryTime > logoutTime)
                        {
                            lstNotyOrder.Add(orderNoty);
                        }
                    }

                    if (orderNoty != null && orderNoty.FinishTime != null)
                    {
                        if (orderNoty.FinishTime > logoutTime)
                        {
                            lstNotyOrder.Add(orderNoty);
                        }
                    }

                    if (orderNoty != null && orderNoty.CancelTime != null)
                    {
                        if (orderNoty.CancelTime > logoutTime)
                        {
                            lstNotyOrder.Add(orderNoty);
                        }
                    }
                }
            }
            return lstNotyOrder;
        }

        public List<Order> CustomerOffConfirmOrderNoty(int userId)
        {

            User user = db.Users.SingleOrDefault(n => n.UserId == userId);
            List<Order> lstNotyOrder = new List<Order>();
            if (user.LogoutTime != null)
            {
                DateTime logoutTime = user.LogoutTime.Value;
                List<Order> lstOrder = db.Orders.Where(n => n.CustomerUserId == userId).ToList();
                for (int i = 0; i < lstOrder.Count; i++)
                {
                    int orderId = lstOrder[i].OrderId;
                    Order orderNoty = db.Orders.SingleOrDefault(n => n.OrderId == orderId);
                    if (orderNoty != null && orderNoty.CreateTime != null)
                    {
                        if (orderNoty != null && orderNoty.StaffEditTime != null)
                        {
                            if (orderNoty.StaffEditTime > logoutTime)
                            {
                                lstNotyOrder.Add(orderNoty);
                            }
                        }
                    }
                }
            }
            return lstNotyOrder;
        }

        public List<Order> StaffOffNewOrderNoty()
        {
            //List<Order> lstNotyOrder = new List<Order>();
            List<User> lstUser = db.Users.Where(n => n.RoleId == 2).ToList();
            foreach (var item in lstUser)
            {
                if (item.LogoutTime == null)
                {
                    item.LogoutTime = new DateTime(1, 1, 1);
                }
            }
            DateTime logoutTime = lstUser[0].LogoutTime.Value;
            for (int i = 0; i < lstUser.Count; i++)
            {
                if (lstUser[i].LogoutTime > lstUser[i + 1].LogoutTime)
                {
                    logoutTime = lstUser[i].LogoutTime.Value;
                }
            }

            List<Order> lstOrder = db.Orders.Where(n => n.CreateTime > logoutTime).ToList();
            return lstOrder;
        }

        public List<Order> StaffOffConfirmOrderNoty()
        {
            List<User> lstUser = db.Users.Where(n => n.RoleId == 2).ToList();
            foreach (var item in lstUser)
            {
                if (item.LogoutTime == null)
                {
                    item.LogoutTime = new DateTime(1, 1, 1);
                }
            }
            DateTime logoutTime = lstUser[0].LogoutTime.Value;
            for (int i = 0; i < lstUser.Count; i++)
            {
                if (lstUser[i].LogoutTime > lstUser[i + 1].LogoutTime)
                {
                    logoutTime = lstUser[i].LogoutTime.Value;
                }
            }

            List<Order> lstOrder = db.Orders.Where(n => n.ApproveTime > logoutTime).ToList();
            return lstOrder;
        }

        public List<Order> StaffOffCancelOrderNoty()
        {
            List<User> lstUser = db.Users.Where(n => n.RoleId == 2).ToList();
            foreach (var item in lstUser)
            {
                if (item.LogoutTime == null)
                {
                    item.LogoutTime = new DateTime(1, 1, 1);
                }
            }
            DateTime logoutTime = lstUser[0].LogoutTime.Value;
            for (int i = 0; i < lstUser.Count; i++)
            {
                if (lstUser[i].LogoutTime > lstUser[i + 1].LogoutTime)
                {
                    logoutTime = lstUser[i].LogoutTime.Value;
                }
            }

            List<Order> lstOrder = db.Orders.Where(n => n.CancelTime > logoutTime).ToList();
            return lstOrder;
        }

        public List<ProductMaterial> StaffOffLowQuantityNoty()
        {
            List<ProductMaterial> lstPM = db.ProductMaterials.Where(n => n.CurrentQuantity < n.StandardQuantity).ToList();
            return lstPM;
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