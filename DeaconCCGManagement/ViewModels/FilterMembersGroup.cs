using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeaconCCGManagement.enums;

namespace DeaconCCGManagement.ViewModels
{
    public class FilterMembersGroup
    {
        public MemberFilter MemberFilter { get; set; }
        public SelectList CCGs { get; set; }
        public bool FilterCCGs { get; set; }
    }
}