using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.Models
{
    public class MemberPhoto
    {
        public int Id { get; set; }
        
        [Required]
        public byte[] Photo { get; set; }

        [Required]
        public byte[] Thumbnail { get; set; }

        [Required]
        [MaxLength(50)]
        public string MimeType { get; set; }
   
        // Foreign key property
        public int? MemberId { get; set; }

        // Foreign key property
        public string AppUserId { get; set; }

        // Navigational property
        public virtual CCGMember Member { get; set; }
        public virtual CCGAppUser AppUser { get; set; }
     
    }
}