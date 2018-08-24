using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace DeaconCCGManagement.PushNotifications
{
    public class NotificationModel : TableEntity, IComparable<NotificationModel>
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public string LinkText { get; set; }

        [IgnoreProperty]
        public NotificationType NotifyType { get; set; }

        public int NotifyTypeInt { get; set; }

        public NotificationModel(string userId, string rowKey)
        {
            this.PartitionKey = userId;
            this.RowKey = rowKey;
        }

        public NotificationModel() { }

        public int CompareTo(NotificationModel other)
        {
            if (this.Timestamp > other.Timestamp)
                return 1;
            else if (this.Timestamp < other.Timestamp)
                return -1;
            else 
                return 0;          
        }
    }

    public enum NotificationType
    {
        Info, Warning, Success, Failure
    }
}