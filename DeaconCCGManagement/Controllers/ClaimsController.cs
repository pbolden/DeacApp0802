using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using DeaconCCGManagement.Helpers;

namespace DeaconCCGManagement.Controllers
{
    public class ClaimsController : ControllerBase
    {
        [Authorize]
        public ViewResult Index()
        {
            var identity = User.Identity as ClaimsIdentity;
            return View(identity.Claims);
        }



        public ActionResult TestAddClaims()
        {
            var identity = User.Identity as ClaimsIdentity;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Role, "Pastor"),
                new Claim(ClaimTypes.Role, "Administrator"),
                new Claim(ClaimTypes.Role, "Deacon Leadership"),
                new Claim(ClaimTypes.Role, "Deacon"),
                new Claim(ClaimTypes.Role, "TestRole"),
            };
            Request.LogonUserIdentity.AddClaims(claims);

            //identity.AddClaims(claims);

            var userSurname = AuthHelper.GetUserSurname(User.Identity.Name);
            var claim = new Claim(ClaimTypes.Surname, userSurname);

            if (!identity.HasClaim(c => c.Type == ClaimTypes.Surname))
            {
                Request.LogonUserIdentity.AddClaim(claim);
                //identity.AddClaim(claim);
            }

            return RedirectToAction("Index");
        }
    }
}