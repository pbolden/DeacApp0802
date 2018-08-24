using System.Web.Mvc;

namespace DeaconCCGManagement.Controllers
{
    [System.Web.Mvc.Authorize]
    public class BootstrapTestController : Controller
    {
        // GET: BootstrapTest
        public ActionResult Index()
        {
            return View();
        }
    }
}