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
    }
}