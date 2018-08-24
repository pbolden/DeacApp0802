using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.enums
{
    public enum DateRangeFilter
    {
        [Display(Name = "Last Two Weeks")]
        TwoWeeks,

        [Display(Name = "Last 30 Days")]
        LastMonth,

        [Display(Name = "Last 60 Days")]
        LastTwoMonths,

        [Display(Name = "Last 3 Months")]
        ThreeMonths,

        [Display(Name = "Last 6 Months")]
        SixMonths,

        [Display(Name = "Last 12 Months")]
        LastYear,

        [Display(Name = "All")]
        None
    }
}