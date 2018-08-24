using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class ChangeRequestViewModel
    {
        public int Id { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = false, DataFormatString = "{0:d}")]
        public DateTime CRDate { get; set; }

        public AppUser AppUserFrom { get; set; }

        public List<AppUser> AppUsersTo { get; set; }

        public EditMemberViewModel CurrentMemberData { get; set; }

        public EditMemberViewModel NewMemberData { get; set; }

        public class AppUser
        {
            public string Id { get; set; }
            public string FullName { get; set; }
        }
    }
}