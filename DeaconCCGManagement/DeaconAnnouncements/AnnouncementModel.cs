using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace DeaconCCGManagement.DeaconAnnouncements
{
    public class AnnouncementModel : TableEntity
    {
        public DateTime ExpirationDate { get; set; }

        public string Title { get; set; }       

        public string AnnouncementHtml { get; set; }

        public AnnouncementModel(string partitionKey, string rowKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        public AnnouncementModel() { }
        


    }
}