using System;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.Models
{
    public class ChangeRequest : CCGMemberBasic
    {
        [Key]
        public int Id { get; set; }

        public DateTime CRDate { get; set; }

        public int CcgMemberId { get; set; }

        public string DeaconId { get; set; }

        public virtual CCGMember CcgMember { get; set; }
        public virtual CCGAppUser Deacon { get; set; }
    }
}