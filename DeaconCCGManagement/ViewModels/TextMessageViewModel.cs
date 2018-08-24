using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class TextMessageViewModel
    {
        public TextMessageContact TextMessageContact { get; set; }

        public bool Bulk { get; set; }

        // For bulk messages
        public IList<BulkContactMember> Members { get; set; }

        [ScaffoldColumn(false)]
        public bool IsTest { get; set; }

        [ScaffoldColumn(false)]
        public bool TextMessageSent { get; set; }

        [Display(Name = "'From' Number for Testing Only")]
        public string TestFromNumber { get; set; }

        [Display(Name = "'To' Number for Testing Only")]
        public string TestToNumber { get; set; }

        public NotificationViewModel StatusNotification { get; set; }

        public bool HasStatusNotification { get; set; }
    }
}