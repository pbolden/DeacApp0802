using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.Test_Data;
using Microsoft.AspNet.Identity.EntityFramework;


namespace DeaconCCGManagement.DAL
{
    /// <summary>
    /// Context initializer. Seeds database with test data.
    /// </summary>
    public class CcgDbContextInitializer : DropCreateDatabaseIfModelChanges<CcgDbContext>
    {
        // Init dependencies:
        // DropCreateDatabaseAlways
        // CreateDatabaseIfNotExists
        // DropCreateDatabaseIfModelChanges

        private static Random _randomizer = new Random();

        // Used to seed app users and assign their roles.
        private List<CCGAppUser> _appUsers = new List<CCGAppUser>();

        // Used to seed members and contact records.
        private List<CCGMember> _members = new List<CCGMember>();

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

        protected override void Seed(CcgDbContext dbContext)
        {
         






            // No using block to let the Initializer dispose of the context
            //var db = new UnitOfWork();

            //SeedRoles(db);
            //SeedCCGs(db);
            //SeedContactTypes(db);

            //dbContext.SaveChanges();
            ////db.Commit(); // Prevents concurrency issues.
            //SeedAppUsers(db);
            ////db.Commit();
            //dbContext.SaveChanges();
            //SeedAppUsersRoles(db);
            ////db.Commit();
            //dbContext.SaveChanges();

            //SeedMembers(db);
            //SeedContactRecords(db);
            ////db.Commit();
            //dbContext.SaveChanges();


            //AssignUserIdToContactRecord(db);
            ////db.Commit();
            //dbContext.SaveChanges();


            //SeedPrayerRequests(db);
            ////db.Commit();
            //dbContext.SaveChanges();

        }

