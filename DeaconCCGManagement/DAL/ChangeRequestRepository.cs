using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.DAL
{
    public class ChangeRequestRepository : GenericRepository<ChangeRequest>, IChangeRequestRepository<ChangeRequest>
    {
        public ChangeRequestRepository(CcgDbContext context) : base(context)
        {
            
        }

        public IEnumerable<ChangeRequest> GetChangeRequests()
        {
            var dbSet = context.Set<ChangeRequest>();
            return dbSet.OrderBy(cr => cr.CRDate).ToList();
        }

        public override ChangeRequest FindById(int? id)
        {           
            var dbSet = context.Set<ChangeRequest>();
            return dbSet.Include("CcgMember").Include("Deacon").SingleOrDefault(chr => chr.Id == id);            
        }

        public override void Add(ChangeRequest entity)
        {           
            var dbSet = context.Set<ChangeRequest>();
            if(entity != null) dbSet.Add(entity);
            Save(context);            
        }       
    }
}