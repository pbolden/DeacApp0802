using DeaconCCGManagement.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class CCGViewModel
    {
        public int Id { get; set; }

        [Display(Name = "CCG")]
        public string CCGName { get; set; }

        // used to concat ccg name with deacon last names
        public ICollection<CCGAppUser> AppUsers { get; set; }

        public string ReturnUrl { get; set; }
    }
}