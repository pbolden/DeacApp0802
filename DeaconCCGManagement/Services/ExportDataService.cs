using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using ClosedXML.Excel;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.Services
{
    public class ExportDataService : ServiceBase
    {
        private readonly CCGService _ccgService;
        private string _mimeXls;
        private string _mimeCsv;

        public ExportDataService(UnitOfWork unitOfWork) : base(unitOfWork)
        {
            _ccgService = new CCGService(unitOfWork);
            _mimeXls = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            _mimeCsv = "csv";

        }

        public FileResult ExportCCGMembersAsXls(IEnumerable<CCGMember> members)
        {
            string fileName = GetFileNameWithDate("CCG-Members");
            fileName = AddExtXlsx(fileName);
            var dt = new DataTable("CCG Members");

            #region Add data columns
            dt.Columns.AddRange(new DataColumn[25] {
                                            new DataColumn("Id"),
                                            new DataColumn("FirstName"),
                                            new DataColumn("LastName"),
                                            new DataColumn("Title"),
                                            new DataColumn("Suffix"),
                                            new DataColumn("Address"),
                                            new DataColumn("City"),
                                            new DataColumn("State"),
                                            new DataColumn("ZipCode"),
                                            new DataColumn("PhoneNumber"),
                                            new DataColumn("CellNumber"),
                                            new DataColumn("BirthDate"),
                                            new DataColumn("Email"),
                                            new DataColumn("DateJoinedZion"),
                                            new DataColumn("InactiveDate"),
                                            new DataColumn("AnniversaryDate"),
                                            new DataColumn("IsMemberActive"),
                                            new DataColumn("FamilyNumber"),
                                            new DataColumn("EnvelopNumber"),
                                            new DataColumn("FamDistrictDeacon"),
                                            new DataColumn("IndividualId"),
                                            new DataColumn("ZionEntryDate"),
                                            new DataColumn("DateLastChanged"),
                                            new DataColumn("LastDateContacted"),
                                            new DataColumn("CCG")
            });
            #endregion

            foreach (var member in members)
            {
                // Change name of CCG to include deacon name(s)
                _ccgService.SetCCGViewName(member.CCG);

                dt.Rows.Add(member.Id, member.FirstName, member.LastName, member.Title, member.Suffix, member.Address, member.City,
                    member.State, member.ZipCode, member.PhoneNumber, member.CellPhoneNumber, member.BirthDate, member.EmailAddress,
                    member.DateJoinedZion, member.InactiveDate, member.AnniversaryDate, member.IsMemberActive, member.FamilyNumber,
                    member.EnvelopNumber, member.FamDistrictDeacon, member.IndividualId, member.ZionEntryDate, member.DateLastChanged,
                    member.LastDateContacted, member.CCG.CCGName);
            }
            
            return ExportXlsFile(fileName, dt);
        }

        public FileResult ExportContactRecordsAsXls(IEnumerable<ContactRecord> contactRecords)
        {
            string fileName = GetFileNameWithDate("Contact-Records");
            fileName = AddExtXlsx(fileName);
            var dt = new DataTable("Contact Records");

            #region Add data columns
            dt.Columns.AddRange(new DataColumn[14] {
                                            new DataColumn("Id"),
                                            new DataColumn("Member"),
                                            new DataColumn("Deacon"),
                                            new DataColumn("Private"),
                                            new DataColumn("ContactDate"),
                                            new DataColumn("Timestamp"),
                                            new DataColumn("Duration"),
                                            new DataColumn("Subject"),
                                            new DataColumn("Comments"),
                                            new DataColumn("PassAlong"),
                                            new DataColumn("PassAlongComments"),
                                            new DataColumn("PassAlongFollowUpComments"),
                                            new DataColumn("Archive"),
                                            new DataColumn("ContactType"),
            });
            #endregion

            foreach (var record in contactRecords)
            {
                dt.Rows.Add(record.Id, record.MemberFullName, record.DeaconFullName, record.Private, record.ContactDate,
                    record.Timestamp, record.Duration, record.Subject, record.Comments, record.PassAlong, record.PassAlongComments,
                    record.PassAlongFollowUpComments, record.Archive, record.ContactType.Name);
            }
            
            return ExportXlsFile(fileName, dt);
        }

        public FileResult ExportAppUsersAsXls()
        {
            string fileName = GetFileNameWithDate("CCG-Management-App-Users");
            fileName = AddExtXlsx(fileName);
            var dt = new DataTable("App Users");

            #region Add data columns
            dt.Columns.AddRange(new DataColumn[10] {
                                            new DataColumn("Id"),
                                            new DataColumn("FirstName"),
                                            new DataColumn("LastName"),
                                            new DataColumn("ZionEmail"),
                                            new DataColumn("Email"),
                                            new DataColumn("PhoneNumber"),
                                            new DataColumn("CellNumber"),
                                            new DataColumn("ChangeRequestManager"),
                                            new DataColumn("CCG"),
                                            new DataColumn("Role(s)"), 
            });
            #endregion

            foreach (var appUser in unitOfWork.AppUserRepository.FindAll())
            {
                // Change name of CCG to include deacon name(s)
                _ccgService.SetCCGViewName(appUser.CCG);

                var roles = unitOfWork.AppUserRepository.GetUserRoles(appUser.Id);
                var sb = new StringBuilder();
                foreach (var role in roles)
                {
                    sb.Append(role);
                    int index = roles.IndexOf(role);
                    if (index < roles.Count - 1) sb.Append(", ");
                }

                dt.Rows.Add(appUser.Id, appUser.FirstName, appUser.LastName, appUser.Email, appUser.EmailAddress,
                    appUser.PhoneNumber, appUser.CellNumber, appUser.ChangeRequestManager, appUser.CCG.CCGName, sb.ToString());

            }

            throw new NotImplementedException();
        }

        private FileResult ExportXlsFile(string fileName, DataTable dt)
        {
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);

                    var file = new FileContentResult(stream.ToArray(), _mimeXls);
                    file.FileDownloadName = fileName;

                    return file;
                }
            }
        }

        public FileResult ExportCCGMembersAsCsv(IEnumerable<CCGMember> members)
        {
            string fileName = GetFileNameWithDate("CCG-Members");
            fileName = AddExtCsv(fileName);
            var dt = new DataTable("CCG Members");

            #region Add data columns
            dt.Columns.AddRange(new DataColumn[25] {
                                            new DataColumn("Id"),
                                            new DataColumn("FirstName"),
                                            new DataColumn("LastName"),
                                            new DataColumn("Title"),
                                            new DataColumn("Suffix"),
                                            new DataColumn("Address"),
                                            new DataColumn("City"),
                                            new DataColumn("State"),
                                            new DataColumn("ZipCode"),
                                            new DataColumn("PhoneNumber"),
                                            new DataColumn("CellNumber"),
                                            new DataColumn("BirthDate"),
                                            new DataColumn("Email"),
                                            new DataColumn("DateJoinedZion"),
                                            new DataColumn("InactiveDate"),
                                            new DataColumn("AnniversaryDate"),
                                            new DataColumn("IsMemberActive"),
                                            new DataColumn("FamilyNumber"),
                                            new DataColumn("EnvelopNumber"),
                                            new DataColumn("FamDistrictDeacon"),
                                            new DataColumn("IndividualId"),
                                            new DataColumn("ZionEntryDate"),
                                            new DataColumn("DateLastChanged"),
                                            new DataColumn("LastDateContacted"),
                                            new DataColumn("CCG")
            });
            #endregion

            foreach (var member in members)
            {
                // Change name of CCG to include deacon name(s)
                _ccgService.SetCCGViewName(member.CCG);

                dt.Rows.Add(member.Id, member.FirstName, member.LastName, member.Title, member.Suffix, member.Address, member.City,
                    member.State, member.ZipCode, member.PhoneNumber, member.CellPhoneNumber, member.BirthDate, member.EmailAddress,
                    member.DateJoinedZion, member.InactiveDate, member.AnniversaryDate, member.IsMemberActive, member.FamilyNumber,
                    member.EnvelopNumber, member.FamDistrictDeacon, member.IndividualId, member.ZionEntryDate, member.DateLastChanged,
                    member.LastDateContacted, member.CCG.CCGName);
            }

            return ExportCsvFile(fileName, dt);
        }

        private FileResult ExportCsvFile(string fileName, DataTable dt)
        {   
            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dt);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    var file = new FileContentResult(stream.ToArray(), _mimeXls);
                    file.FileDownloadName = fileName;
                    return file;
                }
            }
        }

        private string GetFileNameWithDate(string fileName)
        {
            return $"{fileName}-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}";
        }

        private string AddExtXlsx(string fileName)
        {
            return $"{fileName}.xlsx";
        }

        private string AddExtCsv(string fileName)
        {
            return $"{fileName}.csv";
        }
    }
}
 
 
 
 
 
 
 
 
 
 
 
 