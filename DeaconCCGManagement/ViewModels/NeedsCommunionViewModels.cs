using System;
using System.ComponentModel.DataAnnotations;
using DeaconCCGManagement.enums;
using PagedList;

namespace DeaconCCGManagement.ViewModels
{
    public class NeedsCommunionViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Needs Communion?")]
        public bool NeedsCommunion { get; set; }

        [Display(Name = "Last Request Date")]
        [DataType(DataType.Date)]
        public DateTime? LastSelected { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Display(Name = "Member")]
        public string FullName => $"{FirstName} {LastName}";

        public bool SelectionAllowed { get; set; }

        public string ReturnUrl { get; set; }

        public int MemberId { get; set; }
    }

    public class NeedsCommunionListViewModel
    {
        public int Id { get; set; }
        public IPagedList<NeedsCommunionViewModel> MembersNeedingCommunion { get; set; }
        public CommunionDateRange CommunionDateRange { get; set; }
        public PurgeNeedsCommunion PurgeNeedsCommunion { get; set; }
        public int Count { get; set; }
    }
}