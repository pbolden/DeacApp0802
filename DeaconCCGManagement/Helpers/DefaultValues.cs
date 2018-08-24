using System.Collections.Generic;
using System.Web.Mvc;


namespace DeaconCCGManagement.Helpers
{
    public class DefaultValues
    {
        public static SelectList ItemsPerPageList
        {
            get
            {
                var options = new List<int> { 5, 10, 25, 50, 100};
                return (new SelectList(options, selectedValue: 10));
            }
        }
    }
}