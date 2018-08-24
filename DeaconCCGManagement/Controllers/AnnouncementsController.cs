using DeaconCCGManagement.DeaconAnnouncements;
using DeaconCCGManagement.PushNotifications;
using DeaconCCGManagement.ViewModels;
using Elmah;
using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Mvc;
using System.Web.Security.AntiXss;

namespace DeaconCCGManagement.Controllers
{
    public class AnnouncementsController : Controller
    {
        // GET: Announcements
        public ActionResult Index()
        {
            var viewModel = AnnouncementHelper.GetAllAnnouncements();


            // Send status notification if an announcement has been 
            // created or updated. User is sent back to index view
            // after saving announcement.
            string title = string.Empty;
            string message = string.Empty;           
            int notifyInt = 2;
            bool hasStatusNotification = false;
            // to send notification that announcement created successfully
            if (AnnouncementHelper.CreatedNewAnnouncement)
            {
                hasStatusNotification = true;
                title = "New Announcement Created";
                message = $"Your new announcement has been created.";
                notifyInt = (int)NotificationType.Success;               
            }

            // to send notification that announcement updated successfully
            if (AnnouncementHelper.UpdatedAnnouncement)
            {
                hasStatusNotification = true;
                title = "Announcement Updated";
                message = $"Your announcement has been updated.";
                notifyInt = (int)NotificationType.Success;
            }

            if (hasStatusNotification)
            {
                ViewBag.StatusTitle = title;
                ViewBag.StatusMessage = message;
                ViewBag.NotifyInt = notifyInt;
                ViewBag.HasStatusNotification = hasStatusNotification;
            }
            else
            {
                ViewBag.HasStatusNotification = false;
            }

            AnnouncementHelper.CreatedNewAnnouncement = false;
            AnnouncementHelper.UpdatedAnnouncement = false;

            return View(viewModel);
        }       

        // GET: Announcements/Create
        public ActionResult Create()
        {
            var viewModel = new AnnouncementViewModel
            {
                ExpirationDate = DateTime.Now.Add(TimeSpan.FromDays(7))
            };
            return View(viewModel);
        }

        // POST: Announcements/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AnnouncementViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);

            try
            {
                // sanitize the html from user
                viewModel.AnnouncementHtml =
                    AntiXssEncoder.HtmlEncode(viewModel.AnnouncementHtml, false);

                // add announcement to Azure table storage
                AnnouncementHelper.AddAnnouncent(viewModel);

                AnnouncementHelper.CreatedNewAnnouncement = true;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);

                return View(viewModel);
            }          

        }  

        // GET: Announcements/Edit/5
        public ActionResult Edit(string partitionKey, string rowKey)
        {
            var viewModel = AnnouncementHelper.GetAnnouncementViewModel(partitionKey, rowKey);
            if (viewModel == null) viewModel = new AnnouncementViewModel();

            viewModel.AnnouncementHtml = WebUtility.HtmlDecode(viewModel.AnnouncementHtml);

            return View("Create", viewModel);
        }

        // POST: Announcements/Edit/5
        [HttpPost]
        public ActionResult Edit(AnnouncementViewModel viewModel)
        {
            try
            {
                // sanitize the html from user
                viewModel.AnnouncementHtml =
                    AntiXssEncoder.HtmlEncode(viewModel.AnnouncementHtml, false);

                // add announcement to Azure table storage
                AnnouncementHelper.UpdateAnnouncent(viewModel);

                AnnouncementHelper.UpdatedAnnouncement = true;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);

                return View();
            }
        }

        // GET: Announcements/Delete/5
        public ActionResult Delete(string partitionKey, string rowKey)
        {
            var viewModel = AnnouncementHelper.GetAnnouncementViewModel(partitionKey, rowKey);
            if (viewModel == null) viewModel = new AnnouncementViewModel();

            return View(viewModel);
        }

        // POST: Announcements/Delete/5
        [HttpPost]
        public ActionResult Delete(AnnouncementViewModel viewModel)
        {
            try
            {
                AnnouncementHelper.DeleteAnnouncement(viewModel.PartitionKey, viewModel.RowKey);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);

                return View();
            }
        }

    }
}
