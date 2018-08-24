using System.Web.Mvc;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.Services;

namespace DeaconCCGManagement.Controllers
{
    [CCGAuthorize("Administrator", "Deacon Leadership")]
    public class AdminController : ControllerBase
    {
        private readonly PrayerRequestService _prayerRequestService;

        public AdminController()
        {
            _prayerRequestService = new PrayerRequestService(unitOfWork);
        }

        // GET: Admin Console
        public ActionResult Index()
        {
            return View();
        }
    }
}
