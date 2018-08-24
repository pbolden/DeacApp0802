using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DeaconCCGManagement.DAL
{
    public class RoleRepository : GenericRepository<IdentityRole>, IRoleRepository<IdentityRole>
    {

        public RoleRepository(CcgDbContext context) : base(context)
        {
        }

        public void AddRole(IdentityRole role)
        {
            using (var roleStore = new RoleStore<IdentityRole>(new CcgDbContext()))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                roleManager.Create(role);
            }
        }

        public async Task<IdentityResult> RemoveRoleAsync(IdentityRole role)
        {
            using (var roleStore = new RoleStore<IdentityRole>(new CcgDbContext()))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                 return await roleManager.DeleteAsync(role);
            }
        }

        public IdentityRole FindRoleByName(string roleName)
        {
            using (var roleStore = new RoleStore<IdentityRole>(new CcgDbContext()))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                return roleManager.FindByName(roleName);
            }
        }

        public IEnumerable<IdentityRole> GetRolesSorted()
        {
            using (var roleStore = new RoleStore<IdentityRole>(new CcgDbContext()))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                 var roles = roleManager.Roles.ToList();
                 roles.Sort(new IdentityRoleCompareByName());
                 return roles;
            }
        }

        public async Task<bool> RoleExistsAsync(string roleName)
        {
            using (var roleStore = new RoleStore<IdentityRole>(new CcgDbContext()))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                return await roleManager.RoleExistsAsync(roleName);
            }
        }

        public async Task<IdentityResult> UpdateRoleAsync(IdentityRole role)
        {
            using (var roleStore = new RoleStore<IdentityRole>(new CcgDbContext()))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                return await roleManager.UpdateAsync(role);
            }
        } 
    }
}