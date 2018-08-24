using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using PagedList;

namespace DeaconCCGManagement.ViewModels
{
    public class ContactRecordViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Member")]
        public string MemberFullName { get; set; }

        [Display(Name = "Deacon")]
        public string DeaconFullName { get; set; }

        [Display(Name = "Private Comments")]
        public bool Private { get; set; }

        [Display(Name = "Subject")]
        [StringLength(160, ErrorMessage = "{0} is too long!")]
        public string Subject { get; set; }

        [Display(Name = "Comments")]
        [StringLength(1000, ErrorMessage = "{0} is too long!")]
        [DataType(DataType.MultilineText)]
        public string Comments { get; set; }

        [Display(Name = "Pass Along")]
        public bool PassAlong { get; set; }

        // Pass along comments to pastor/leadership
        [Display(Name = "Deacon's Message")]
        [StringLength(1000, ErrorMessage = "{0} is too long!")]
        [DataType(DataType.MultilineText)]
        public string PassAlongComments { get; set; }

        // Pass along follow up comments
        [Display(Name = "Follow Up Comments")]
        [StringLength(1000, ErrorMessage = "{0} is too long!")]
        [DataType(DataType.MultilineText)]
        public string PassAlongFollowUpComments { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [ScaffoldColumn(false)]
        public int ContactTypeId { get; set; }

        [Display(Name = "Contact Type")]
        public ContactType ContactType { get; set; }

        // Date of Contact
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Required(ErrorMessage = "{0} is a required field")]
        [Display(Name = "Date")]
        public DateTime ContactDate { get; set; }

        // Duration of a contact.
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:hh}:{0:mm}")]
        [Display(Name = "Duration (hr:min)")]
        public TimeSpan? Duration { get; set; }

        public string ReturnUrl { get; set; }

        public int CCGMemberId { get; set; }
        public string AppUserId { get; set; }

        public virtual CCGMember Member { get; set; }
        public virtual CCGAppUser AppUser { get; set; }
    }

    public class ListContactRecordViewModel
    {
        public IPagedList<ContactRecordViewModel> ContactRecords { get; set; }
        public ActionMethodParams Params { get; set; }
        public SelectList CCGs { get; set; }
        public string CcgName { get; set; }
    }

    public class CreateContactRecordViewModel : ContactRecordViewModel
    {
        [Display(Name = "Contact Type")]
        public SelectList ContactTypes { get; set; }
    }

    public class DeleteContactRecordViewModel : ContactRecordViewModel
    {

    }

    public class EditContactRecordViewModel : ContactRecordViewModel
    {
        [Display(Name = "Contact Type")]
        public SelectList ContactTypes { get; set; }
    }

    public class DetailsContactRecordViewModel : ContactRecordViewModel
    {

    }
}