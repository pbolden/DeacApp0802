using DeaconCCGManagement.enums;

namespace DeaconCCGManagement.ViewModels
{
    public class ActionMethodParams
    {
        public int? MemberId { get; set; }
        public string UserId { get; set; }
        public int? CCGId { get; set; }
        public int? ItemsPerPage { get; set; }
        public int? Page { get; set; }
        public bool GetAll { get; set; }
        public bool ListAll { get; set; }
        public bool Archive { get; set; }
        public string Query { get; set; }
        public ContactsSort ContactsSort { get; set; }
        public DateRangeFilter DateRangeFilter { get; set; }
    }
}