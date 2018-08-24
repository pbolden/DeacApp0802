using DeaconCCGManagement.PushNotifications;
using System;
using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.ViewModels
{
    public class NotificationViewModel
    {
        [Display(Name = "Title")]
        [StringLength(160, ErrorMessage = "{0} is too long!")]
        [Required(ErrorMessage = "{0} is required.")]
        public string Title { get; set; }

        [Display(Name = "Message")]
        [StringLength(500, ErrorMessage = "{0} is too long!")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage = "{0} is required.")]
        public string Message { get; set; }

        [Display(Name = "Link")]       
        [Url(ErrorMessage = "Invalid web address")]
        public string Url { get; set; }

        [Display(Name = "Link Text")]
        public string LinkText { get; set; }

        [Display(Name = "Notification Type")]
        public NotificationType NotifyType { get; set; }
    }
}