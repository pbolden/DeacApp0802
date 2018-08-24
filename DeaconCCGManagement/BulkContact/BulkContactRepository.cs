using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace DeaconCCGManagement.BulkContact
{
    public class BulkContactRepository
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
        string _tableName = "BulkContacts";

        bool IsInitialized = false;

        public BulkContactRepository()
        {

        }

        public bool Init()
        {
            if (IsInitialized) return true;

            try
            {
                // Parse the connection string and return a reference to the storage account.
                _storageAccount = CloudStorageAccount.Parse(
                    CloudConfigurationManager.GetSetting("StorageConnectionString"));

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
                // TODO log exception
            }
            catch (Exception ex)
            {
                IsInitialized = false;
                // TODO log exception
            }

            return IsInitialized;
        }


        public void TempStoreMemberIds()
        {

        }
    }
}