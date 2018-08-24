using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;

namespace DeaconCCGManagement.DAL
{
    /// <summary>
    /// Comparer for IdentityRole. Used to sort roles alphabetically.
    /// </summary>
    public class IdentityRoleCompareByName : IComparer<IdentityRole>
    {
        public int Compare(IdentityRole x, IdentityRole y)
        {
            return string.Compare(x.Name, y.Name,
                StringComparison.CurrentCultureIgnoreCase);
        }
    }
}