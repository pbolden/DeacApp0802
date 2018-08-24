using System.ComponentModel.DataAnnotations;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Models;
using PagedList;

namespace DeaconCCGManagement.ViewModels
{
    public abstract class AppUserViewModelBase
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Zion Email")]
        [EmailAddress(ErrorMessage = "{0} does not appear to be a valid email address.")]
        public string SharePointEmail { get; set; }

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "{0} does not appear to be a valid email address.")]
        public string EmailAddress { get; set; }

        private string _phoneNumber;
        [Display(Name = "Phone Number")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber {
            get { return FormatHelper.FormatPhoneNumber(_phoneNumber); }
            set { _phoneNumber = value; }
        }

        private string _cellNumber;
        [Display(Name = "Cell Number")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string CellNumber
        {
            get { return FormatHelper.FormatPhoneNumber(_cellNumber); }
            set { _cellNumber = value; }
        }

        [ScaffoldColumn(false)]
        public string HrefPhoneNumberLink { get; set; }

        [ScaffoldColumn(false)]
        public string HrefCellNumberLink { get; set; }

        [Display(Name = "Change Request Manager")]
        public bool ChangeRequestManager { get; set; }

        // Concat of first & last names
        [Display(Name = "Name")]
        public string FullName => $"{FirstName} {LastName}";

        [ScaffoldColumn(false)]
        public bool HasPhoto { get; set; }

        [ScaffoldColumn(false)]
        public string ImageSrc { get; set; }

        [ScaffoldColumn(false)]
        public string PhotoFileName { get; set; }

        [ScaffoldColumn(false)]
        public string ReturnUrl { get; set; }        

        public int? CcgId { get; set; }
        public virtual CCG CCG { get; set; }
    }

    public class CreateCcgAppUserViewModel : AppUserViewModelBase
    {
        [Display(Name = "Roles")]
        public EditRolesViewModel EditRoles { get; set; } = new EditRolesViewModel();
    }

    public class DeleteCcgAppUserViewModel : AppUserViewModelBase
    {

    }

    public class DetailsCcgAppUserViewModel : AppUserViewModelBase
    {
      
    }

    public class EditCcgAppUserViewModel : AppUserViewModelBase
    {
        [Display(Name = "Roles")]
        public EditRolesViewModel EditRoles { get; set; } = new EditRolesViewModel();
    }

    public class CcgAppUserViewModel : AppUserViewModelBase
    {
        
    }

    public class ListCcgAppUsersViewModel
    {
        public IPagedList<CcgAppUserViewModel> AppUsers { get; set; }
        public ActionMethodParams Params { get; set; }
        public AppUserFilter AppUserFilter { get; set; }
    }

    public class EditRolesViewModel
    {
        [Display(Name = "Administrator")]
        public bool IsAdmin { get; set; }

        [Display(Name = "Deacon Leadership")]
        public bool IsDeaconLeadership { get; set; }

        [Display(Name = "Deacon")]
        public bool IsDeacon { get; set; }

        [Display(Name = "Pastor")]
        public bool IsPastor { get; set; }
    }
  
}