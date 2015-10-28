using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BMA.Models;

namespace BMA.Business
{
    public class AccountBusiness
    {
        public static User checkLogin(string account, string password)
        {
            BMAEntities db = new BMAEntities();
            User endUser = db.Users.SingleOrDefault(n => n.Username == account && n.Password == password);
            return endUser;
        }
    }
}