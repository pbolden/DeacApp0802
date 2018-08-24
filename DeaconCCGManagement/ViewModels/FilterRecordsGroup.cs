using System.Web.Mvc;
using DeaconCCGManagement.enums;

namespace DeaconCCGManagement.ViewModels
{
    public class FilterRecordsGroup
    {
        public DateRangeFilter DateRangeFilter { get; set; }
        public SelectList CCGs { get; set; }
        public bool FilterCCGs { get; set; }

    }
}