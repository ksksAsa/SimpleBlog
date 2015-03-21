using System;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
namespace SimpleBlog.Infrastructure
{
    public class RoleProvider:System.Web.Security.RoleProvider
    {

        public override string[] GetRolesForUser(string username)
        {
            //if(username=="jim")
              //  return new[] {"admin"};
            //return new string[] { };
         //  try
          //  {
            //int countRoles = Auth.User.Roles.Count;
           // Debug.WriteLine("The count of roles is " +countRoles);
                return  Auth.User.Roles.Select(role => role.Name).ToArray();
           // }
           // catch {
               // return new[] { "admin" };
            //}
           // Array arr = Auth.User.Roles.Select(role=>role.Name).ToArray();
            //System.Diagnostics.Debug.WriteLine(arr);
            //return new string[] { };
        }


        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override string ApplicationName
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            throw new System.NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new System.NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            throw new System.NotImplementedException();
        }

       

        public override string[] GetUsersInRole(string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            throw new System.NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            throw new System.NotImplementedException();
        }

        public override bool RoleExists(string roleName)
        {
            throw new System.NotImplementedException();
        }
    }
}