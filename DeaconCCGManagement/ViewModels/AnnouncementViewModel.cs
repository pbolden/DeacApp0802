using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DeaconCCGManagement.ViewModels
{
    public class AnnouncementViewModel : IComparable<AnnouncementViewModel>
    {
        [Required(ErrorMessage = "Please enter an expiration date.")]
        [Display(Name = "Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        [ScaffoldColumn(false)]
        public DateTime TimeStamp { get; set; }

        [Required(ErrorMessage = "Please enter a title.")]
        public string Title { get; set; }

        [AllowHtml]
        [Display(Name = "Announcement")]        
        public string AnnouncementHtml { get; set; }

        [ScaffoldColumn(false)]
        public string PartitionKey { get; set; }

        [ScaffoldColumn(false)]
        public string RowKey { get; set; }   

        public int CompareTo(AnnouncementViewModel other)
        {
            if (this.TimeStamp < other.TimeStamp)
                return 1;
            else if (this.TimeStamp > other.TimeStamp)
                return -1;
            else
                return 0;
        }
    }
}