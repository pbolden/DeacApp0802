using System;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class TestMemberIndexViewModel
    {
        // Error messages
        private const string ReguiredErrMsg = "Member's {0} is required.";
        private const string TooLongErrMsg = "{0} is too long.";

        public int Id { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "Last Name")]
        [StringLength(160, ErrorMessage = TooLongErrMsg)]
        public string LastName { get; set; }

        [Required(ErrorMessage = ReguiredErrMsg)]
        [Display(Name = "First Name")]
        [StringLength(160, ErrorMessage = TooLongErrMsg)]
        public string FirstName { get; set; }

        // Concat of first and last name. Not stored in database.
        [Display(Name = "Member's Name")]
        public string FullName
        {
            get { return String.Format("{0} {1}", FirstName, LastName); }
        }
    }
}