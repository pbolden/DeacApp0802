using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.Models
{
    public class CCG
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "CCG Name")]
        public string CCGName { get; set; }

        // Used for sorting CCGs alphabetically.
        public int CompareTo(CCG other)
        {
            return string.Compare(CCGName, other.CCGName,
                StringComparison.CurrentCultureIgnoreCase);
        }

        // Navigational properties
        public virtual ICollection<CCGAppUser> AppUsers { get; set; }
        public virtual ICollection<CCGMember> CCGMembers { get; set; }

        public override string ToString()
        {
            return CCGName;
        }
    }
}