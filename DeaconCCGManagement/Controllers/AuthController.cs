using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using DeaconCCGManagement.enums;
using DeaconCCGManagement.Models;
using Microsoft.Owin.Security;
using DeaconCCGManagement.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using DeaconCCGManagement.Services;

namespace DeaconCCGManagement.Controllers
{
    //
    // Authentication controller for off-line development only
    //
    public class AuthController : ControllerBase
    {
        ApplicationSignInManager _signInManager;
        ApplicationUserManager _userManager;
        private AuthService _service;

        #region Constructors
        public AuthController()
        {
            _service = new AuthService(base.unitOfWork);
        }
        public AuthController(ApplicationUserManager userManager, ApplicationSignInManager signInManager) : this()
        {
            UserManager = userManager;
            SignInManager = signInManager;

        }
        #endregion

        #region Properties
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        // GET: Auth/DevelopmentSignIn
        // Off-line development only.
        [AllowAnonymous]
        public ActionResult DevelopmentSignIn()
        {
            //return Content("off-line sign-in");

            if (Request.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.AppUsers = new SelectList(unitOfWork.AppUserRepository.FindUsers(), "Email", "Email");
            return View(new DevelopmentSignInViewModel());
        }

        // GET: Auth/SignIn
        [AllowAnonymous]
        public ActionResult SignIn()
        {
            if (Request.IsAuthenticated)
                return Redirect(Url.Action("Index", "Home"));

            // Off-line development
            if (AuthService.IsOfflineDevelopment())
                return RedirectToAction(nameof(DevelopmentSignIn));
           
            // Redirect to Account controller for 3rd Party login
            return RedirectToAction(nameof(SignIn), "Account");
        }

        // GET: Auth/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            // Get the external login info
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                //
                // Temporary. If no login info is obtained from the external login,
                // the user should get redirected to a custom error page or
                // perhaps redirected to the 3rd party sign-in to try again.
                //
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);
            }

            // Get user instance by email
            var user = GetUser(loginInfo.Email);

            // Add name as claim to the cookie to allow for name display in the view.
            AddNameAsClaim(user);

            // If the user logs-in through a 3rd party, this will login the
            // user with the application which will give Identity   
            // the user's roles and other information. 
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                return View("Lockout");
                //case SignInStatus.RequiresVerification:
                //    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default: // login unsuccessful
                         //
                         // Not sure what we want to do here.
                         //
                return new HttpStatusCodeResult(HttpStatusCode.Unauthorized);

                // If the user does not have an account, then prompt the user to create an account
                //ViewBag.ReturnUrl = returnUrl;
                //ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                //return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> DevelopmentSignIn(DevelopmentSignInViewModel model)
        {
            if (model == null)
                return RedirectToAction(nameof(DevelopmentSignIn), "Auth");

            // Find user where SharePoint email equals the selected email.
            var user = GetUser(model.Email);

            if (user != null)
            {
                //AddNameAsClaim(user);

                await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
            }
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult FilterUsers(DevelopmentSignInViewModel model)
        {
            switch (model.FilterAppUserRole)
            {
                case FilterAppUserRole.Admin:
                ViewBag.AppUsers = new SelectList(unitOfWork.AppUserRepository.FindUsersByRole(AppUserRole.Admin), "Email", "Email");
                break;
                case FilterAppUserRole.DeaconLeadership:
                ViewBag.AppUsers = new SelectList(unitOfWork.AppUserRepository.FindUsersByRole(AppUserRole.DeaconLeadership), "Email", "Email");
                break;
                case FilterAppUserRole.Deacon:
                ViewBag.AppUsers = new SelectList(unitOfWork.AppUserRepository.FindUsersByRole(AppUserRole.Deacon), "Email", "Email");
                break;
                case FilterAppUserRole.Pastor:
                ViewBag.AppUsers = new SelectList(unitOfWork.AppUserRepository.FindUsersByRole(AppUserRole.Pastor), "Email", "Email");
                break;
                case FilterAppUserRole.All:
                default:
                return RedirectToAction(nameof(DevelopmentSignIn));
            }

            return View(nameof(DevelopmentSignIn), model);
        }

        private CCGAppUser GetUser(string email)
        {
            // Find user where SharePoint email equals the selected email.
            return unitOfWork.AppUserRepository.FindUser(u => u.Email.Equals(email));
        }

        private void AddNameAsClaim(CCGAppUser user)
        {
            if (user != null)
            {
                // Adds claim with name which is appended to the cookie. This allows
                // for the display of the user's name in the nav bar.
                UserManager.AddClaim(user.Id, new Claim(ClaimTypes.GivenName, user.FirstName));
            }
        }

        [AllowAnonymous]
        public PartialViewResult UserRoles(string email)
        {
            if (string.IsNullOrEmpty(email))
                return null;
            var user = unitOfWork.AppUserRepository.FindUserByEmail(email);
            var roles = unitOfWork.AppUserRepository.GetUserRoles(user.Id);
            return PartialView("_UserRoles", roles);
        }

        [AllowAnonymous]
        public ActionResult AjaxTest()
        {
            return PartialView("_AjaxTest");
        }

        [AllowAnonymous]
        public ActionResult SignOut()
        {
            // Off-line development
            if (AuthService.IsOfflineDevelopment())
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                return RedirectToAction("SignOutCallback");
            }

            // 3rd Party authentication for online testing and production
            return RedirectToAction(nameof(SignOut), "Account");
        }

        [AllowAnonymous]
        public ActionResult SignOutCallback()
        {
            if (User.Identity.IsAuthenticated)
            {
                // Redirect to home page if the user is authenticated.
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (_userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }
            if (_signInManager != null)
            {
                _signInManager.Dispose();
                _signInManager = null;
            }
            base.Dispose(disposing);
        }

        #region Helpers

        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get { return HttpContext.GetOwinContext().Authentication; }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri) : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        #endregion
    }
}
