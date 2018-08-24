using DeaconCCGManagement.DAL;
using DeaconCCGManagement.Models;
using DeaconCCGManagement.PushNotifications;
using DeaconCCGManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DeaconCCGManagement.Helpers
{
    public static class NotifyHelper
    {
        /// <summary>
        /// Sends the user notification.
        /// </summary>
        /// <param name="who">The username of recipient (user's email).</param>
        /// <param name="title">The title of the notification.</param>
        /// <param name="message">The message.</param>
        /// <param name="url">The URL (if any).</param>
        /// <param name="linkText">The link text (if any url).</param>
        /// <param name="type">The notification type.</param>
        public static void SendUserNotification(string who, string title,
            string message, string url=null, string linkText=null, NotificationType type=NotificationType.Info)
        {
            if (string.IsNullOrEmpty(who)) return;
            if (string.IsNullOrEmpty(message)) return;

            var notifyRepo = new NotificationsRepository();

            var notification = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = string.IsNullOrEmpty(title) ? string.Empty : title,
                Message = message,
                Url = string.IsNullOrEmpty(url) ? string.Empty : url,
                LinkText = string.IsNullOrEmpty(linkText) ? string.Empty : linkText,
                NotifyType = type
            };
            notifyRepo.AddNotification(notification);
        }

        public static void AddSentNotification(string who, string title, string message)
        {
            if (string.IsNullOrEmpty(who)) return;

            var notifyRepo = new NotificationsRepository(sentNotifications: true);
            var notification = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = string.IsNullOrEmpty(title) ? string.Empty : title,
                Message = message,
            };
            notifyRepo.AddNotification(notification);
        }

        public static void NotifyOfMembersSpecialDates(string userEmail)
        {
            using (var unitOfWork = new UnitOfWork())
            {
                var calService = new CalendarService(unitOfWork);
                var specialOccasions = calService.GetEventsForDateRange(DateTime.Now, DateTime.Now.AddDays(2), userEmail);

                if (specialOccasions == null || specialOccasions.Count() == 0) return;

                // Repository for sent notifications
                var notifyRepo = new NotificationsRepository(sentNotifications: true);
                var sentNotifications = notifyRepo.GetNotificationsByUserId(userEmail).ToList();                

                string message = string.Empty;
                foreach (var specialOccasion in specialOccasions)
                {
                    message = $"{specialOccasion.Description} is on {specialOccasion.DateString}";

                    // To prevent user getting the same special occasion notifications multiple times
                    if (!HasNotificationBeenSent(sentNotifications, specialOccasion.Title, message))
                    {
                        SendUserNotification(userEmail,
                            specialOccasion.Title,
                            message,
                            specialOccasion.Url,
                            specialOccasion.Title, // link text
                            type: NotificationType.Info);

                        AddSentNotification(userEmail, specialOccasion.Title, message);
                    }                       

                }

                // Get rid of old sent notifications after a set time span
                RemoveOldSentNotifications(userEmail, sentNotifications, notifyRepo);
            }
        }

        public static bool HasNotificationBeenSent(List<NotificationModel> sentNotifications,
            string title, string message)
        {
            foreach (var sentNotification in sentNotifications)
                if (sentNotification.Title.Equals(title) && sentNotification.Message.Equals(message))
                    return true;                          

            return false;
        }

        /// <summary>
        /// Removes the old sent notifications.
        /// </summary>
        /// <param name="userEmail">The user email.</param>
        /// <param name="sentNotifications">The sent notifications.</param>
        public static void RemoveOldSentNotifications(string userEmail,
            List<NotificationModel> sentNotifications, NotificationsRepository sentNotifyRepo)
        {
            int daysOld = 30;
            var timeSpan = TimeSpan.FromDays(daysOld);

            var dateTimeOffset = DateTime.Now.Subtract(timeSpan);
            foreach (var sentNotification in sentNotifications)
            {
                // Remove connection if older that 'dateTimeOffset'
                if (sentNotification.Timestamp.DateTime <= dateTimeOffset)
                    sentNotifyRepo.RemoveNotification(sentNotification.PartitionKey, sentNotification.RowKey);

            }

        }
    }
}