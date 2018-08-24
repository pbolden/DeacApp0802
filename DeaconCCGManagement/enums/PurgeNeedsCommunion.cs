using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.enums
{
    public enum PurgeNeedsCommunion
    {
        [Display(Name = "Over 60 Days")]
        Over60Days,

        [Display(Name = "Over 6 Months")]
        Over6Months,

        [Display(Name = "Over 1 Year")]
        Over1Year,

        [Display(Name = "Do Not Purge")]
        None
    }
}