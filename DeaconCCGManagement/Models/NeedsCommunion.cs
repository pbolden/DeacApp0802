using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.Models
{
    [Table("NeedsCommunion")]
    public class NeedsCommunion
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual CCGMember Member { get; set; }
    }
}