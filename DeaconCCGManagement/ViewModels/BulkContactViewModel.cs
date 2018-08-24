using System.Collections.Generic;
using System.Web.Mvc;

namespace DeaconCCGManagement.ViewModels
{
    public class BulkContactViewModel
    {
        public IList<BulkContactMember> Members { get; set; }

        public CreateContactRecordViewModel CreateContactVM { get; set; }

        public int? ContactTypeId { get; set; }

        public SelectList ContactTypes { get; set; }

        public bool NoMembersSelected { get; set; }
    }

    public class BulkContactMember
    {
        public int MemberId { get; set; }

        public string MemberFullName { get; set; }
    }
}