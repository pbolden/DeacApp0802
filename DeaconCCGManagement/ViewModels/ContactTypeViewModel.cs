using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.ViewModels
{
    public class ContactTypeViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(160, ErrorMessage = "{0} is too long.")]
        [Display(Name = "Contact Type")]
        public string Name { get; set; }

        public string ReturnUrl { get; set; }
    }
}