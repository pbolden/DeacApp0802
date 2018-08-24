using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeaconCCGManagement.Models
{
    public class AntiXSSTest
    {
        public int Id { get; set; }

        [AllowHtml]
        [DataType(DataType.MultilineText)]
        public string Message { get; set; }

    }
}