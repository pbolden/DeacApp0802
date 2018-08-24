using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeaconCCGManagement.Models
{
    /// <summary>
    /// This domain model will model a contact record.
    /// </summary>
    public class ContactRecord
    {
        // Set Key for Record Identification
        [Key]
        [Display(Name = "Contact ID")]
        public int Id { get; set; }

        [Display(Name = "Member")]     
        public string MemberFullName {
            get
            {
                if (CCGMember == null)                
                    return string.Empty;
                
                return $"{CCGMember.FirstName} {CCGMember.LastName}";
            }
        }

        [Display(Name = "Deacon")]
        public string DeaconFullName
        {
            get
            {
                if (AppUser == null)                
                    return string.Empty;
                
                return $"{AppUser.FirstName} {AppUser.LastName}";
            }
        }

        // Keep comments private
        [Display(Name = "Keep Comments Private")]
        public bool Private { get; set; }

        // Date of Contact
        [Required(ErrorMessage = "{0} is a required field")]
        [Display(Name = "Contact Date")]
        public DateTime ContactDate { get; set; }

        // Contact Timestamp
        [Required(ErrorMessage = "Timestamp is auto-generated, if not displayed contact the administrator.")]
        public DateTime Timestamp { get; set; }

        // Duration of a contact.
        [Display(Name = "Contact Duration")]
        public TimeSpan? Duration { get; set; }

        // Pass Along to pastor/leadership?
        [Display(Name = "Pass Along")]
        public bool PassAlong { get; set; }

        // Pass along comments to pastor/leadership
        [Display(Name = "Deacon's Message")]
        [StringLength(1000, ErrorMessage = "{0} is too long!")]
        public string PassAlongComments { get; set; }

        // Follow up comments
        [Display(Name = "Follow Up Comments")]
        [StringLength(1000, ErrorMessage = "{0} is too long!")]
        public string PassAlongFollowUpComments { get; set; }

        // Contact Subject
        [Display(Name = "Subject")]
        [StringLength(160, ErrorMessage = "{0} is too long!")]
        public string Subject { get; set; }

        // Comment Section
        [Display(Name = "Comments")]
        [StringLength(1000, ErrorMessage = "{0} is too long!")]
        public string Comments { get; set; }

        // Archive boolean flag
        [Display(Name = "Archive")]
        public bool Archive { get; set; }

        // Foreign key relationship with Contact Type
        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Contact Type")]
        public int ContactTypeId { get; set; }
   
        // Id of user who made the contact
        public string AppUserId { get; set; }

        // Foreign key relationship with CCGMember
        [ForeignKey("CCGMember")]
        public int CCGMemberId { get; set; }
       
        // Navigational properties
        public virtual CCGMember CCGMember { get; set; }
        public virtual CCGAppUser AppUser { get; set;}
        public virtual ContactType ContactType { get; set; }

        // One-to-one relationship with PassAlongContact
        public virtual PassAlongContact PassAlongContact { get; set; }
    }
}