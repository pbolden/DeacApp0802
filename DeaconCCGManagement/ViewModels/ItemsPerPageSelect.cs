using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.ViewModels
{
    public class ItemsPerPageSelect
    {
        public int[] ItemsPerPage { get; set; }
        public string AbsPath { get; set; }
        public NameValueCollection QueryStrings { get; set; }
        public bool ListAllOption { get; set; }
        public string ListAllText { get; set; }
    }
}