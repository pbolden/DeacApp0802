using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DeaconCCGManagement.ViewModels
{
    public class PhoneCallContact
    {
        [ScaffoldColumn(false)]
        public int MemberId { get; set; }

        [ScaffoldColumn(false)]
        public string MemberFullName { get; set; }

        [ScaffoldColumn(false)]
        public string PhoneNumber { get; set; }

        [ScaffoldColumn(false)]
        public string HrefPhoneNumberLink { get; set; }

        [Display(Name = "Pass Along")]
        public bool PassAlong { get; set; }

        [Display(Name = "Comments")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "{0} is a required field")]
        [Display(Name = "Date")]
        public DateTime CallDateTime { get; set; }

        [Required(ErrorMessage = "Please enter the call duration.")]
        [Display(Name = "Call Duration")]
        public TimeSpan CallDuration { get; set; }

        [ScaffoldColumn(false)]
        public int ContactTypeId { get; set; }

        [Display(Name = "Contact Type")]
        public SelectList ContactTypes { get; set; }
    }
}