using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Auth;
using System.IO;
using System.Configuration;
using Elmah;

namespace DeaconCCGManagement.BlobStorage
{
    public class BlobStorageReposity : IBlobStorageRepository
    {
        // auth access to MS storage account
        private StorageCredentials _storageCredentials;

        // access cloud storage name 
        private CloudStorageAccount _cloudStorageAccount;

        // cloud logical reg
        private CloudBlobClient _cloudBlobClient;

        // blob container
        private CloudBlobContainer _cloudBlobContainer;

        private string _containerName;

        bool _hasContainerAccess = false;

        bool _isInitialized = false;

        public BlobStorageReposity()
        {
            
        }

        public bool Init()
        {
            if (_isInitialized) return true;

            bool useLocalBlob = bool.Parse(ConfigurationManager.AppSettings["UseLocalStorageEmulator"]);

            if (useLocalBlob)
            {
                try
                {
                    // use Azure storage emulator for local development
                    _cloudStorageAccount = CloudStorageAccount.DevelopmentStorageAccount;
                    _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
                    _containerName = ConfigurationManager.AppSettings["BlobStorageContainerRaw"];
                    _cloudBlobContainer = _cloudBlobClient.GetContainerReference(_containerName);
                    _cloudBlobContainer.CreateIfNotExists();
                    _cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
                    _hasContainerAccess = true;
                    _isInitialized = true;
                }
                catch (StorageException ex)
                {
                    // set to false and ignore
                    _hasContainerAccess = false;
                    _isInitialized = false;

                    // log caught exception with Elmah
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                catch (Exception ex)
                {
                    _hasContainerAccess = false;
                    _isInitialized = false;

                    // log caught exception with Elmah
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }

            }
            else // use Azure blob storage on the cloud
            {
                try
                {
                    //
                    // Azure cloud blob storage account
                    //
                    string accountName = ConfigurationManager.AppSettings["AzureStorageAccountName"];
                    string key = ConfigurationManager.AppSettings["AzureStorageAccountKey"];
                    _containerName = ConfigurationManager.AppSettings["BlobStorageContainerRaw"];
                    _storageCredentials = new StorageCredentials(accountName, key);
                    _cloudStorageAccount = new CloudStorageAccount(_storageCredentials, true);
                    _cloudBlobClient = _cloudStorageAccount.CreateCloudBlobClient();
                    _cloudBlobContainer = _cloudBlobClient.GetContainerReference(_containerName);
                    _cloudBlobContainer.SetPermissions(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
                    _hasContainerAccess = true;
                    _isInitialized = true;                    
                }
                catch (StorageException ex)
                {
                    // set to false and ignore
                    _hasContainerAccess = false;
                    _isInitialized = false;

                    // log caught exception with Elmah
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
                catch (Exception ex)
                {
                    _hasContainerAccess = false;
                    _isInitialized = false;

                    // log caught exception with Elmah
                    ErrorSignal.FromCurrentContext().Raise(ex);
                }
            }

            return _isInitialized;
        }

        public Uri GetBlobUri(string fileName)
        {
            if (!Init()) return null;
            if (string.IsNullOrEmpty(fileName)) return null;
            if (!_hasContainerAccess) return null;

            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(_containerName);
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
            return blockBlob.Uri;
        }



        public bool DeleteBlob(string fileName)
        {
            if (!Init()) return false;
            if (string.IsNullOrEmpty(fileName)) return false;
            if (!_hasContainerAccess) return false;
            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);
            bool deleted = blockBlob.DeleteIfExists();

            return deleted;
        }

        public async Task<bool> DownloadBlobAsync(string fileName)
        {
            //if (!Init()) return false;

            //_cloudBlobContainer = _cloudBlobClient.GetContainerReference(_containerName);
            //CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);


            //using (var fileStream = File.OpenWrite(Path.Combine(_downloadPath, fileName)))
            //{
            //    await blockBlob.DownloadToStreamAsync(fileStream);
            //    return true;
            //}

            return true;

        }

        public IEnumerable<BlobViewModel> GetBlobs()
        {
            if (!Init()) return null;
            if (!_hasContainerAccess) return null;
            var context = _cloudBlobContainer.ListBlobs().ToList();
            IEnumerable<BlobViewModel> vm = context.Select(x => new BlobViewModel
            {

                BlobContainerName = x.Container.Name,
                StorageUri = x.StorageUri.PrimaryUri.ToString(),
                PrimaryUri = x.StorageUri.PrimaryUri.ToString(),
                Uri = x.Uri,
                ActualFileName = x.Uri.AbsoluteUri.Substring(x.Uri.AbsoluteUri.LastIndexOf("/") + 1),
                FileExtension = Path.GetExtension(x.Uri.AbsoluteUri.Substring(x.Uri.AbsoluteUri.LastIndexOf("/") + 1))

            }).ToList();

            return vm;
        }

        public bool UploadBlob(HttpPostedFileBase blobFile, string fileName)
        {
            if (!Init()) return false;
            if (blobFile == null) return false;
            if (!_hasContainerAccess) return false;

            _cloudBlobContainer = _cloudBlobClient.GetContainerReference(_containerName);

            CloudBlockBlob blockBlob = _cloudBlobContainer.GetBlockBlobReference(fileName);

            using (var fileStream = blobFile.InputStream)
            {
                blockBlob.UploadFromStream(fileStream);
                return true;
            }           

        }

       
    }
}