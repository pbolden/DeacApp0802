using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using AutoMapper;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.DAL
{
    public class NeedsCommunionRepository : GenericRepository<NeedsCommunion>, INeedsCommunionRepository<NeedsCommunion>
    {
        public NeedsCommunionRepository(CcgDbContext context) : base(context)
        {
        }

        public override IEnumerable<NeedsCommunion> FindAll(Expression<Func<NeedsCommunion, bool>> predicate = null)
        {

            var dbSet = context.Set<NeedsCommunion>();
            return predicate != null ? dbSet.Include("Member").Where(predicate).AsEnumerable().ToList() : dbSet.Include("Member").AsEnumerable().ToList();
            
        }
    }
}