using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.enums
{
    public enum CommunionDateRange
    {
        [Display(Name = "Last 30 Days")]
        Last30Days,

        [Display(Name = "Last 60 Days")]
        Last60Days,

        [Display(Name = "Last 90 Days")]
        Last90Days,

        [Display(Name = "Last 6 Months")]
        Last6Months,

        [Display(Name = "Last 12 Months")]
        LastYear,

        [Display(Name = "All")]
        None
    }
}