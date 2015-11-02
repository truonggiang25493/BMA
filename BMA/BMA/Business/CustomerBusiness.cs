using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class CustomerBusiness
    {
        private BMAEntities db;

        public CustomerBusiness()
        {
            db = new BMAEntities();
        }

        public List<Customer> GetCustomerList()
        {
            return db.Customers.Where(m => m.IsActive).ToList();
        }

        #region AddCustomerForOrder
        /// <summary>
        /// Add new user for customer when approve the order
        /// Remove the guest info
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="email">Email</param>
        /// <param name="customerName">Fullname</param>
        /// <param name="customerAddress">Address</param>
        /// <param name="customerPhoneNumber">Phone number</param>
        /// <param name="customerTaxCode">Tax code</param>
        /// <param name="orderId">Order Id</param>
        /// <returns></returns>
        public bool AddCustomerForOrder(string username, string email, string customerName, string customerAddress,
            string customerPhoneNumber, string customerTaxCode, int orderId)
        {
            Customer customer = new Customer
            {
                CustomerAddress = customerAddress,
                CustomerPhoneNumber = customerPhoneNumber,
                TaxCode = customerTaxCode,
                IsActive = true,
                IsLoyal = false
            };
            List<Customer> customers = new List<Customer> { customer };

            User user = new User
            {
                Fullname = customerName,
                Email = email,
                Username = username,
                Password = "123456"
            };
            //Bug Generate password
            Role role = db.Roles.FirstOrDefault(m => m.Name.Equals("Customer"));
            user.Role = role;
            user.Customers = customers;

            Order order = db.Orders.FirstOrDefault(m => m.OrderId == orderId && !m.IsStaffEdit);
            if (order != null)
            {
                order.User = user;
                // Remove Guest Info
                GuestInfo guestInfo = order.GuestInfo;
                if (guestInfo != null)
                {
                    order.GuestInfo = null;
                    db.GuestInfoes.Remove(guestInfo);
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
            return false;
        }
        #endregion

    }
}