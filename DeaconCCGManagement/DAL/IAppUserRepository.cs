using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.DAL
{
    public interface IAppUserRepository<TEntity>
    {
        void AddOrUpdate(TEntity entity);
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        TEntity FindById(int? id);
        IEnumerable<TEntity> FindAll(Expression<Func<TEntity, bool>> predicate = null);
        TEntity Find(Expression<Func<TEntity, bool>> predicate, bool firstOrDefault = false);
        bool Exists(Expression<Func<TEntity, bool>> predicate);
        int Count();
        void Save(CcgDbContext context);


        // These are unique to this repository
        void AddUser(CCGAppUser user);
        void AddUserWithPassword(CCGAppUser user, string password);
        void RemoveUser(CCGAppUser user);
        IEnumerable<CCGAppUser> FindUsers(Expression<Func<CCGAppUser, bool>> predicate = null);
        IEnumerable<CCGAppUser> FindUsersByRole(AppUserRole role);
        CCGAppUser FindUser(Expression<Func<CCGAppUser, bool>> predicate);
        CCGAppUser FindUserById(string id);
        CCGAppUser FindUserByEmail(string email);
        void AddUserToRole(string userId, string roleName);
        void RemoveRoleFromUser(string userId, string roleName);
        List<string> GetUserRoles(string userId);
        bool IsInRole(string email, AppUserRole role);
        bool IsInRole(string email, string role);
        void UpdateUser(CCGAppUser user);
    }
}
