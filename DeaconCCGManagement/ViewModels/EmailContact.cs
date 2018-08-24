using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DeaconCCGManagement.ViewModels
{
    public class EmailContact
    {
        protected const string InvalidEmailErrMsg = "This email address does not appear to be valid.";

        public int MemberId { get; set; }

        [Required]
        [Display(Name = "From Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = InvalidEmailErrMsg)]
        public string FromEmailAddress { get; set; }

        public string Alias { get; set; }

        public string SenderName { get; set; }

        public string ReceiverName { get; set; }

        [Required]
        [Display(Name = "To Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = InvalidEmailErrMsg)]
        public string ToEmailAddress { get; set; }

        [Display(Name = "Cc Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = InvalidEmailErrMsg)]
        public string CcEmailAddress { get; set; }

        [Display(Name = "Bcc Email Address")]
        [DataType(DataType.EmailAddress, ErrorMessage = InvalidEmailErrMsg)]
        public string BccEmailAddress { get; set; }

        [Required(ErrorMessage = "Please enter a subject.")]
        [Display(Name = "Subject")]
        [MaxLength(260, ErrorMessage = "Sorry, that's too long.")]
        public string Subject { get; set; }
       
        // Plain text will be converted from the html body
        public string PlainTextBody { get; set; }

        [AllowHtml]
        [Required(ErrorMessage = "Please enter the email's body text.")]
        [Display(Name = "Body")]
        [MaxLength(10000, ErrorMessage = "Sorry, that's too long.")]
        public string HtmlBody { get; set; }

        public DateTime DateSent { get; set; }
    }
}