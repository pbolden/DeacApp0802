using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using WebGrease.Css.Extensions;

namespace DeaconCCGManagement.DAL
{
    /// <summary>
    /// Data store logic and CRUD operations for app users.
    /// </summary>
    public class AppUserRepository : GenericRepository<CCGAppUser>, IAppUserRepository<CCGAppUser>
    {

        public AppUserRepository(CcgDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Add a new app user without a password.
        /// </summary>
        /// <param name="user"></param>
        public void AddUser(CCGAppUser user)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                if (user != null)
                    userManager.Create(user);
            }
        }

        /// <summary>
        /// Add new app user with password.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="password"></param>
        public void AddUserWithPassword(CCGAppUser user, string password)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                if (user != null)
                    userManager.Create(user, password);
            }


        }

        /// <summary>
        /// Remove a user.
        /// </summary>
        /// <param name="user"></param>
        public void RemoveUser(CCGAppUser user)
        {
            if (user != null)
            {               
                using (var userStore = new UserStore<CCGAppUser>(context))
                using (var userManager = new ApplicationUserManager(userStore))
                {
                    // Remove all contact records created by the user
                    var contactRecords = context.ContactRecords.Where(r => r.AppUserId == user.Id);
                    contactRecords.ForEach(r => context.ContactRecords.Remove(r));

                    // Remove all pass along records created by the user
                    foreach (var contactRecord in contactRecords)
                    {
                        var passAlongRecord = context.PassAlongContacts.FirstOrDefault(r => r.Id == contactRecord.Id);
                        if (passAlongRecord != null)
                            context.PassAlongContacts.Remove(passAlongRecord);
                    }
                    context.SaveChanges();
                    userManager.Delete(user);
                }
            }
        }

        /// <summary>
        /// Returns app users filtered by expression or
        /// returns all app users if no expression is passed.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<CCGAppUser> FindUsers(Expression<Func<CCGAppUser, bool>> predicate = null)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                if (predicate != null)
                {
                    return userManager.Users.Where(predicate).OrderBy(u => u.LastName).AsEnumerable();
                }
                return userManager.Users.OrderBy(u => u.LastName).AsEnumerable();
            }

        }
        /// <summary>
        /// Returns users by role. e.g., collection of deacons.
        /// </summary>
        /// <param name="role">User role to filter by.</param>
        /// <returns></returns>
        public IEnumerable<CCGAppUser> FindUsersByRole(AppUserRole role)
        {
            if (!Enum.IsDefined(typeof(AppUserRole), role) || role == AppUserRole.None)
                return null;

            var usersFiltered = new List<CCGAppUser>();

            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                var users = userManager.Users;
                foreach (var user in users)
                    if (IsInRole(user.Email, role))
                        usersFiltered.Add(user);

                return usersFiltered;
            }

        }

        /// <summary>
        /// Returns single app user filtered by expression.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public CCGAppUser FindUser(Expression<Func<CCGAppUser, bool>> predicate)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                return predicate != null ? userManager.Users.SingleOrDefault(predicate) : null;
            }
        }

        /// <summary>
        /// Returns app user with matching id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CCGAppUser FindUserById(string id)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                return string.IsNullOrEmpty(id) ? null : userManager.FindById(id);
            }

        }

        /// <summary>
        /// Returns app user with matching email.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public CCGAppUser FindUserByEmail(string email)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                return userManager.FindByEmail(email);
            }

        }

        /// <summary>
        /// Adds user to a role.
        /// </summary>
        /// <param name="userId">User's id.</param>
        /// <param name="roleName">Role the user should be added.</param>
        /// <returns></returns>
        public void AddUserToRole(string userId, string roleName)
        {
            if (base.Exists(u => u.Id == userId))
            {
                using (var userStore = new UserStore<CCGAppUser>(context))
                using (var userManager = new ApplicationUserManager(userStore))
                {
                    userManager.AddToRoles(userId, roleName);
                }
            }
        }

        /// <summary>
        /// Remove role from user.
        /// </summary>
        /// <param name="userId">User's id.</param>
        /// <param name="roleName">Role the user should be removed.</param>
        /// <returns></returns>
        public void RemoveRoleFromUser(string userId, string roleName)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                userManager.RemoveFromRole(userId, roleName);
            }

        }

        /// <summary>
        /// Returns user's roles as a list of strings.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetUserRoles(string userId)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                return userManager.GetRoles(userId).OrderBy(r => r).ToList();
            }

        }

        /// <summary>
        ///  Determines if user is in a role.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string email, AppUserRole role)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                var user = userManager.Users.SingleOrDefault(u => u.Email.Equals(email));
                if (user == null)
                    return false;

                switch (role)
                {
                    case AppUserRole.Admin:
                        return userManager.IsInRole(user.Id, AuthHelper.Admin);
                    case AppUserRole.DeaconLeadership:
                        return userManager.IsInRole(user.Id, AuthHelper.DeaconLeadership);
                    case AppUserRole.Deacon:
                        return userManager.IsInRole(user.Id, AuthHelper.Deacon);
                    case AppUserRole.Pastor:
                        return userManager.IsInRole(user.Id, AuthHelper.Pastor);
                    case AppUserRole.None:
                    default:
                        return false;
                }
            }


        }

        /// <summary>
        /// Checks if user is in role.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool IsInRole(string email, string role)
        {
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                var user = userManager.Users.SingleOrDefault(u => u.Email.Equals(email));
                if (user == null)
                    return false;
                if (role.Equals(AuthHelper.Admin))
                    return userManager.IsInRole(user.Id, AuthHelper.Admin);
                if (role.Equals(AuthHelper.DeaconLeadership))
                    return userManager.IsInRole(user.Id, AuthHelper.DeaconLeadership);
                if (role.Equals(AuthHelper.Deacon))
                    return userManager.IsInRole(user.Id, AuthHelper.Deacon);
                if (role.Equals(AuthHelper.Pastor))
                    return userManager.IsInRole(user.Id, AuthHelper.Pastor);

                return false;
            }


        }

        /// <summary>
        /// Updates user's data.
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(CCGAppUser user)
        {           
            using (var userStore = new UserStore<CCGAppUser>(context))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                context.Entry(user).State = EntityState.Modified;

                if (user != null)
                    userManager.Update(user);
            }


        }
    }
}