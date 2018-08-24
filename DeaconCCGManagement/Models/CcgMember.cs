using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeaconCCGManagement.Models
{
    /// <summary>
    /// Base class for the CCGMember and ChangeRequest.
    /// </summary>
     public abstract class CCGMemberBasic
    {
        // Error messages
        private const string ReguiredErrMsg = "Member's {0} is required.";
        private const string TooLongErrMsg = "{0} is too long.";

        [StringLength(100, ErrorMessage = TooLongErrMsg)]
        public string RealmID { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "First Name")]
        [StringLength(160, ErrorMessage = TooLongErrMsg)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "Last Name")]
        [StringLength(160, ErrorMessage = TooLongErrMsg)]
        public string LastName { get; set; }

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

        // Home or primary phone number
        [Display(Name = "Home Phone")]
        [Phone(ErrorMessage = "Not a valid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }

        // Cell phone number
        [Display(Name = "Cell Phone")]
        [Phone(ErrorMessage = "Not a valid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string CellPhoneNumber { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "{0} does not appear to be a valid email address.")]
        public string EmailAddress { get; set; }

        // Date joined Zion
        [Display(Name = "Joined Zion Date")]
        public DateTime? DateJoinedZion { get; set; }

        // Date the member was marked as inactive.
        // We will need logic to set this when whenever the
        // IsMemberActive property is set to false.
        [Display(Name = "Inactive Date")]
        public DateTime? InactiveDate { get; set; }

        // Wedding anniversary date
        [Display(Name = "Wedding Anniversary")]
        public DateTime? AnniversaryDate { get; set; }

        // Status of member (Active/Not Active)
        [Display(Name = "Active Member")]
        public bool IsMemberActive { get; set; } = true;

        // Name of the photo file stored in blob
        public string PhotoFileName { get; set; }
    }

    /// <summary>
    /// Models CCG members. Data sourced from central database.
    /// </summary>
    public class CCGMember : CCGMemberBasic
    {
        // Error messages
        private const string ReguiredErrMsg = "Member's {0} is required.";
        private const string TooLongErrMsg = "{0} is too long.";

        [Key]
        [Display(Name = "Member ID")]
        public int Id { get; set; }
       
        public string FamilyNumber { get; set; }
        public string EnvelopNumber { get; set; }
        
        // District deacon assigned
        [Display(Name = "Deacon Name")]
        public string FamDistrictDeacon { get; set; }
        
        // Unique value assigned to each member
        public string IndividualId { get; set; }

        // Date record was initially entered into ACS
        [Display(Name = "Zion Entry Date")]
        public DateTime? ZionEntryDate { get; set; }

        // Date record was last modified in ACS
        [Display(Name = "Last Modified Date")]
        public DateTime? DateLastChanged { get; set; }

        // Date member was last contacted
        [Display(Name = "Last Contact Date")]
        public DateTime? LastDateContacted { get; set; }

        // Foreign key properties
        public int? CcgId { get; set; }

        // Navigational properties
        public virtual CCG CCG { get; set; }
       
        public virtual ICollection<ContactRecord> ContactRecords { get; set; }
    }
}