using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.DAL
{
    public class PassAlongContactRepository : GenericRepository<PassAlongContact>, IPassAlongContactRepository<PassAlongContact>
    {
        public PassAlongContactRepository(CcgDbContext context) : base(context)
        {
            
        }

        public IEnumerable<ContactRecord> FindContactRecords(bool archive)
        {
            return !archive ? FindContactRecords(c => !c.Archive) 
                : FindContactRecords(c => c.Archive);
        }

        public IEnumerable<ContactRecord> FindContactRecords(Expression<Func<PassAlongContact, bool>> predicate)
        {
            var contactRecords = new List<ContactRecord>();
           
            var dbSet = context.Set<PassAlongContact>();

            var passAlongContacts = dbSet
                .Where(predicate).OrderByDescending(c => c.Timestamp);

            foreach (var passAlongContact in passAlongContacts)
            {
                var record = context.ContactRecords
                    .Include("CCGMember")
                    .Include("AppUser")
                    .Include("ContactType")
                    .Include("PassAlongContact")
                    .SingleOrDefault(r => r.Id == passAlongContact.Id);
                contactRecords.Add(record);
            }

                //contactRecords.AddRange(passAlongContacts.Select(passAlongContact
                //    => passAlongContact.ContactRecord));
            
            return contactRecords;
        }
    }
}