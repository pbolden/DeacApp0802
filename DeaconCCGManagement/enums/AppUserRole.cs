using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.enums
{
    /// <summary>
    /// Enumeration of roles for application users.
    /// </summary>
    public enum AppUserRole
    {
        [Display(Name = "Administrator")]
        Admin,

        [Display(Name = "Deacon Leadership")]
        DeaconLeadership,

        Deacon,
        Pastor,
        None
    }
    /// <summary>
    /// Enumeration of roles used for dropdown selection in views.
    /// Eliminates 'None' as an option.
    /// </summary>
    public enum SelectedAppUserRole
    {
        [Display(Name = "Administrator")]
        Admin,

        [Display(Name = "Deacon Leadership")]
        DeaconLeadership,
        Deacon,
        Pastor
    }

    /// <summary>
    /// Enumeration of roles used for dropdown filter in views.
    /// </summary>
    public enum FilterAppUserRole
    {
        [Display(Name = "Administrator")]
        Admin,

        [Display(Name = "Deacon Leadership")]
        DeaconLeadership,
        Deacon,
        Pastor,
        All
    }
}