using DeaconCCGManagement.enums;

namespace DeaconCCGManagement.ViewModels
{
    /// <summary>
    /// Used for off-line development sign-in view only.
    /// </summary>
    public class DevelopmentSignInViewModel
    {

        //[RegularExpression(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,4}",
        // ErrorMessage = "{0} does not appear to be a valid email address.")]

        //[EmailAddress(ErrorMessage = "{0} does not appear to be a valid email address.")]
        //[Display(Name = "SharePoint Email")]
        //[StringLength(160, MinimumLength = 2, ErrorMessage = "{0} is too long or too short.")]
        //public string SharePointEmail { get; set; }

        public string Id { get; set; }
        public string Email { get; set; }
        public FilterAppUserRole FilterAppUserRole { get; set; }
    }
}