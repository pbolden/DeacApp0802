using System;
using System.Web.Mvc;

namespace DeaconCCGManagement.Controllers
{
    [System.Web.Mvc.Authorize]
    public class TestController : Controller
    {
        // GET: Test
        static Random _randomizer = new Random();

        public ActionResult Test1()
        {
            return View();
        }

        public ActionResult DatePicker()
        {
            return View();
        }

        public ActionResult InputMask()
        {
            return View();
        }

        public ActionResult DropDownMenuTest()
        {
            return View();
        }

        public ActionResult ModalDialog()
        {
            return View();
        }


    }
}