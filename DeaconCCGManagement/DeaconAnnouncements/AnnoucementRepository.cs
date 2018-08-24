using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Configuration;
using Elmah;

namespace DeaconCCGManagement.DeaconAnnouncements
{
    public class AnnoucementRepository
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

        public bool IsInitialized = false;

        public AnnoucementRepository()
        {
        }

        public bool Init()
        {
            if (IsInitialized) return true;

            bool useLocalEmulator =
              bool.Parse(ConfigurationManager.AppSettings["UseLocalStorageEmulator"]);

            _tableName = ConfigurationManager.AppSettings["AnnouncementsTableName"];

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

        public bool AddAnnouncement(AnnouncementModel announcement)
        {
            if (!Init()) return false;

            // Create the TableOperation object that inserts the announcement entity.
            var insertOperation = TableOperation.InsertOrReplace(announcement);

            // Execute the insert operation.
            _table.Execute(insertOperation);

            return true;
        }

        public AnnouncementModel GetAnnouncementByKey(string partitionKey, string rowKey)
        {
            if (!Init()) return null;

            // Create a retrieve operation that takes a announcement entity.
            var retrieveOperation =
                TableOperation.Retrieve<AnnouncementModel>(partitionKey, rowKey);

            // Execute the retrieve operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            return (AnnouncementModel)retrievedResult.Result;
        }

        public bool RemoveAnnouncement(string partitionKey, string rowKey)
        {
            if (!Init()) return false;

            // Create a retrieve operation that expects a connection entity.
            var retrieveOperation =
                TableOperation.Retrieve<AnnouncementModel>(partitionKey, rowKey);

            // Execute the operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            // Assign the result to a announcement entity.
            AnnouncementModel deleteEntity = (AnnouncementModel)retrievedResult.Result;

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

        public IEnumerable<AnnouncementModel> GetAllAnnouncements(string partitionKey)
        {
            if (!Init()) return null;

            var connections = new List<AnnouncementModel>();

            var query = new TableQuery<AnnouncementModel>()
                          .Where(TableQuery.GenerateFilterCondition(
                          "PartitionKey",
                          QueryComparisons.Equal,
                          partitionKey)); 

            connections = _table.ExecuteQuery(query).ToList();

            return connections;
        }
    }
}