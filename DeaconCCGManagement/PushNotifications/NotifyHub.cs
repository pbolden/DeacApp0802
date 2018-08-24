using DeaconCCGManagement.Helpers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DeaconCCGManagement.PushNotifications
{
    [Microsoft.AspNet.SignalR.Authorize]
    [HubName("notifyHub")]
    public class NotifyHub : Hub
    {
        [HubMethodName("sendNotification")]
        public void SendNotification()
        {
            var name = Context.User.Identity.Name;

            if (string.IsNullOrEmpty(name)) return;

            //
            // Azure table storage repositories
            //
            var connRepository = new ConnectionsRepository();
            var notifyRepo = new NotificationsRepository();

            if (!notifyRepo.Init()) return;
            if (!connRepository.Init()) return;

            var connections = connRepository.GetConnectionsByUserId(name).ToList();
            var notifications = notifyRepo.GetNotificationsByUserId(name).ToList();
            notifications.Sort();

            if (connections.Count > 0 && notifications.Count > 0)
            {
                // Send notification(s) to each user connection.
                foreach (var connection in connections)
                {
                    foreach (var notification in notifications)
                    {
                        string json = System.Web.Helpers.Json.Encode(new
                        {
                            title = notification.Title,
                            message = notification.Message,
                            url = string.IsNullOrEmpty(notification.Url) ? string.Empty : notification.Url,
                            linkText = string.IsNullOrEmpty(notification.LinkText) ? string.Empty : notification.LinkText,
                            type = (int)notification.NotifyTypeInt,
                            notificationId = notification.RowKey
                        });

                        // Calls method on client side to show notification
                        Clients.Client(connection.RowKey).addNotification(json);
                    }
                }
            }
        }     

        [HubMethodName("getNotificationsCount")]
        public void GetNotificationsCount()
        {
            var name = Context.User.Identity.Name;
            if (string.IsNullOrEmpty(name)) return;
            var notifyRepo = new NotificationsRepository();
            if (!notifyRepo.Init()) return;
            var notifications = notifyRepo.GetNotificationsByUserId(name);
            Clients.Caller.updateNotificationsCount(notifications.Count());
        }

        [HubMethodName("removeNotification")]
        public void RemoveNotification(string notificationId)
        {
            if (string.IsNullOrEmpty(notificationId)) return;
            var notifyRepo = new NotificationsRepository();
            if (!notifyRepo.Init()) return;
            var name = Context.User.Identity.Name;
            notifyRepo.RemoveNotification(name, notificationId);
        }

        public override Task OnConnected()
        {
            var name = Context.User.Identity.Name;

            if (!string.IsNullOrEmpty(name))
            {
                var repository = new ConnectionsRepository();
                if (repository.Init())
                {
                    var connection = new ConnectionModel(name, Context.ConnectionId, DateTime.Now);

                    repository.AddConnection(connection);

                    // TODO testing only
                    //var connections = repository.GetAllConnections();  

                }
            }

            return base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            var repository = new ConnectionsRepository();
            if (repository.Init())
            {
                RemoveConnection(name, Context.ConnectionId, repository);
            }

            return base.OnDisconnected(stopCalled);
        }

        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

        public void RemoveConnection(string name, string connectionId, ConnectionsRepository repo)
        {
            if (!repo.Init()) return;

            if (!string.IsNullOrEmpty(name))
            {
                repo.RemoveConnection(name, connectionId);

                // Just in case user has old connections in table
                RemoveOldUserConnections(name, 1, repo);

                // TODO testing only
                //var connections = repo.GetAllConnections();

            }
        }
       
        public void RemoveOldUserConnections(string who, int daysOld, ConnectionsRepository repo)
        {
            if (!repo.Init()) return;

            var connections = repo.GetConnectionsByUserId(who).ToList();

            var timeSpan = TimeSpan.FromDays(daysOld);         

            var dateTimeOffset = DateTime.Now.Subtract(timeSpan);
            foreach (var connection in connections)
            {
                // Remove connection if older that 'daysOld'
                if (connection.ConnectionTimeStamp <= dateTimeOffset)
                    RemoveConnection(connection.PartitionKey, connection.RowKey, repo);
            }
        }

        [HubMethodName("notifyOfMembersSpecialDates")]
        public void NotifyOfMembersSpecialDates()
        {
            var name = Context.User.Identity.Name;
            NotifyHelper.NotifyOfMembersSpecialDates(name);
        }

        /// <summary>
        /// Adds dummy notifications for testing.
        /// </summary>
        /// <param name="who">The who.</param>
        public static bool AddDummyNotifications(string who)
        {
            var repository = new NotificationsRepository();

            if (!repository.Init()) return false;

            var dummyNotifications = new List<NotificationModel>();
            var notification1 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "Lorem ipsum dolor",
                Message = "Duis aute irure dolor in reprehenderit in voluptate",
                NotifyType = NotificationType.Warning
            };

            var notification2 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "Excepteur sint occaecat",
                Message = "Excepteur sint occaecat cupidatat non proident," +
                          "sunt in culpa qui officia deserunt mollit",
                NotifyType = NotificationType.Warning
            };

            var notification3 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "Mauris ipsum arcu",
                Message = "Mauris ipsum arcu, feugiat non tempor tincidunt sit amet turpis",
                Url = "/Home/Contact",
                LinkText = "ipsum arcu!",
                NotifyType = NotificationType.Info
            };

            var notification4 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "Ut a diam magna",
                Message = "Nulla convallis, orci in sodales blandit",
                NotifyType = NotificationType.Success
            };

            var notification5 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "lorem augue feugiat",
                Message = "vitae dapibus mi ligula quis ligula." +
                          "Aenean mattis pulvinar est quis bibendum.",
                NotifyType = NotificationType.Success
            };

            var notification6 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "Donec posuere pulvinar",
                Message = "nec sagittis lacus pharetra ac",
                NotifyType = NotificationType.Failure
            };

            var notification7 = new NotificationModel(who, Guid.NewGuid().ToString())
            {
                Title = "Pellentesque et magna",
                Message = "Donec velit vulputate nec tristique vitae",
                NotifyType = NotificationType.Info
            };

            dummyNotifications.Add(notification1);
            dummyNotifications.Add(notification2);
            dummyNotifications.Add(notification3);
            dummyNotifications.Add(notification4);
            dummyNotifications.Add(notification5);
            dummyNotifications.Add(notification6);
            dummyNotifications.Add(notification7);


            foreach (var notification in dummyNotifications)
                repository.AddNotification(notification);

            return true;
            //var notifications = repository.GetNotificationsByUserId(who);

        }
    }

}