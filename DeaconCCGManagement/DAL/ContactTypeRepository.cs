using System.Collections.Generic;
using System.Linq;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.DAL
{
    public class ContactTypeRepository : GenericRepository<ContactType>, IContactTypeRepository<ContactType>
    {
        public ContactTypeRepository(CcgDbContext context) : base(context)
        {
        }

        /// <summary>
        /// Returns all contact types sorted.
        /// </summary>
        /// <returns></returns>
        public List<ContactType> GetContactTypesSorted()
        {
            return base.FindAll().OrderBy(c => c.Name).ToList();
        } 
        
    }
}