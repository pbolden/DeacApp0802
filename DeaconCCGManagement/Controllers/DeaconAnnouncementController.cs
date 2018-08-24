using DeaconCCGManagement.Helpers;
using DeaconCCGManagement.Infrastructure;
using DeaconCCGManagement.PushNotifications;
using DeaconCCGManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DeaconCCGManagement.Controllers
{
    [CCGAuthorize("Pastor", "Deacon Leadership", "Administrator")]
    public class DeaconAnnouncementController : ControllerBase
    {
        // GET: DeaconAnnouncement
        public ActionResult Index()
        {
            return View(new NotificationViewModel());
        }

        public ActionResult SendDeaconsEmail()
        {
            return View();
        }

        public ActionResult SendDeaconsTextMessage()
        {
            return View();
        }        

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendDeaconsNotification(NotificationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var appUsers = unitOfWork.AppUserRepository.FindUsersByRole(enums.AppUserRole.Deacon);

                // If url is not empty and link text is, use title as link text.
                string linkText = !string.IsNullOrEmpty(viewModel.Url) 
                    && string.IsNullOrEmpty(viewModel.LinkText) ? viewModel.Title : viewModel.LinkText;

                foreach (var appUser in appUsers)
                {
                    if (appUser.UserName != User.Identity.Name)
                    {
                        NotifyHelper.SendUserNotification(appUser.UserName, 
                            viewModel.Title, viewModel.Message, 
                            viewModel.Url, linkText, viewModel.NotifyType);
                    }
                }
                string title = "Bulk Notifications Sent";
                string message = "Your notification to all deacons have been sent.";
                NotificationType notifyType = NotificationType.Info;
                NotifyHelper.SendUserNotification(User.Identity.Name, title, message, type: notifyType);
            }
            return View("Index", viewModel);
        }


    }
}