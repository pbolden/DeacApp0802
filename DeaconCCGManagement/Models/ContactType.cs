using System.ComponentModel.DataAnnotations;

namespace DeaconCCGManagement.Models
{
    /// <summary>
    /// Domain model for Contact Type reference table.
    /// Represents various methods for contacting members (i.e., e-mail, text, phone, etc.)
    /// </summary>
    public class ContactType
    {
        [Key]
        [Display(Name = "Contact Type ID")]
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [StringLength(160, ErrorMessage = "{0} is too long.")]
        [Display(Name = "Contact Type")]
        public string Name { get; set; }
    }
}