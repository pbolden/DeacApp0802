using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DeaconCCGManagement.Models
{
    public class CCGAppUser : IdentityUser
    {
        //
        // Base class has an Id property that holds a string.
        //

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "{0} is required.")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        public string FullName => $"{FirstName} {LastName}";

        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "{0} does not appear to be a valid email address.")]
        public string EmailAddress { get; set; }

        [Display(Name = "Cell Number")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [DataType(DataType.PhoneNumber)]
        public string CellNumber { get; set; }

        [Display(Name = "CR Manager")]
        public bool ChangeRequestManager { get; set; }

        // Name of the photo file stored in blob
        public string PhotoFileName { get; set; }

        // FK Relationship with CCG
        public int? CcgId { get; set; }

        // Navigational properties
        public virtual CCG CCG { get; set; }

        /// <summary>
        /// Used by ASP.NET Identity to create an identity to represent the user.
        /// </summary>
        /// <param name="manager"></param>
        /// <returns></returns>
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<CCGAppUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);


            // Add custom user claims here
            //
            // TEST add claim (role)
            //
            //var claim1 = new Claim(ClaimTypes.Role, "NewRole1");
            //var claim2 = new Claim(ClaimTypes.Role, "NewRole2");
            //userIdentity.AddClaim(claim1);
            //userIdentity.AddClaim(claim2);


            return userIdentity;
        }

    }
}