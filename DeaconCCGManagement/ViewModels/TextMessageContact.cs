using System;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class TextMessageContact
    {
        [ScaffoldColumn(false)]
        public int MemberId { get; set; }

        [Required(ErrorMessage = "Please enter your text message.")]
        [Display(Name = "Text Message")]
        [DataType(DataType.MultilineText)]
        [MaxLength(160, ErrorMessage = "Too long.")]
        public string Message { get; set; }

        [ScaffoldColumn(false)]
        public string MemberFullName { get; set; }

        [Required]
        [Display(Name = "To Phone Number")]
        public string ToPhoneNumber { get; set; }

        [Required]
        [Display(Name = "From Phone Number")]
        public string FromPhoneNumber { get; set; }

        [ScaffoldColumn(false)]
        public DateTime DateSent { get; set; }
    }
}