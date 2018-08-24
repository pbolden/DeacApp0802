using DeaconCCGManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DeaconCCGManagement.DAL
{
    public interface IRoleRepository<TEntity>
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

        // unique to this repository
        void AddRole(IdentityRole role);
        Task<IdentityResult> RemoveRoleAsync(IdentityRole role);
        IdentityRole FindRoleByName(string roleName);
        IEnumerable<IdentityRole> GetRolesSorted();
        Task<bool> RoleExistsAsync(string roleName);
        Task<IdentityResult> UpdateRoleAsync(IdentityRole role);
    }
}
