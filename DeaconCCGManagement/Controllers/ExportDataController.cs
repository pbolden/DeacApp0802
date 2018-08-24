using System.Linq;
using System.Web.Mvc;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Services;

namespace DeaconCCGManagement.Controllers
{
    public class ExportDataController : ControllerBase
    {
        private ExportDataService _service;
        private CcgMembersService _membersService;
        private ContactRecordsService _contactRecordsService;

        public ExportDataController()
        {
            _service = new ExportDataService(unitOfWork);
            _membersService = new CcgMembersService(unitOfWork);
            _contactRecordsService = new ContactRecordsService(unitOfWork);
        }

        public FileResult CCGMembers(int? ccgId, bool getAll = false,
            string query = null, bool allAccess = false,
            int? memberFilter = (int)MemberFilter.None,
            int? dataDownloadFileType = (int)ExportDataFileType.Xls)
        {
            var fileType = dataDownloadFileType != null ? (ExportDataFileType)dataDownloadFileType : ExportDataFileType.Xls;

            // Get members collection from db
            // TODO  action result not needed here
            ActionResult actionResult;
            var members = _membersService.GetMembersList(ccgId, getAll, query, allAccess,
                User.Identity.Name, out actionResult).ToList();

            // Filter members
            var filter = memberFilter != null ? (MemberFilter)memberFilter : MemberFilter.None;
            _membersService.FilterMembers(ref members, filter, unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name));

            switch (fileType)
            {
                case ExportDataFileType.Xls:
                    // Export excel file to user's browser
                    return _service.ExportCCGMembersAsXls(members);
                case ExportDataFileType.Csv:
                    break;
            }
            //TODO temporary
            return new FileContentResult(new byte[] { }, "");
        }

        public FileResult ContactRecords(int? memberId, int? ccgId,
            bool getAll = false, bool archive = false, string query = null,
            int? dateRangeFilter = (int)DateRangeFilter.SixMonths,
            int? dataDownloadFileType = (int)ExportDataFileType.Xls)
        {

            var fileType = dataDownloadFileType != null ? (ExportDataFileType)dataDownloadFileType 
                : ExportDataFileType.Xls;
            var dateFilter = dateRangeFilter == null ? DateRangeFilter.ThreeMonths 
                : (DateRangeFilter)dateRangeFilter;

            switch (fileType)
            {
                case ExportDataFileType.Xls:

                    // Get contact records
                    ActionResult actionResult;
                    var user = unitOfWork.AppUserRepository.FindUserByEmail(User.Identity.Name);

                   
                    var contactRecords = _contactRecordsService.GetContactRecords(memberId, ccgId, getAll, 
                        archive,  user, dateFilter, query, out actionResult);

                    contactRecords = _contactRecordsService.SearchContactRecords(query, contactRecords);
                    contactRecords = _contactRecordsService.RemovePrayerRequests(contactRecords);

                    // Filter contact records by date range (eg. last 6 months)
                    //_contactRecordsService.FilterContactsByDateRange(contactRecords.ToList(), dateFilter);

                    // Export contact records to browser as xls file
                    return _service.ExportContactRecordsAsXls(contactRecords);
                    

                case ExportDataFileType.Csv:
                    break;
            }

            //TODO temporary
            return new FileContentResult(new byte[] { }, "");
        }

        public FileResult AppUsers()
        {
            return new FileContentResult(new byte[] { }, "");
        }
    }
}