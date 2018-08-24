using System.Web;
using System.Web.Mvc;
using System.Collections.Generic;
using System.Linq;
using DeaconCCGManagement.DAL;

namespace DeaconCCGManagement.Infrastructure
{
    // Pro ASP.NET 5.0 p. 466
    // Inherit from AuthorizeAttribute

    /// <summary>
    /// Custom authorize attribute used to allow or not allow users 
    /// to access classes or methods based on the user's role.
    /// </summary>
    public sealed class CCGAuthorizeAttribute : AuthorizeAttribute
    {
        readonly List<string> _roles;

        public CCGAuthorizeAttribute(params string[] roles)
        {
            _roles = roles.ToList();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
                return false;

            using (var unitOfWork = new UnitOfWork())
            {
                // The 'Name' should be the user's email address.
                var email = httpContext.User.Identity.Name;

                // Check if user belongs to any role in collection
                return _roles.Any(role => unitOfWork.AppUserRepository.IsInRole(email, role));
            }

           

        }
    }
}
