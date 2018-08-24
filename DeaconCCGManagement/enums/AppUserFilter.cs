using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.enums
{
    public enum AppUserFilter
    {
        [Display(Name = "All App Users")]
        AllAppUsers,

        Administrator,

        [Display(Name = "Deacon Leadership")]
        DeaconLeadership,

        Deacon,
        Pastor,

        [Display(Name = "Admin and Deacon")]
        AdminAndDeacon,

        [Display(Name = "Leadership and Deacon")]
        LeadershipAndDeacon,

        [Display(Name = "Change Request Manager")]
        ChangeRequestManager
    }
}