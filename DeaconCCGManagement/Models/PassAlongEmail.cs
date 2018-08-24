using System;

namespace DeaconCCGManagement.Models
{
    public class PassAlongEmail
    {
        public DateTime ContactDate { get; set; }
        public string DeaconName { get; set; }
        public string MemberEmail { get; set; }
        public string MemberPhoneNumber { get; set; }
        public string EmailSubject { get; set; }
        public string ContactSubject { get; set; }
        public string Comments { get; set; }
        public string ContactType { get; set; }
    }
}