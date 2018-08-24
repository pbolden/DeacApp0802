using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.Services;


namespace DeaconCCGManagement.Controllers
{
    [Authorize]
    public class CalendarController : ControllerBase
    {
        private CalendarService _calService;

        public CalendarController()
        {
            _calService = new CalendarService(base.unitOfWork);
        }

        // GET: Calendar
        public ActionResult Index(bool getAll)
        {
            ViewBag.GetAll = getAll;
            return View();
        }

        public JsonResult GetEvents(DateTime start, DateTime end, bool getAll)
        {
            var eventsForDate = _calService.GetEventsForDateRange(start, end, User.Identity.Name, getAll);
            
            var events = from e in eventsForDate
                let eventDate = e.EventDate
                where eventDate != null
                select new
                         {
                             id = e.Id,
                             title = e.Title,
                             description = e.Description,
                             firstName = e.FirstName,
                             lastName = e.LastName,
                             start = eventDate.Value.ToString("yyyy-MM-dd"),
                             dateString = e.DateString,
                             phoneNumber = e.PhoneNumber,
                             cellNumber = e.CellPhoneNumber,
                             email = e.EmailAddress,
                             memberDetailsUrl = e.Url,
                             url = "#",
                };

            var eventsList = events.ToArray();
            return Json(eventsList, JsonRequestBehavior.AllowGet);
        }
    }
}