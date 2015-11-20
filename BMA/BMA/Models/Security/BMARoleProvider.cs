using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;

namespace BMA.Models.Security
{
    public class BMARoleProvider : RoleProvider
    {
        public override string[] GetRolesForUser(string username)
        {
            using (BMAEntities db = new BMAEntities())
            {
                User user = db.Users.FirstOrDefault(u => u.Username == username);

                var roles = from u in db.Users
                            from r in db.Roles
                            where u.RoleId == r.RoleId
                            select r.Name;
                if (roles != null)
                    return roles.ToArray();
                else
                    return new string[] { }; ;
            }
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            using (BMAEntities db = new BMAEntities())
            {
                User user = db.Users.FirstOrDefault(u => u.Username == username);

                var roles = from u in db.Users
                            from r in db.Roles
                            where u.RoleId == r.RoleId
                            select r.Name;
                if (user != null)
                    return roles.Any(r => r.Equals(roleName, StringComparison.CurrentCultureIgnoreCase));
                else
                    return false;
            }
        }
        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new NotImplementedException();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new NotImplementedException();
        }
    }
}