using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.Comparers
{
    public class CCGMemberEqComparer : IEqualityComparer<CCGMember>
    {
        public bool Equals(CCGMember x, CCGMember y) => x.Id == y.Id;

        public int GetHashCode(CCGMember obj) => obj.Id.GetHashCode();
    }
}