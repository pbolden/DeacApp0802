using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;

namespace DeaconCCGManagement.ViewModels
{
    public abstract class MemberViewModelBase
    {
        // Error messages
        protected const string ReguiredErrMsg = "Member's {0} is required.";
        protected const string TooLongErrMsg = "{0} is too long.";

        [Display(Name = "Member ID")]
        public int Id { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        // Concat of first and last name. Not stored in database.
        [Display(Name = "Member")]
        public string FullName
        {
            get { return $"{LastName}, {FirstName}"; }
        }

        // Home or primary phone number
        private string _phoneNumber;
        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "Home Phone")]
        [Phone(ErrorMessage = "Not a valid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber
        {
            get { return FormatHelper.FormatPhoneNumber(_phoneNumber); }
            set { _phoneNumber = value; }
        }

        // Cell phone number
        private string _cellPhoneNumber;
        [Display(Name = "Cell Phone")]
        [DataType(DataType.PhoneNumber)]
        public string CellPhoneNumber
        {
            get { return FormatHelper.FormatPhoneNumber(_cellPhoneNumber); }
            set { _cellPhoneNumber = value; }
        }

        [ScaffoldColumn(false)]
        public string HrefPhoneNumberLink { get; set; }

        [ScaffoldColumn(false)]
        public string HrefCellNumberLink { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "{0} does not appear to be a valid email address.")]
        public string EmailAddress { get; set; }

        [ScaffoldColumn(false)]
        public bool IsUserMemberDeacon { get; set; }

        [ScaffoldColumn(false)]
        public bool HasPhoto { get; set; }

        [ScaffoldColumn(false)]
        public string ImageSrc { get; set; }

        [ScaffoldColumn(false)]
        public string PhotoFileName { get; set; }

        [Display(Name = "CCG")]
        public CCG CCG { get; set; }
    }


    public abstract class DefaultMemberViewModel : MemberViewModelBase
    {

        // Peron's title (e.g. Dr.)
        [StringLength(50, ErrorMessage = TooLongErrMsg)]
        public string Title { get; set; }

        // Person's suffix (e.g. Jr., II)
        [StringLength(50, ErrorMessage = TooLongErrMsg)]
        public string Suffix { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "Address")]
        [StringLength(500, ErrorMessage = TooLongErrMsg)]
        public string Address { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [StringLength(160, ErrorMessage = TooLongErrMsg)]
        public string City { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [StringLength(50, ErrorMessage = TooLongErrMsg)]
        public string State { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "Zip Code")]
        [StringLength(50, ErrorMessage = TooLongErrMsg)]
        public string ZipCode { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        // Date joined Zion
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Display(Name = "Joined Zion Date")]
        public DateTime? DateJoinedZion { get; set; }

        // Date record was initially entered into ACS
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Display(Name = "Joined Entry Date")]
        public DateTime? ZionEntryDate { get; set; }

        // Status of member (Active/Not Active)
        [Display(Name = "Active Member")]
        public bool IsMemberActive { get; set; } = true;

        // Wedding anniversary date
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Display(Name = "Wedding Anniversary")]
        public DateTime? AnniversaryDate { get; set; }

        // Date member was last contacted
        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        [Display(Name = "Last Contact Date")]
        public DateTime? LastDateContacted { get; set; }

        public string ReturnUrl { get; set; }

        // Foreign key properties

        // No longer FK between user/member
        //public string AppUserId { get; set; }

        public int? CcgId { get; set; }
    }

    public class DetailsMemberViewModel : DefaultMemberViewModel
    {
      
    }

    public class CreateMemberViewModel : DefaultMemberViewModel
    {
      
    }

    public class DeleteMemberViewModel : DefaultMemberViewModel
    {
       
    }

    public class EditMemberViewModel : DefaultMemberViewModel
    {
        // Unique value assigned to each member
        [Display(Name = "Member's Id")]
        public int CcgMemberId { get; set; }

        // Unique value assigned to each member by the church
        [Display(Name = "Individual's Id")]
        public string IndividualId { get; set; }
    }

    // Bind is for the multi-select feature. Only bound property
    // values are posted back to the server.
    [Bind(Include = "Id, Selected")]
    public class ListMembersViewModel : MemberViewModelBase
    {
        // For selecting members in the view.
        [Display(Name = "Select")]
        public bool Selected { get; set; }

        public string[] SelectedMemberIds { get; set; }
    }
}