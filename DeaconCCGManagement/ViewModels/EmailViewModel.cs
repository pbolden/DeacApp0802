using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class EmailViewModel
    {
        public EmailContact EmailContact { get; set; }

        public bool Bulk { get; set; }

        // For bulk messages
        public IList<BulkContactMember> Members { get; set; }

        [Display(Name = "To-Email for Testing Only")]
        public string TestToEmail { get; set; }

        public bool IsTesting { get; set; }

        public NotificationViewModel StatusNotification { get; set; }

        public bool HasStatusNotification { get; set; }


    }
}