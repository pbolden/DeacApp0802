using System;
using System.Web.Mvc;

namespace DeaconCCGManagement.Controllers
{
    public class HomeController : ControllerBase
    {
        public ActionResult Index()
        {
            //var name = User.Identity.Name;
            //SignalRHub.NotifyHub.AddDummyNotifications(name);            

            return View();
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


        // TEST
        public ActionResult SideMenu()
        {
            return View();
        }
    }
}