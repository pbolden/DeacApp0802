using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeaconCCGManagement.Models
{
    public class PassAlongContact
    {
        // Since this is a one-to-one relationship with ContactRecord,
        // both tables should use the same primary keys
        [Key, ForeignKey(nameof(ContactRecord))]
        [Display(Name = "Pass Along Contact ID")]
        public int Id { get; set; }

        //[Required(ErrorMessage = "Contact id is required.")]
        //public int ContactRecordId { get; set; }

        public bool Archive { get; set; }

        public DateTime Timestamp { get; set; }

        public bool PassAlongEmailSent { get; set; }

        // One-to-one relationship with ContactRecord
        public virtual ContactRecord ContactRecord { get; set; }
    }
}