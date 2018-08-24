using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.Test_Data;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DeaconCCGManagement.DAL
{
    /// <summary>
    /// Seeds the database with test data for development.
    /// </summary>
    public class DatabaseSeeder
    {
        private static Random _randomizer = new Random();

        // Used to seed app users and assign their roles.
        private List<CCGAppUser> _appUsers = new List<CCGAppUser>();

        // Used to seed members and contact records.
        private List<CCGMember> _members = new List<CCGMember>();

        private CcgDbContext _dbContext;

        #region Lorem Ipsum dummy text

        private string dummyText = @"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod" +
                                    "tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam," +
                                    "quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo" +
                                    "consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse" +
                                    "cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non" +
                                    "proident, sunt in culpa qui officia deserunt mollit anim id est laborum." +
                                    "Aliquam gravida metus sit amet orci facilisis eu ultricies risus iaculis. Nunc" +
                                    "tempus tristique magna, molestie adipiscing nibh bibendum vel. Donec sed nisi luctus" +
                                    "sapien scelerisque pretium id eu augue. Mauris ipsum arcu, feugiat non tempor" +
                                    "tincidunt, tincidunt sit amet turpis. Vestibulum scelerisque rutrum luctus. Curabitur" +
                                    "eu ornare nisl. Cras in sem ut eros consequat fringilla nec vitae felis. Nulla" +
                                    "facilisi. Mauris suscipit feugiat odio, a condimentum felis luctus in. Nulla interdum" +
                                    "dictum risus, accumsan dignissim tortor ultricies in. Duis justo mauris, posuere vel" +
                                    "convallis ut, auctor non libero. Ut a diam magna, ut egestas dolor. Nulla convallis," +
                                    "orci in sodales blandit, lorem augue feugiat nulla, vitae dapibus mi ligula quis ligula." +
                                    "Aenean mattis pulvinar est quis bibendum. Donec posuere pulvinar ligula, nec sagittis lacus pharetra ac. Cras nec" +
                                    "tortor mi. Pellentesque et magna vel erat consequat commodo a id nunc. Donec velit" +
                                    "elit, vulputate nec tristique vitae, scelerisque ac sem. Proin blandit quam ut magna" +
                                    "ultrices porttitor. Fusce rhoncus faucibus tincidunt. Cras ac erat lacus, dictum" +
                                    "elementum urna. Nulla facilisi. Praesent ac neque nulla, in rutrum ipsum. Aenean" +
                                    "imperdiet, turpis sit amet porttitor hendrerit, ante dui eleifend purus, eu fermentum" +
                                    "dolor enim et elit. Suspendisse facilisis molestie hendrerit. Aenean congue congue sapien, ac" +
                                    "luctus nulla rutrum vel. Fusce vitae dui urna. Fusce iaculis mattis justo sit amet" +
                                    "varius. Duis velit massa, varius in congue ut, tristique sit amet lorem. Curabitur" +
                                    "porta, mauris non pretium ultrices, justo elit tristique enim, et elementum tellus" +
                                    "enim sit amet felis. Sed sollicitudin rutrum libero sit amet malesuada. Duis vitae" +
                                    "gravida purus. Proin in nunc at ligula bibendum pharetra sit amet sit amet felis." +
                                    "Integer ut justo at massa ullamcorper sagittis. Mauris blandit tortor lacus," +
                                    "convallis iaculis libero. Etiam non pellentesque dolor. Fusce ac facilisis ipsum." +
                                    "Suspendisse eget ornare ligula. Aliquam erat volutpat. Aliquam in porttitor purus." +
                                    "Suspendisse facilisis euismod purus in dictum. Vivamus ac neque ut sapien" +
                                    "fermentum placerat. Sed malesuada pellentesque tempor. Aenean cursus, metus a" +
                                    "lacinia scelerisque, nulla mi malesuada nisi, eget laoreet massa risus eu felis." +
                                    "Vivamus imperdiet rutrum convallis. Proin porta, nunc a interdum facilisis, nunc dui" +
                                    "aliquet sapien, non consectetur ipsum nisi et felis. Nullam quis ligula nisi, sed" +
                                    "scelerisque arcu. Nam lorem arcu, mollis ac sodales eget, aliquet ac eros. Duis" +
                                    "hendrerit mi vitae odio convallis eget lobortis nibh sodales. Nunc ut nunc vitae" +
                                    "nibh scelerisque tempor at malesuada sapien. Nullam elementum rutrum odio nec aliquet.";

        #endregion

        public bool SeedDatabase(CcgDbContext dbContext)
        {
            //
            // Only use CcgDbContext created by migration
            //
            _dbContext = dbContext;

            var db = new UnitOfWork();

            SeedRoles(db);
            SeedCCGs(db);
            SeedContactTypes(db);
            dbContext.SaveChanges();
            SeedAppUsers(db);
            dbContext.SaveChanges();
            SeedAppUsersRoles(db);
            dbContext.SaveChanges();
            SeedMembers(db);
            dbContext.SaveChanges();
            SeedContactRecords(db);
            dbContext.SaveChanges();

            AssignUserIdToContactRecord(db);
            dbContext.SaveChanges();

            SeedPrayerRequests(db);
            dbContext.SaveChanges();

            SeedNeedsCommunion(db);
            dbContext.SaveChanges();

            return true;
        }

        private void SeedRoles(UnitOfWork db)
        {
            // Seed roles to database
            //db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.Admin });
            //db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.DeaconLeadership });
            //db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.Deacon });
            //db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.Pastor });


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
                ccgName = $"CCG{i:d2}"; // same as String.Format("CCG{0:d2},i")

                var name = ccgName;

                var dbSet = _dbContext.Set<CCG>();

                if (!dbSet.Any(ccg => ccg.CCGName.Equals(name)))
                {
                    dbSet.AddOrUpdate(new CCG { CCGName = ccgName });
                }
            }
            _dbContext.SaveChanges();
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

        private void SeedAppUsers(UnitOfWork db)
        {
            var testDataDeserializer = new TestDataDeserializer();
            var testDataDeacons = testDataDeserializer.DeserializeDeaconsTestData();

            // Get users from external source and add to list.
            foreach (var testDataDeacon in testDataDeacons)
            {
                if (string.IsNullOrEmpty(testDataDeacon.SharepointEmail) ||
                    string.IsNullOrEmpty(testDataDeacon.FamDistrictDeacon))
                    continue;

                var names = GetTestAppUserNames(testDataDeacon.FamDistrictDeacon);
                int? ccgId = GetTestCcgId(testDataDeacon.GroupName, db);

                _appUsers.Add(new CCGAppUser
                {
                    Email = testDataDeacon.SharepointEmail,
                    UserName = testDataDeacon.SharepointEmail,
                    LastName = names["Last Name"],
                    FirstName = names["First Name"],
                    CcgId = ccgId,
                    EmailAddress = testDataDeacon.Email,
                    PhoneNumber = GetRandomPhoneNumber(),
                });
            }

            // Add users to data store
            foreach (var appUser in _appUsers)
            {
                var dbSet = _dbContext.Set<CCGAppUser>();


                using (var userStore = new UserStore<CCGAppUser>(_dbContext))
                using (var userManager = new ApplicationUserManager(userStore))
                {

                    // if user exists, update
                    if (dbSet.Any(u => u.UserName.Equals(appUser.UserName)))
                    {
                        var user = userManager.Users.SingleOrDefault(u => u.UserName.Equals(appUser.UserName));
                        user.Email = appUser.Email;
                        user.UserName = appUser.UserName;
                        user.LastName = appUser.LastName;
                        user.FirstName = appUser.FirstName;
                        user.CcgId = appUser.CcgId;
                        user.EmailAddress = appUser.EmailAddress;
                        user.PhoneNumber = appUser.PhoneNumber;

                        _dbContext.Entry(user).State = EntityState.Modified;

                        userManager.Update(user);

                    }
                    else
                    {
                        // else add user
                        if (appUser != null)
                            userManager.Create(appUser);
                    }
                }
            }
        }

        private void SeedAppUsersRoles(UnitOfWork db)
        {
            //
            // Role assignments:
            //
            // admin, deacon, deacon leadership
            // pbolden@zionmbc.org
            // DFREEMAN@zionmbc.org

            // deacon leadership
            // JLATIMORE@zionmbc.org

            // pastor
            // oneal@zionmbc.org

            // all others assigned to deacon role only


            _appUsers.Clear();

            var dbSet = _dbContext.Set<CCGAppUser>();
            _appUsers = dbSet.AsEnumerable().ToList();

            using (var userStore = new UserStore<CCGAppUser>(_dbContext))
            using (var userManager = new ApplicationUserManager(userStore))
            {

                foreach (var appUser in _appUsers)
                {
                    switch (appUser.Email)
                    {
                        case "JLATIMORE@zionmbc.org":
                            if (dbSet.Any(u => u.Id == appUser.Id))
                            {
                                userManager.AddToRoles(appUser.Id, AuthHelper.DeaconLeadership);
                            }
                            //db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.DeaconLeadership);
                            break;
                        case "oneal@zionmbc.org":
                            if (dbSet.Any(u => u.Id == appUser.Id))
                            {
                                userManager.AddToRoles(appUser.Id, AuthHelper.Pastor);
                            }
                            //db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Pastor);
                            break;
                        case "DFREEMAN@zionmbc.org":
                        case "pbolden@zionmbc.org":
                            if (dbSet.Any(u => u.Id == appUser.Id))
                            {
                                userManager.AddToRoles(appUser.Id, AuthHelper.DeaconLeadership);
                                userManager.AddToRoles(appUser.Id, AuthHelper.Admin);
                                userManager.AddToRoles(appUser.Id, AuthHelper.Deacon);
                            }
                            //db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Admin);
                            //db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Deacon);
                            //db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.DeaconLeadership);
                            break;
                        default:
                            if (dbSet.Any(u => u.Id == appUser.Id))
                            {
                                userManager.AddToRoles(appUser.Id, AuthHelper.Deacon);
                            }
                            //db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Deacon);
                            break;
                    }
                }


            }
        }

        private Dictionary<string, string> GetTestAppUserNames(string famDistrictDeacon)
        {
            var names = new Dictionary<string, string>();
            string[] namesSplit = famDistrictDeacon.Split('_', '_');

            names.Add("Last Name", namesSplit[0][0] + namesSplit[0].Substring(1).ToLower());
            names.Add("First Name", namesSplit[2][0] + namesSplit[2].Substring(1).ToLower());

            return names;
        }

        private int? GetTestCcgId(string ccgName, UnitOfWork db)
        {
            string[] ccgSplit = ccgName.Split('_');
            ccgName = ccgSplit[0];
            if (ccgName == null) return null;

            var dbSet = _dbContext.Set<CCG>();
            var ccg = dbSet.FirstOrDefault(c => c.CCGName == ccgName);

            //var ccg = db.CCGRepository.Find(c => c.CCGName == ccgName, firstOrDefault:true);
            return ccg?.Id;
        }

        private string GetRandomPhoneNumber()
        {
            var sb = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                sb.Append(i == 0 ? _randomizer.Next(1, 10) : _randomizer.Next(0, 10));
            }
            return sb.ToString();
        }

        private void SeedMembers(UnitOfWork db)
        {
            var testDataDeserializer = new TestDataDeserializer();
            var members = testDataDeserializer.DeserializeMembersTestData();

            foreach (var member in members)
            {
                _members.Add(new CCGMember
                {
                    IndividualId = member.IndividualId.Equals("") ? null : member.IndividualId,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    Title = member.Title.Equals("") ? null : member.Title,
                    Suffix = member.Suffix.Equals("") ? null : member.Suffix,
                    Address = member.Address.Equals("") ? null : member.Address,
                    City = member.City,
                    State = member.State,
                    ZipCode = member.ZipCode,
                    CellPhoneNumber = member.CellPhone.Equals("") ? null : member.CellPhone,
                    PhoneNumber = member.HomePhone,
                    EmailAddress = member.PreferredEmail,
                    FamilyNumber = member.FamilyNumber.Equals("") ? null : member.FamilyNumber,
                    EnvelopNumber = member.EnvelopeNumber.Equals("") ? null : member.EnvelopeNumber,
                    BirthDate = GetRandomBirthDate(),
                    DateJoinedZion = GetRandomAnniversaryDate(),
                    AnniversaryDate = GetRandomAnniversaryDate(),
                    ZionEntryDate = member.EntryDate,
                    DateLastChanged = member.DateLastChanged,
                    CcgId = GetTestCcgId(member.CCG, db),
                });
            }


            // Delete all members
            //var records = db.MemberRepository.FindMembers();
            //foreach (var record in records)
            //{
            //    db.MemberRepository.Delete(record);
            //}
            //db.Commit();


            foreach (var member in _members)
            {
                var dbSet = _dbContext.Set<CCGMember>();

                if (!dbSet.Any(m => (m.FirstName.Equals(member.FirstName)
                                     && m.LastName.Equals(member.LastName))))
                {
                    dbSet.AddOrUpdate(member);

                    //db.MemberRepository.AddOrUpdate(member);
                }
            }
        }

        private string GetAppUserId(string famDistrictDeacon, UnitOfWork db)
        {
            var names = GetTestAppUserNames(famDistrictDeacon);
            string firstName = names["First Name"];
            string lastName = names["Last Name"];


            var dbSet = _dbContext.Set<CCGAppUser>();
            var appUser = dbSet.FirstOrDefault(u => u.FirstName == firstName &&
                                                    u.LastName == lastName);

            return appUser?.Id;
        }

        #region Seed contact records
        private void SeedContactRecords(UnitOfWork db)
        {
            int numOfRecords = 3000;

            var dbSet = _dbContext.Set<ContactRecord>();

            //
            //Delete all contact records
            //
            //var records = dbSet.Include("ContactType");
            //foreach (var record in records)
            //{
            //    db.ContactRecordRepository.Delete(record);
            //}
            //_dbContext.SaveChanges();

            // don't seed contact records if count is >= numOfRecords
            // this will avoid duplication of test data
            // can't check if already exists since it's random data


            int count = dbSet.Count();

            if (count >= numOfRecords)
            {
                return;
            }

            IList<ContactRecord> seedContactRecords = new List<ContactRecord>();

            for (int i = 0; i < numOfRecords; i++)
            {
                bool passAlong = _randomizer.Next(0, 5) == 0;
                bool isPrivate = _randomizer.Next(0, 10) == 0;
                var duration = TimeSpan.FromMinutes(_randomizer.Next(1, 120));
                var randomDate = GetRandomDate();
                seedContactRecords.Add(new ContactRecord
                {
                    Private = isPrivate,
                    ContactDate = randomDate,
                    Timestamp = randomDate,
                    Duration = duration,
                    PassAlong = passAlong,
                    Subject = GetRandomSubject(),
                    Comments = GetRandomComments(),
                    CCGMemberId = GetRandomMemberId(db),
                    ContactTypeId = GetRandomContactTypeId(db),
                });
            }

            foreach (ContactRecord rec in seedContactRecords)
            {
                dbSet.AddOrUpdate(rec);
            }
        }

        private void SeedNeedsCommunion(UnitOfWork db)
        {
            int numOfRecords = 1000;
            var seedNeedsCommunion = new List<NeedsCommunion>();

            var dbSetCcgMembers = _dbContext.Set<CCGMember>();
            var members = dbSetCcgMembers.Include("CCG").AsEnumerable().ToList();

            var dbSet = _dbContext.Set<NeedsCommunion>();

            if (dbSet.Count() >= numOfRecords)
            {
                return;
            }

            for (int i = 0; i < numOfRecords; i++)
            {
                var year = _randomizer.Next(4) == 0 ? DateTime.Today.Year - 1 : DateTime.Today.Year;
                seedNeedsCommunion.Add(new NeedsCommunion
                {
                    MemberId = members[_randomizer.Next(members.Count)].Id,
                    Timestamp = GetRandomDate(year)
                });
            }

            foreach (var needsCommunion in seedNeedsCommunion)
            {
                dbSet.AddOrUpdate(needsCommunion);
            }
        }

        private DateTime GetRandomDate(int startYear = 2015)
        {
            int year = _randomizer.Next(startYear, DateTime.Now.Year + 1);
            int month = _randomizer.Next(1, DateTime.Now.Month);
            int day = _randomizer.Next(1, DateTime.DaysInMonth(year, month) + 1);

            return new DateTime(year, month, day);
        }

        private DateTime GetRandomAnniversaryDate()
        {
            int year = _randomizer.Next(1985, DateTime.Now.Year + 1);
            int month = _randomizer.Next(1, 13);
            int day = _randomizer.Next(1, DateTime.DaysInMonth(year, month) + 1);

            return new DateTime(year, month, day);
        }

        private DateTime? GetRandomBirthDate()
        {
            int year = _randomizer.Next(1950, DateTime.Now.Year + 1);
            int month = _randomizer.Next(1, 13);
            int day = _randomizer.Next(1, DateTime.DaysInMonth(year, month) + 1);

            return new DateTime(year, month, day);
        }

        private DateTime GetRandomTimeStamp()
        {
            int year = _randomizer.Next(2015, 2018);
            int month = _randomizer.Next(1, 13);
            int day = _randomizer.Next(1, 29);
            int hour = _randomizer.Next(1, 13);
            int minute = _randomizer.Next(60);
            int seconds = _randomizer.Next(60);
            return new DateTime(year, month, day, hour, minute, seconds);
        }

        private int GetRandomMemberId(UnitOfWork db)
        {

            var dbSet = _dbContext.Set<CCGMember>();
            var members = dbSet.Include("CCG").AsEnumerable();
            int count = members.Count();
            if (count > 0)
            {
                var member = members.ElementAt(_randomizer.Next(0, count));
                return member.Id;
            }
            return 0;
        }

        private int GetRandomContactTypeId(UnitOfWork db)
        {

            var dbSet = _dbContext.Set<ContactType>();
            var types = dbSet.AsEnumerable();

            int count = types.Count();
            if (count > 0)
            {
                var type = types.ElementAt(_randomizer.Next(0, types.Count()));
                return type.Id;
            }
            return 0;

        }

        private void AssignUserIdToContactRecord(UnitOfWork db)
        {
            // get all contact records
            var dbSet = _dbContext.Set<ContactRecord>();
            var contactRecords = dbSet.AsEnumerable();

            foreach (var contactRecord in contactRecords)
            {
                // TODO replace ALL repository code

                // get member obj
                var dbSetCCGMember = _dbContext.Set<CCGMember>();
                var member = dbSetCCGMember.Include("CCG")
                    .SingleOrDefault(m => m.Id == contactRecord.CCGMemberId);

                // get ccg obj

                var dbSetCCG = _dbContext.Set<CCG>();
                var ccg = dbSetCCG.SingleOrDefault(g => g.Id == member.CcgId);


                // randomly select from CcgUsers collection
                List<CCGAppUser> appUsers;
                using (var userStore = new UserStore<CCGAppUser>(_dbContext))
                using (var userManager = new ApplicationUserManager(userStore))
                {
                    appUsers = userManager.Users.Where(u => u.CcgId == ccg.Id).OrderBy(u => u.LastName).AsEnumerable().ToList();
                }

                if (appUsers.Count == 0)
                {
                    continue;
                }

                int index = _randomizer.Next(0, appUsers.Count);


                var user = appUsers.ElementAt(index);

                // assign user is to contact record
                contactRecord.AppUserId = user.Id;
            }

        }

        private void SeedPrayerRequests(UnitOfWork db)
        {
            int numOfRecords = 1500;

            // don't seed contact records if count is >= numOfRecords
            // this will avoid duplication of test data

            //
            // Delete all prayer requests
            //
            //var prs = db.ContactRecordRepository.FindAll(c =>
            //    c.ContactType.Name.Equals("Prayer Request")).ToList();
            //foreach (var pr in prs)
            //{
            //    db.ContactRecordRepository.Delete(pr);
            //}
            //db.Commit();

            var dbSetContactRecord = _dbContext.Set<ContactRecord>();
            int count = dbSetContactRecord.Count(c => c.ContactType.Name.Equals("Prayer Request"));

            if (count >= numOfRecords)
            {
                return;
            }

            var prayerRequests = new List<ContactRecord>();

            // Used to find contact type object
            string contacTypePR = "Prayer Request";

            // Get contact type object that matches 'Prayer Request'

            var dbSetContactType = _dbContext.Set<ContactType>();
            var contactType = dbSetContactType.SingleOrDefault(t => t.Name.Equals(contacTypePR, StringComparison.CurrentCultureIgnoreCase));

            for (int i = 0; i < numOfRecords; i++)
            {
                int ccgId = GetRandomCCGId(db);
                var userId = GetRandomUserIdInCCG(db, ccgId);
                var memberId = GetRandomMemberIdInCCG(db, ccgId);
                var date = GetRandomPrayerRequestDate();
                // randomly make private (only about 5-10%)
                var isPrivate = _randomizer.Next(0, 10) == 0;

                if (userId == null || memberId == 0) continue;

                prayerRequests.Add(new ContactRecord
                {
                    ContactDate = date,
                    Timestamp = date,
                    Private = isPrivate,
                    AppUserId = userId,
                    CCGMemberId = memberId,
                    Subject = GetRandomSubject(),
                    Comments = GetRandomComments(),
                    ContactTypeId = contactType.Id
                });
            }

            foreach (ContactRecord rec in prayerRequests)
            {
                dbSetContactRecord.AddOrUpdate(rec);
            }
        }

        private DateTime GetRandomPrayerRequestDate()
        {
            // get random date
            // make sure we get plenty of prayer requests 2 weeks old or less
            var tsMax = _randomizer.Next(5) == 0 ? 15 : 400;
            var ts = TimeSpan.FromDays(_randomizer.Next(0, tsMax));
            return DateTime.Now.Subtract(ts);
        }

        private string GetRandomUserIdInCCG(UnitOfWork db, int ccgId)
        {
            List<CCGAppUser> users;
            using (var userStore = new UserStore<CCGAppUser>(_dbContext))
            using (var userManager = new ApplicationUserManager(userStore))
            {
                users = userManager.Users.Where(u => u.CcgId == ccgId).OrderBy(u => u.LastName).AsEnumerable().ToList();
            }

            if (users.Count == 0) return null;

            var user = users.ElementAt(_randomizer.Next(0, users.Count()));
            return user.Id;
        }
        private int GetRandomMemberIdInCCG(UnitOfWork db, int ccgId)
        {
            var dbSet = _dbContext.Set<CCGMember>();
            var members = dbSet.Include("CCG").Where(m => m.CcgId == ccgId).ToList();

            if (members.Count() == 0) return 0;

            var member = members.ElementAt(_randomizer.Next(0, members.Count()));
            return member.Id;
        }

        private int GetRandomCCGId(UnitOfWork db)
        {
            // get random ccgs that have users and members assigned
            var dbSet = _dbContext.Set<CCG>();
            var ccgs = dbSet.Where(g => g.AppUsers.Count > 0
                                   && g.CCGMembers.Count > 0).ToList();



            // get random ccg
            CCG ccg = null;
            if (ccgs.Count() > 0)
                ccg = ccgs.ElementAt(_randomizer.Next(0, ccgs.Count()));
            return ccg != null ? ccg.Id : 0;
        }

        private string GetRandomSubject()
        {
            int dummyTextLen = dummyText.Length;
            int numOrChars = _randomizer.Next(10, 100);
            return dummyText.Substring(0, numOrChars);
        }

        private string GetRandomComments()
        {
            int dummyTextLen = dummyText.Length;
            int numOrChars = _randomizer.Next(10, 300);
            return dummyText.Substring(0, numOrChars);
        }

        #endregion
    }
}