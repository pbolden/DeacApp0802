using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DeaconCCGManagement.BlobStorage
{
    public interface IBlobStorageRepository
    {
        IEnumerable<BlobViewModel> GetBlobs();
        bool DeleteBlob(string fileName);
        bool UploadBlob(HttpPostedFileBase blobFile, string fileName);
        Task<bool> DownloadBlobAsync(string fileName);
    }
}