using DeaconCCGManagement.DeaconAnnouncements;
using Elmah;
using System;
using System.Web.Mvc;

using DeaconCCGManagement.AzureFunctions;


namespace DeaconCCGManagement.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            var viewModel = AnnouncementHelper.GetAllAnnouncements();          

            return View(viewModel);
        }

        public ActionResult About()
        {
            ViewBag.Message = "About this ZMBC Deacons CCG Management App.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your ZMBC Contacts.";

            return View();
        }
        
    }
}