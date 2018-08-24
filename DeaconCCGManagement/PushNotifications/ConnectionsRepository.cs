using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.Azure;
using System.Configuration;
using Elmah;

namespace DeaconCCGManagement.PushNotifications
{
    public class ConnectionsRepository
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

        public ConnectionsRepository()
        {
        }

        public bool Init()
        {
            if (IsInitialized) return true;

            bool useLocalEmulator =
              bool.Parse(ConfigurationManager.AppSettings["UseLocalStorageEmulator"]);

            _tableName = ConfigurationManager.AppSettings["ConnectionsTableName"];

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

        public bool AddConnection(ConnectionModel connection)
        {
            if (!Init()) return false;

            // Create the TableOperation object that inserts the connection entity.
            var insertOperation = TableOperation.InsertOrReplace(connection);

            // Execute the insert operation.
            _table.Execute(insertOperation);

            return true;
        }

        public bool RemoveConnection(string userId, string connectionId)
        {
            if (!Init()) return false;

            // Create a retrieve operation that expects a connection entity.
            var retrieveOperation =
                TableOperation.Retrieve<ConnectionModel>(userId, connectionId);

            // Execute the operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            // Assign the result to a Connection entity.
            ConnectionModel deleteEntity = (ConnectionModel)retrievedResult.Result;

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

        public ConnectionModel GetConnectionByKey(string userId, string connectionId)
        {
            if (!Init()) return null;

            // Create a retrieve operation that takes a connection entity.
            var retrieveOperation =
                TableOperation.Retrieve<ConnectionModel>(userId, connectionId);

            // Execute the retrieve operation.
            TableResult retrievedResult = _table.Execute(retrieveOperation);

            return (ConnectionModel)retrievedResult.Result;
        }

        public IEnumerable<ConnectionModel> GetConnectionsByUserId(string userId)
        {
            if (!Init()) return null;

            // Get all connections for one user.
            var query = new TableQuery<ConnectionModel>()
                            .Where(TableQuery.GenerateFilterCondition(
                            "PartitionKey",
                            QueryComparisons.Equal,
                            userId));

            var connections = _table.ExecuteQuery(query).ToList();

            return connections;
        }

        public IEnumerable<ConnectionModel> GetAllConnections()
        {
            if (!Init()) return null;

            var connections = new List<ConnectionModel>();

            var query = new TableQuery<ConnectionModel>()
                          .Where(TableQuery.GenerateFilterCondition(
                          "PartitionKey",
                          QueryComparisons.NotEqual,
                          "0000000000000000000000000123456789")); // lame but I don't see another way

            connections = _table.ExecuteQuery(query).ToList();

            return connections;
        }

    }
}