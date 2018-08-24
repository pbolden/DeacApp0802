using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace DeaconCCGManagement.BlobStorage
{
    public class BlobViewModel
    {
        public string BlobContainerName { get; set; }
        public Uri Uri { get; set; }
        public string StorageUri { get; set; }
        public string ActualFileName { get; set; }
        public string PrimaryUri { get; set; }
        public string FileExtension { get; set; }

        public string FileNameWithoutExt
        {
            get
            {
                return Path.GetFileNameWithoutExtension(ActualFileName);
            }
        }

        public string FileNameExtOnly
        {
            get
            {
                return Path.GetExtension(ActualFileName).Substring(1);
            }
        }


    }
}