        private void SeedRoles(UnitOfWork db)
        {
            // Seed roles to database
            db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.Admin });
            db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.DeaconLeadership });
            db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.Deacon });
            db.RoleRepository.AddRole(new IdentityRole { Name = AuthHelper.Pastor });
        }

        private void SeedCCGs(UnitOfWork db)
        {
            string ccgName;
            for (var i = 1; i < 100; i++)
            {
                ccgName = $"CCG{i:d2}"; // same as String.Format("CCG{0:d2},i")
                db.CCGRepository.Add(new CCG { CCGName = ccgName });
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

            contactTypes.ForEach(c => db.ContactTypeRepository.Add(
                new ContactType { Name = c }));
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
                db.AppUserRepository.AddUser(appUser);

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

            foreach (var appUser in _appUsers)
            {
                switch (appUser.Email)
                {
                    case "JLATIMORE@zionmbc.org":
                        db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.DeaconLeadership);
                        break;
                    case "oneal@zionmbc.org":
                        db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Pastor);

                    break;
                        case "DFREEMAN@zionmbc.org":
                        case "pbolden@zionmbc.org":
                        db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Admin);
                        db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Deacon);
                        db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.DeaconLeadership);
                        break;
                    default:
                        db.AppUserRepository.AddUserToRole(appUser.Id, AuthHelper.Deacon);
                        break;
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
            var ccg = db.CCGRepository.Find(c => c.CCGName == ccgName);
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
                    BirthDate = member.DateOfBirth,
                    DateJoinedZion = member.DateJoined,
                    ZionEntryDate = member.EntryDate,
                    DateLastChanged = member.DateLastChanged,
                    CcgId = GetTestCcgId(member.CCG, db),
                });
            }

            foreach (var member in _members)
            {
                db.MemberRepository.Add(member);
            }
        }

        private string GetAppUserId(string famDistrictDeacon, UnitOfWork db)
        {
            var names = GetTestAppUserNames(famDistrictDeacon);
            string firstName = names["First Name"];
            string lastName = names["Last Name"];
            var appUser = db.AppUserRepository.Find(u => u.FirstName == firstName &&
                                                         u.LastName == lastName);
            
            return appUser?.Id;
        }

        #region Seed contact records
        private void SeedContactRecords(UnitOfWork db)
        {
            int numOfRecords = 3000;

            IList<ContactRecord> seedContactRecords = new List<ContactRecord>();
            string longComment =
                "Wants to try his hand at church planting. Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident,sunt in culpa qui officia deserunt mollit anim id est laborum. quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse consequat.Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.Excepteur sint occaecat cupidatat non proident,sunt in culpa qui officia deserunt mollit anim id est laborumquis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo";

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
                    CCGMemberId = GetRandomMemberId(),
                    ContactTypeId = GetRandomContactTypeId(),
                });
            }


            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "All is well. Member and family in good standing.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Have been away for a while dealing with family difficulties, need contact from pastor.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(9),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "All is well.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(7),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = " Excepteur sint occaecat cupidatat non proident laborum.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "All is well.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Labore et dolore magna aliqua.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(15),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Cillum dolore eu fugiat.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(12),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Fugiat nulla pariatur.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(2),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Fofficia deserunt mollit.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(25),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Tempor incididunt ut labore.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(25),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Incididunt ut labore et dolore magna aliqua.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(25),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Incididunt ut labore et dolore magna aliqua.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(20),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Fofficia deserunt mollit.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(21),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Exercitation ullamco laboris.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(12),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Pebbles in hospital.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(22),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Wedding anniversasry this weekend.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Needs anger management class.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Gonna get that mouse!",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Gotta go for a while.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Proident, sunt in culpa qui.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Lorem ipsum dolor sit amet.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Consectetur adipisicing elit, sed do eiusmod.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(20),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Ut enim ad minim veniam.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(20),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Nostrud exercitation ullamco.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(28),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Grandchild due Sunday night.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(60),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Death in family.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(15),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Incididunt ut labore et dolore magna aliqua.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(20),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Eserunt mollit anim id est laborum.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(13),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Velit esse cillum dolore eu fugiat nulla.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Needs anger management class.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Needs counseling.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "In desperate need of prayer!",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Gotta go for a while.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Proint in culpa qui.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Laboris nisi ut aliquip.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Nostrud exercitation ullamco laboris nisi ut",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Et dolore magna aliqua.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Pro, un in cula qui.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Has got a fighting chance.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Recently quit smoking.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(15),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Got a new house. Need to update address information.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(18),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Have issues with his mother.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Has issues with teamwork at work.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(45),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Needs financial management class.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(26),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(50),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Aqui officia deser.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(21),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Has some attachment issues.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Has a need to enter drug counseling.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(45),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Has irrational feelings about Creation.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(36),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(15),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Just got married.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(31),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Traveling on missions abroad.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Gone hunting, will return next week.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(15),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Needs anger management class.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            }); 

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(12),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Having personal revival.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(14),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Nisi ut aliquip, ex ea commodo consequat.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Prudent, punt in pulpa aqui.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "In hospice needs prayer.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Gexercitation ullamco laboris nisi ut aliquip!",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "otta tago foir ak whilp.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Poiden, sunit uin culgopa quoi.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Sed do eiusmod, quis nostrud.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = true,
                Subject = "Emergency contact",
                Comments = "Cancer diagnosis, family in crisis.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "runollit animid estaborum.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "New NFL contract, wants to tithe.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Recently unemployed.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(25),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Needs prayer, stuck in difficult situation.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine, excelling at new job.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(40),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Going into convalescence home.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Everything's a joke to him.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Things are getting hairy.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(45),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "In studio preparing for new album.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Enjoying retirement.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Trying a new career. Needs prayer.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "A real prince.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Recently divorced, putting things back together.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(55),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "Needs anger management class.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Everything is fine.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(20),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Gonna get that house!",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Needs prayer. Having issues with blood.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Going through identity crisis.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(5),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Needs conflict resolution class.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Entering retirement, enjoying new hobby.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Please take off contact list.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(19),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Ut enim ad minim veniam.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Doing a little writing.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(15),
                PassAlong = true,
                Subject = "Quarterly contact",
                Comments = "At new church, please take off contact list.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Uncle Jesse doing much better now, thanks for prayer.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(10),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Enjoying career in law enforcement. Dog died, is devastated.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Sha boing boing.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = true,
                Subject = "Needs Counseling",
                Comments = "Considering divorce.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(25),
                PassAlong = false,
                Subject = "First contact",
                Comments = "Going through transformation.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(16),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "Retirement suits him.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Emergency",
                Comments = "I've lost my game and don't know where to find it!",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(11),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = "In need of prayer.",
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            seedContactRecords.Add(new ContactRecord
            {
                ContactDate = GetRandomDate(),
                Timestamp = GetRandomTimeStamp(),
                Duration = TimeSpan.FromMinutes(30),
                PassAlong = false,
                Subject = "Quarterly contact",
                Comments = longComment,
                CCGMemberId = GetRandomMemberId(),
                ContactTypeId = GetRandomContactTypeId(),
            });

            foreach (ContactRecord rec in seedContactRecords)
                db.ContactRecordRepository.Add(rec);
        }

        private DateTime GetRandomDate()
        {
            int year = _randomizer.Next(2015, 2018);
            int month = _randomizer.Next(1, 13);
            int day = _randomizer.Next(1, 29);

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

        private int GetRandomMemberId()
        {
            return _randomizer.Next(1, _members.Count);
        }

        private int GetRandomContactTypeId()
        {
            return _randomizer.Next(1, 17);
        }

        private void AssignUserIdToContactRecord(UnitOfWork db)
        {
            // get all contact records
            var contactRecords = db.ContactRecordRepository.FindAll();
            foreach (var contactRecord in contactRecords)
            {
                // get member obj
                var member = db.MemberRepository.FindMemberById(contactRecord.CCGMemberId);
                
                // get ccg obj
                var ccg = db.CCGRepository.Find(g => g.Id == member.CcgId);

                // randomly select from CcgUsers collection
                int index = _randomizer.Next(0, ccg.AppUsers.Count);
                var user = ccg.AppUsers.ElementAt(index);
               
                // assign user is to contact record
                contactRecord.AppUserId = user.Id;
            }

        }

        private void SeedPrayerRequests(UnitOfWork db)
        {
            int numOfRecords = 1500;

            var prayerRequests = new List<ContactRecord>();

            // Used to find contact type object
            string contacTypePR = "Prayer Request";

            // Get contact type object that matches 'Prayer Request'
            var contactType = db.ContactTypeRepository
                .Find(t => t.Name.Equals(contacTypePR, StringComparison.CurrentCultureIgnoreCase));

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
                db.ContactRecordRepository.Add(rec);

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
            var users = db.AppUserRepository.FindUsers(u => u.CcgId == ccgId);
            if (users.Count() == 0) return null;

            var user = users.ElementAt(_randomizer.Next(0, users.Count()));
            return user.Id;
        }
        private int GetRandomMemberIdInCCG(UnitOfWork db, int ccgId)
        {
            var members = db.MemberRepository.FindMembers(m => m.CcgId == ccgId);

            if (members.Count() == 0) return 0;

            var member = members.ElementAt(_randomizer.Next(0, members.Count()));
            return member.Id;
        }

        private int GetRandomCCGId(UnitOfWork db)
        {
            // get random ccg
            var ccgs = db.CCGRepository.FindAll();

            // only use the first 30 CCGs
            var ccg = ccgs.ElementAt(_randomizer.Next(0, 30));
            return ccg.Id;
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