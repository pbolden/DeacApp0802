using System.Web.Mvc;
using PagedList;

namespace DeaconCCGManagement.ViewModels
{
    public class PrayerRequestViewModel : ContactRecordViewModel
    {
        public string ReturnUrl { get; set; }
    }

    public class PrayerRequestListViewModel
    {
        public IPagedList<PrayerRequestViewModel> PrayerRequests { get; set; }
        public ActionMethodParams Params { get; set; }
        public SelectList CCGs { get; set; }
        public string CcgName { get; set; }
        
    }
}