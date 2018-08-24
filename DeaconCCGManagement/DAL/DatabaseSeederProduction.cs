using System.Collections.Generic;
using System.Data.Entity.Migrations;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;
using Microsoft.AspNet.Identity;


namespace DeaconCCGManagement.DAL
{
    /// <summary>
    /// Seeds the database for production.
    /// </summary>
    public class DatabaseSeederProduction
    {
        private CcgDbContext _dbContext ;

        public void SeedDatabase(CcgDbContext dbContext)
        {

            _dbContext = dbContext;

              // No using block to let the migration dispose of the context
              var db = new UnitOfWork();

                SeedRoles(db);
                SeedCCGs(db);
                SeedContactTypes(db);
                _dbContext.SaveChanges();
        }

        private void SeedRoles(UnitOfWork db)
        {

            // Seed roles to database
            using (var roleStore = new RoleStore<IdentityRole>(_dbContext))
            using (var roleManager = new RoleManager<IdentityRole>(roleStore))
            {
                roleManager.Create(new IdentityRole { Name = AuthHelper.Admin });
                roleManager.Create(new IdentityRole { Name = AuthHelper.DeaconLeadership });
                roleManager.Create(new IdentityRole { Name = AuthHelper.Deacon });
                roleManager.Create(new IdentityRole { Name = AuthHelper.Pastor });
            }
        }

        private void SeedCCGs(UnitOfWork db)
        {
            string ccgName;
            for (var i = 1; i < 100; i++)
            {
                ccgName = $"CCG{i:d2}"; 

                var name = ccgName;

                var dbSet = _dbContext.Set<CCG>();
                if (!dbSet.Any(ccg => ccg.CCGName.Equals(name)))
                {
                    dbSet.AddOrUpdate(new CCG { CCGName = ccgName });
                }
            }
        }

        private void SeedContactTypes(UnitOfWork db)
        {
            var contactTypes = new List<string>
            {
                "Call",
                "Church",
                "Email",
                "District Newsletter",
                "Letter",
                "Card - Birthday",
                "Card - Anniversary",
                "Card - Bereavement",
                "Card - Encouragement",
                "Call - Left Message",
                "Family Dinner",
                "Event",
                "Visit",
                "Instant Messaging",
                "Facebook Entry",
                "Text Messaging",
                "Prayer Request",
                "Other",
            };

            foreach (var contactType in contactTypes)
            {
                var dbSet = _dbContext.Set<ContactType>();

                if (!dbSet.Any(ct => ct.Name.Equals(contactType)))
                {
                    dbSet.AddOrUpdate(new ContactType { Name = contactType });
                }
            }
        }
    }
}