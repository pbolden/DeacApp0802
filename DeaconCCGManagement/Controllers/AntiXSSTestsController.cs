using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security.AntiXss;
using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Models;
using AntiXssEncoder = System.Web.Security.AntiXss.AntiXssEncoder;

namespace DeaconCCGManagement.Controllers
{
    public class AntiXSSTestsController : Controller
    {
        private CcgDbContext db = new CcgDbContext();

        public ActionResult Index()
        {
            return View(new List<AntiXSSTest>());
        }

        public ActionResult Create()
        {
            return View();
        }
        [ValidateInput(false)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AntiXSSTest model)
        {
            var msgUnsanitized = model.Message;
            var msgSanitized = AntiXssEncoder.HtmlEncode(model.Message, false);

            string msgOutput = $"Unsanitized: {msgUnsanitized}  <br /> Sanitized: {msgSanitized}";

            return Content(msgOutput);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
