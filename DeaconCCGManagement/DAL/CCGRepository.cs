using System.Linq.Expressions;
using DeaconCCGManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace DeaconCCGManagement.DAL
{
    public class CCGRepository : GenericRepository<CCG>, ICCGRepository<CCG>
    {
        public CCGRepository(CcgDbContext context) : base(context)
        {
        }



        public override IEnumerable<CCG> FindAll(Expression<Func<CCG, bool>> predicate = null)
        {

            var dbSet = context.Set<CCG>();
            return predicate != null ? dbSet.Include("AppUsers").Include("CCGMembers").Where(predicate).ToList() : dbSet.Include("AppUsers").Include("CCGMembers").AsEnumerable().ToList();
            
        }

        public override CCG Find(Expression<Func<CCG, bool>> predicate, bool firstOrDefault = false)
        {

            var dbSet = context.Set<CCG>();
            return firstOrDefault ? dbSet.Include("AppUsers").Include("CCGMembers").FirstOrDefault(predicate)
            : dbSet.Include("AppUsers").Include("CCGMembers").SingleOrDefault(predicate);
            
        }

        public virtual CCG FindById(int? id)
        {
            
            var dbSet = context.Set<CCG>();
            return dbSet.Include("AppUsers").Include("CCGMembers").SingleOrDefault(c => c.Id == id);
            
        }
    }
}