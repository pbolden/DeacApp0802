using Elmah;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;


namespace DeaconCCGManagement.PushNotifications
{
    public class NotificationsRepository
    {
        // auth access to MS storage account
        private StorageCredentials _storageCredentials;

        // access cloud storage name 
        private CloudStorageAccount _storageAccount;

        // cloud table client
        private CloudTableClient _tableClient;

        // cloud table
        private CloudTable _table;

        // cloud table name
        string _tableName;

        bool IsInitialized = false;

        // Option to use this repository for sent notifications as well 
        bool _sentNotifications = false;

        public NotificationsRepository()
        {

        }

        public NotificationsRepository(bool sentNotifications)
        {
            _sentNotifications = sentNotifications;
        }

        public bool Init()
        {
            if (IsInitialized) return true;
          
            bool useLocalEmulator = 
                bool.Parse(ConfigurationManager.AppSettings["UseLocalStorageEmulator"]);

            // SentNotifications and Notifications are two separate tables
            if (_sentNotifications)
                _tableName = ConfigurationManager.AppSettings["SentNotificationsTableName"];
            else
                _tableName = ConfigurationManager.AppSettings["NotificationsTableName"];

            try
            {

                if (useLocalEmulator)
                {
                    // Parse the connection string and return a reference to the storage account.
                    _storageAccount = CloudStorageAccount.Parse(
                        CloudConfigurationManager.GetSetting("StorageConnectionString"));
                }
                else
                {
                    string accountName = ConfigurationManager.AppSettings["AzureStorageAccountName"];
                    string key = ConfigurationManager.AppSettings["AzureStorageAccountKey"];               
                    _storageCredentials = new StorageCredentials(accountName, key);
                    _storageAccount = new CloudStorageAccount(_storageCredentials, true);
                }

                // Create the table client.            
                _tableClient = _storageAccount.CreateCloudTableClient();

                // Retrieve a reference to the table.
                _table = _tableClient.GetTableReference(_tableName);

                // Create the table if it doesn't exist.
                _table.CreateIfNotExists();

                IsInitialized = true;                
            }
            catch (StorageException ex)
            {
                IsInitialized = false;

                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);
            }
            catch (Exception ex)
            {
                IsInitialized = false;

                // log caught exception with Elmah
                ErrorSignal.FromCurrentContext().Raise(ex);
            }

            return IsInitialized;
        }

        public bool AddNotification(NotificationModel notification)
        {
            if (!Init()) return false;

            notification.NotifyTypeInt = (int)notification.NotifyType;

            // Create the TableOperation object that inserts the notification entity.
            var insertOperation = TableOperation.InsertOrReplace(notification);

            // Execute the insert operation.
            _table.Execute(insertOperation);

            return true;
        }

        public bool RemoveNotification(string userId, string notificationId)
        {
            if (!Init()) return false;

            // Create a retrieve operation that expects a notification entity.
            var retrieveOperation =
                TableOperation.Retrieve<NotificationModel>(userId, notificationId);

            // Execute the operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            // Assign the result to a Notification entity.
            NotificationModel deleteEntity = (NotificationModel)retrievedResult.Result;

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                _table.Execute(deleteOperation);
                return true;
            }
            return false;

        }

        public NotificationModel GetNotificationByKey(string userId, string notificationId)
        {
            if (!Init()) return null;

            // Create a retrieve operation that takes a Notification entity.
            var retrieveOperation =
                TableOperation.Retrieve<NotificationModel>(userId, notificationId);

            // Execute the retrieve operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            return (NotificationModel)retrievedResult.Result;
        }

        public IEnumerable<NotificationModel> GetNotificationsByUserId(string userId)
        {
            if (!Init()) return null;

            // Get all notifications for one user.
            var query = new TableQuery<NotificationModel>()
                            .Where(TableQuery.GenerateFilterCondition(
                            "PartitionKey",
                            QueryComparisons.Equal,
                            userId));

            var notifications = _table.ExecuteQuery(query).ToList();

            return notifications;
        }

        public IEnumerable<NotificationModel> GetAllNotifications()
        {
            if (!Init()) return null;

            var notifications = new List<NotificationModel>();

            var query = new TableQuery<NotificationModel>()
                          .Where(TableQuery.GenerateFilterCondition(
                          "PartitionKey",
                          QueryComparisons.NotEqual,
                          "0-0-0-0-0-0-0-0-0-0-0-0_123456789")); // lame but I don't see another way

            notifications = _table.ExecuteQuery(query).ToList();

            return notifications;
        }

    }
}