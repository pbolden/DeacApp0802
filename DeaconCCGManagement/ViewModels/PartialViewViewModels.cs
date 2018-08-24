using System;
using System.Web.Mvc;

namespace DeaconCCGManagement.ViewModels
{
    public abstract class BasicPartialViewModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public int? MemberId { get; set; }
    }

    public class CreateButtonViewModel : BasicPartialViewModel
    {
      
    }

    public class PartialViewModel : BasicPartialViewModel
    {
        public ActionMethodParams Params { get; set; }
    }

    public class PrayerRequestPartialViewModel : PartialViewModel
    {
        public int PrayerRequestsCount { get; set; }
        public SelectList CCGs { get; set; }
        public bool GetAll { get; set; }
    }
}