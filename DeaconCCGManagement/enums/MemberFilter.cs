using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.enums
{
    public enum MemberFilter
    {
        [Display(Name = "No Photo")]
        NoPhoto,

        [Display(Name = "Active Member")]
        ActiveMember,

        [Display(Name = "Inactive Member")]
        InactiveMember,

        [Display(Name = "Needs Communion")]
        NeedsCommunion,

        [Display(Name = "No Contact for 1 Month")]
        NoContactForOneMonth,

        [Display(Name = "No Contact for 2 Months")]
        NoContactForTwoMonths,

        [Display(Name = "No Contact for 3 Months")]
        NoContactForThreeMonths,

        [Display(Name = "No Contact for 6 Months")]
        NoContactForSixMonths,

        [Display(Name = "All Members")]
        None
    }
}